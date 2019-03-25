using System;
using System.Linq;
using System.Threading.Tasks;
using Aiursoft.Pylon;
using Aiursoft.Pylon.Attributes;
using Aiursoft.Pylon.Models;
using Aiursoft.Pylon.Models.ForApps.AddressModels;
using Aiursoft.Pylon.Models.Stargate.ListenAddressModels;
using Aiursoft.Pylon.Services;
using Aiursoft.Pylon.Services.ToAPIServer;
using Aiursoft.Pylon.Services.ToStargateServer;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using XiangruiCloudChat.Server.Data;
using XiangruiCloudChat.Server.Models;
using XiangruiCloudChat.Server.Models.ApiAddressModels;
using XiangruiCloudChat.Server.Models.ApiViewModels;
using XiangruiCloudChat.Server.Services;

namespace XiangruiCloudChat.Server.Controllers
{
    [APIExpHandler]
    [APIModelStateChecker]
    public class AuthController : Controller
    {
        private readonly ServiceLocation _serviceLocation;
        private readonly IConfiguration _configuration;
        private readonly IHostingEnvironment _env;
        private readonly AuthService<ApplicationUser> _authService;
        private readonly OAuthService _oauthService;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserService _userService;
        private readonly AppsContainer _appsContainer;
        private readonly ChatPushService _pusher;
        private readonly ChannelService _channelService;
        private readonly VersionChecker _version;
        private readonly ApplicationDbContext _dbContext;
        private readonly ThirdPartyPushService _thirdPartyPushService;

        public AuthController(
            ServiceLocation serviceLocation,
            IConfiguration configuration,
            IHostingEnvironment env,
            AuthService<ApplicationUser> authService,
            OAuthService oauthService,
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            UserService userService,
            AppsContainer appsContainer,
            ChatPushService pusher,
            ChannelService channelService,
            VersionChecker version,
            ApplicationDbContext dbContext,
            ThirdPartyPushService thirdPartyPushService)
        {
            _serviceLocation = serviceLocation;
            _configuration = configuration;
            _env = env;
            _authService = authService;
            _oauthService = oauthService;
            _userManager = userManager;
            _signInManager = signInManager;
            _userService = userService;
            _appsContainer = appsContainer;
            _pusher = pusher;
            _channelService = channelService;
            _version = version;
            _dbContext = dbContext;
            _thirdPartyPushService = thirdPartyPushService;
        }

        public IActionResult Version()
        {
            var latest = _version.CheckVersion();
            return this.ChatJson(new VersionViewModel
            {
                LatestVersion = latest,
                OldestSupportedVersion = latest,
                Message = "成功的获取了当前最新版本号.",
                DownloadAddress = ""
            });
        }

        [HttpPost]
        public async Task<IActionResult> AuthByPassword(AuthByPasswordAddressModel model)
        {
            var pack = await _oauthService.PasswordAuthAsync(Extends.CurrentAppId, $"{model.PhoneNumber}@xiangruikong.com", model.Password);
            if (pack.Code != ErrorType.Success)
            {
                return this.Protocol(ErrorType.Unauthorized, pack.Message);
            }
            var user = await _authService.AuthApp(new AuthResultAddressModel
            {
                code = pack.Value,
                state = string.Empty
            }, isPersistent: true);
            if (!await _dbContext.AreFriends(user.Id, user.Id))
            {
                _dbContext.AddFriend(user.Id, user.Id);
                await _dbContext.SaveChangesAsync();
            }

            await _dbContext.OnlineDevices.AddAsync(new OnlineDevice
            {
                UserId = user.Id
            });
            await _dbContext.SaveChangesAsync();

            user.IsOnline = true;
            await _dbContext.SaveChangesAsync();

            return this.ChatJson(new AiurProtocol()
            {
                Code = ErrorType.Success,
                Message = "您已经成功登陆."
            });
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterAddressModel model)
        {
            var result = await _oauthService.AppRegisterAsync($"{model.PhoneNumber}@xiangruikong.com", model.Password, model.ConfirmPassword);
            return this.ChatJson(result);
        }

        [AiurForceAuth(preferController: "", preferAction: "", justTry: false, register: false)]
        public IActionResult OAuth()
        {
            var ApplicationUrl = "";

            if (_env.IsDevelopment())
            {
                ApplicationUrl = _configuration["ApplicationUrls:DevlopementApplicationUrl"];
            }
            else
            {
                ApplicationUrl = _configuration["ApplicationUrls:ProductionApplicationUrl"];
            }

            return Redirect(ApplicationUrl);
        }

        public async Task<IActionResult> AuthResult(AuthResultAddressModel model)
        {
            var user = await _authService.AuthApp(model);
            var ApplicationUrl = "";

            if (_env.IsDevelopment())
            {
                ApplicationUrl = _configuration["ApplicationUrls:DevlopementApplicationUrl"];
            }
            else
            {
                ApplicationUrl = _configuration["ApplicationUrls:ProductionApplicationUrl"];
            }

            await _dbContext.OnlineDevices.AddAsync(new OnlineDevice
            {
                UserId = user.Id
            });
            await _dbContext.SaveChangesAsync();

            user.IsOnline = true;
            await _dbContext.SaveChangesAsync();

            return Redirect(ApplicationUrl);
        }

        public async Task<IActionResult> SignInStatus()
        {
            var user = await _userManager.GetUserAsync(User);
            var signedIn = user != null;
            return this.ChatJson(new AiurValue<bool>(signedIn)
            {
                Code = ErrorType.Success,
                Message = "成功获取了您的登陆状态."
            });
        }

        [AiurForceAuth(directlyReject: true)]
        public async Task<IActionResult> Me()
        {
            var user = await _userManager.GetUserAsync(User);
            user = await _authService.OnlyUpdate(user);
            user.IsMe = true;
            return this.ChatJson(new AiurValue<ApplicationUser>(user)
            {
                Code = ErrorType.Success,
                Message = "成功的获取了您的用户信息."
            });
        }

        [HttpPost]
        [AiurForceAuth(directlyReject: true)]
        public async Task<IActionResult> UpdateInfo(UpdateInfoAddressModel model)
        {
            var cuser = await _userManager.GetUserAsync(User);
            cuser.HeadImgFileKey = model.HeadImgKey;
            cuser.NickName = model.NickName;
            cuser.Bio = model.Bio;
            cuser.MakeEmailPublic = !model.HideMyEmail;
            await _userService.ChangeProfileAsync(cuser.Id, await _appsContainer.AccessToken(), cuser.NickName, cuser.HeadImgFileKey, cuser.Bio);
            await _userManager.UpdateAsync(cuser);
            return this.Protocol(ErrorType.Success, "成功的更新了您的个人信息.");
        }

        [HttpPost]
        [AiurForceAuth(directlyReject: true)]
        public async Task<IActionResult> ChangePassword(ChangePasswordAddresModel model)
        {
            var cuser = await _userManager.GetUserAsync(User);
            await _userService.ChangePasswordAsync(cuser.Id, await _appsContainer.AccessToken(), model.OldPassword, model.NewPassword);
            return this.Protocol(ErrorType.Success, "成功的修改了您的密码!");
        }

        [AiurForceAuth(directlyReject: true)]
        public async Task<IActionResult> InitPusher()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user.CurrentChannel == -1 || (await _channelService.ValidateChannelAsync(user.CurrentChannel, user.ConnectKey)).Code != ErrorType.Success)
            {
                var channel = await _pusher.Init(user.Id);
                user.CurrentChannel = channel.ChannelId;
                user.ConnectKey = channel.ConnectKey;
                await _userManager.UpdateAsync(user);
            }
            var model = new InitPusherViewModel
            {
                Code = ErrorType.Success,
                Message = "成功的获取了您的Stargate Channel.",
                ChannelId = user.CurrentChannel,
                ConnectKey = user.ConnectKey,
                ServerPath = new AiurUrl(_serviceLocation.StargateListenAddress, "Listen", "Channel", new ChannelAddressModel
                {
                    Id = user.CurrentChannel,
                    Key = user.ConnectKey
                }).ToString()
            };
            return this.ChatJson(model);
        }

        [AiurForceAuth(directlyReject: true)]
        public async Task<IActionResult> LogOut(LogOutAddressModel model)
        {
            var user = await _userManager.GetUserAsync(User);
            var onlines = await _dbContext
                .OnlineDevices
                .Where(t => t.UserId == user.Id)
                .AsNoTracking()
                .ToListAsync();
            _dbContext.OnlineDevices.Remove(onlines[0]);
            await _dbContext.SaveChangesAsync();
            var onlinesresult = await _dbContext
                .OnlineDevices
                .Where(t => t.UserId == user.Id)
                .AsNoTracking()
                .ToListAsync();
            Console.WriteLine(onlinesresult);
            if (onlinesresult.Count == 0)
            {
                user.IsOnline = false;
                await _dbContext.SaveChangesAsync();
            }
            else
            {
                user.IsOnline = true;
                await _dbContext.SaveChangesAsync();
            }
            
            var device = await _dbContext
                .Devices
                .Where(t => t.UserID == user.Id)
                .SingleOrDefaultAsync(t => t.Id == model.DeviceId);
            await _signInManager.SignOutAsync();
            if (device == null)
            {
                return this.Protocol(ErrorType.RequireAttention, "成功的登出, 但是没有找到对应的Device: " + model.DeviceId);
            }
            _dbContext.Devices.Remove(device);
            await _dbContext.SaveChangesAsync();
            return this.Protocol(ErrorType.Success, "成功登出.");
        }

        [HttpPost]
        [AiurForceAuth(directlyReject: true)]
        public async Task<IActionResult> AddDevice(AddDeviceAddressModel model)
        {
            var user = await _userManager.GetUserAsync(User);
            if (_dbContext.Devices.Any(t => t.PushP256DH == model.PushP256DH))
            {
                return this.Protocol(ErrorType.HasDoneAlready, "已经有一个带有重复Push 256dh的设备: " + model.PushP256DH);
            }
            var devicesExists = await _dbContext.Devices.Where(t => t.UserID == user.Id).ToListAsync();
            if (devicesExists.Count >= 100)
            {
                var toDrop = devicesExists.OrderBy(t => t.AddTime).First();
                _dbContext.Devices.Remove(toDrop);
                await _dbContext.SaveChangesAsync();
            }
            var device = new Device
            {
                Name = model.Name,
                UserID = user.Id,
                PushAuth = model.PushAuth,
                PushEndpoint = model.PushEndpoint,
                PushP256DH = model.PushP256DH,
                IPAddress = HttpContext.Connection.RemoteIpAddress.ToString()
            };
            _dbContext.Devices.Add(device);
            await _dbContext.SaveChangesAsync();

            return this.ChatJson(new AiurValue<long>(device.Id)
            {
                Code = ErrorType.Success,
                Message = "已成功创建新设备 ID: " + device.Id
            });
        }

        [HttpPost]
        [AiurForceAuth(directlyReject: true)]
        public async Task<IActionResult> UpdateDevice(UpdateDeviceAddressModel model)
        {
            var user = await _userManager.GetUserAsync(User);
            var device = await _dbContext
                .Devices
                .Where(t => t.UserID == user.Id)
                .SingleOrDefaultAsync(t => t.Id == model.DeviceId);
            if (device == null)
            {
                return this.Protocol(ErrorType.NotFound, "无法找到对应的DeviceID: " + model.DeviceId);
            }
            device.Name = model.Name;
            device.PushAuth = model.PushAuth;
            device.PushEndpoint = model.PushEndpoint;
            device.PushP256DH = model.PushP256DH;
            _dbContext.Devices.Update(device);
            await _dbContext.SaveChangesAsync();

            return this.ChatJson(new AiurValue<Device>(device)
            {
                Code = ErrorType.Success,
                Message = "成功的更新了对应的DeviceID: " + device.Id
            });
        }

        [AiurForceAuth(directlyReject: true)]
        public async Task<IActionResult> MyDevices()
        {
            var user = await _userManager.GetUserAsync(User);
            var devices = await _dbContext
                .Devices
                .Where(t => t.UserID == user.Id)
                .OrderByDescending(t => t.AddTime)
                .ToListAsync();
            return this.ChatJson(new AiurCollection<Device>(devices)
            {
                Code = ErrorType.Success,
                Message = "成功的获取了您的所有Device."
            });
        }
    }
}