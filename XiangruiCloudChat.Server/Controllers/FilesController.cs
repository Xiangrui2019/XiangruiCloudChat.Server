using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Aiursoft.Pylon;
using Aiursoft.Pylon.Attributes;
using Aiursoft.Pylon.Models;
using Aiursoft.Pylon.Services;
using Aiursoft.Pylon.Services.ToOSSServer;
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
    [AiurForceAuth(directlyReject: true)]
    public class FilesController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDbContext _dbContext;
        private readonly IConfiguration _configuration;
        private readonly ServiceLocation _serviceLocation;
        private readonly StorageService _storageService;
        private readonly AppsContainer _appsContainer;
        private readonly SecretService _secretService;

        public FilesController(
            UserManager<ApplicationUser> userManager,
            ApplicationDbContext dbContext,
            IConfiguration configuration,
            ServiceLocation serviceLocation,
            StorageService storageService,
            AppsContainer appsContainer,
            SecretService secretService)
        {
            _userManager = userManager;
            _dbContext = dbContext;
            _configuration = configuration;
            _serviceLocation = serviceLocation;
            _storageService = storageService;
            _appsContainer = appsContainer;
            _secretService = secretService;
        }

        [HttpPost]
        [FileChecker(MaxSize = 5 * 1024 * 1024)]
        [APIModelStateChecker]
        public async Task<IActionResult> UploadIcon()
        {
            var file = Request.Form.Files.First();
            if (!file.FileName.IsStaticImage())
            {
                return this.Protocol(ErrorType.InvalidInput, "只支持图片类型的文件. 请上传 `jpg`,`png`, or `bmp` 后缀的文件.");
            }
            var uploadedFile = await _storageService.SaveToOSS(file, Convert.ToInt32(_configuration["ChatUserIconsBucketId"]), 1000, SaveFileOptions.RandomName);
            return this.ChatJson(new UploadImageViewModel
            {
                Code = ErrorType.Success,
                Message = $"成功的上传了您的头像, 但是我们没有更新您的个人信息. 现在您可以调用 `/auth/{nameof(AuthController.UpdateInfo)}` 更新您的个人信息.",
                FileKey = uploadedFile.FileKey,
                DownloadPath = $"{_serviceLocation.OSS}/Download/FromKey/{uploadedFile.FileKey}"
            });
        }

        [HttpPost]
        [FileChecker]
        [APIModelStateChecker]
        public async Task<IActionResult> UploadMedia()
        {
            var file = Request.Form.Files.First();
            if (!file.FileName.IsImageMedia() && !file.FileName.IsVideo())
            {
                return this.Protocol(ErrorType.InvalidInput, "只支持图片和视频类型的文件. 请上传 `jpg`,`png`, `bmp`, `mp4`, `ogg` or `webm` 后缀的文件.");
            }
            var uploadedFile = await _storageService.SaveToOSS(file, Convert.ToInt32(_configuration["ChatPublicBucketId"]), 400);
            return this.ChatJson(new UploadImageViewModel
            {
                Code = ErrorType.Success,
                Message = "成功的上传了您的文件!",
                FileKey = uploadedFile.FileKey,
                DownloadPath = $"{_serviceLocation.OSS}/Download/FromKey/{uploadedFile.FileKey}"
            });
        }

        [HttpPost]
        [FileChecker]
        [APIModelStateChecker]
        public async Task<IActionResult> UploadFile(UploadFileAddressModel model)
        {
            var conversation = await _dbContext.Conversations.SingleOrDefaultAsync(t => t.Id == model.ConversationId);
            if (conversation == null)
            {
                return this.Protocol(ErrorType.NotFound, $"找不到对应的会话: {model.ConversationId}!");
            }
            var user = await _userManager.GetUserAsync(User);
            if (!await _dbContext.VerifyJoined(user.Id, conversation))
            {
                return this.Protocol(ErrorType.Unauthorized, $"您没有这个会话的文件上传权限: {conversation.Id}!");
            }
            var file = Request.Form.Files.First();
            var uploadedFile = await _storageService.SaveToOSS(file, Convert.ToInt32(_configuration["ChatSecretBucketId"]), 200);
            var fileRecord = new FileRecord
            {
                FileKey = uploadedFile.FileKey,
                SourceName = Path.GetFileName(file.FileName.Replace(" ", "")),
                UploaderId = user.Id,
                ConversationId = conversation.Id
            };
            _dbContext.FileRecords.Add(fileRecord);
            await _dbContext.SaveChangesAsync();
            return this.ChatJson(new UploadFileViewModel
            {
                Code = ErrorType.Success,
                Message = "成功的上传了文件!",
                FileKey = uploadedFile.FileKey,
                SavedFileName = fileRecord.SourceName,
                FileSize = file.Length
            });
        }

        [HttpPost]
        public async Task<IActionResult> FileDownloadAddress(FileDownloadAddressAddressModel model)
        {
            var record = await _dbContext
                .FileRecords
                .Include(t => t.Conversation)
                .SingleOrDefaultAsync(t => t.FileKey == model.FileKey);
            if (record?.Conversation == null)
            {
                return this.Protocol(ErrorType.NotFound, "无法找到这个文件!");
            }
            var user = await _userManager.GetUserAsync(User);
            if (!await _dbContext.VerifyJoined(user.Id, record.Conversation))
            {
                return this.Protocol(ErrorType.Unauthorized, $"您在这个会话中没有下载文件的权限: {record.Conversation.Id}!");
            }
            var secret = await _secretService.GenerateAsync(record.FileKey, await _appsContainer.AccessToken(), 1);
            return this.ChatJson(new FileDownloadAddressViewModel
            {
                Code = ErrorType.Success,
                Message = "成功的生成了文件下载链接!",
                FileName = record.SourceName,
                DownloadPath = $"{_serviceLocation.OSS}/Download/FromSecret?Sec={secret.Value}&sd=true&name={record.SourceName}"
            });
        }
    }
}