using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net.Mime;
using System.Threading.Tasks;
using Aiursoft.Pylon;
using Aiursoft.Pylon.Attributes;
using Aiursoft.Pylon.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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
    public class FriendshipController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDbContext _dbContext;
        private readonly ChatPushService _pusher;
        private static readonly object Obj = new object();

        public FriendshipController(
            UserManager<ApplicationUser> userManager,
            ApplicationDbContext dbContext,
            ChatPushService pushService)
        {
            _userManager = userManager;
            _dbContext = dbContext;
            _pusher = pushService;
        }

        public async Task<IActionResult> MyFriends([Required]bool? orderByName)
        {
            var user = await _userManager.GetUserAsync(User);
            var list = new List<ContactInfo>();
            var conversations = await _dbContext.MyConversations(user.Id);
            foreach (var conversation in conversations)
            {
                list.Add(new ContactInfo
                {
                    ConversationId = conversation.Id,
                    DisplayName = conversation.GetDisplayName(user.Id),
                    DisplayImageKey = conversation.GetDisplayImage(user.Id),
                    LatestMessage = conversation.GetLatestMessage().Content,
                    LatestMessageTime = conversation.GetLatestMessage().SendTime,
                    UnReadAmount = conversation.GetUnReadAmount(user.Id),
                    Discriminator = conversation.Discriminator,
                    UserId = (conversation as PrivateConversation)?.AnotherUser(user.Id).Id,
                    AesKey = conversation.AESKey,
                    Muted = conversation is GroupConversation && (await _dbContext.GetRelationFromGroup(user.Id, conversation.Id)).Muted
                });
            }
            list = orderByName == true ?
                list.OrderBy(t => t.DisplayName).ToList() :
                list.OrderByDescending(t => t.LatestMessageTime).ToList();
            return this.ChatJson(new AiurCollection<ContactInfo>(list)
            {
                Code = ErrorType.Success,
                Message = "成功的获取了您的所有好友."
            });
        }

        [HttpPost]
        public async Task<IActionResult> DeleteFriend([Required]string id)
        {
            var user = await _userManager.GetUserAsync(User);
            var target = await _dbContext.Users.FindAsync(id);
            if (target == null)
                return this.Protocol(ErrorType.NotFound, "无法找到对应的好友.");
            if (!await _dbContext.AreFriends(user.Id, target.Id))
                return this.Protocol(ErrorType.NotEnoughResources, "他不是你的朋友.");
            await _dbContext.RemoveFriend(user.Id, target.Id);
            await _dbContext.SaveChangesAsync();
            await _pusher.WereDeletedEvent(target.Id);
            return this.Protocol(ErrorType.Success, "成功删除了您的好友.");
        }

        [HttpPost]
        public async Task<IActionResult> CreateRequest([Required]string id)
        {
            var user = await _userManager.GetUserAsync(User);

            var target = await _dbContext.Users.FindAsync(id);
            if (target == null)
                return this.Protocol(ErrorType.NotFound, "无法找到对应的用户!");
            if (target.Id == user.Id)
                return this.Protocol(ErrorType.RequireAttention, "您不可以加自己为好友!");
            var areFriends = await _dbContext.AreFriends(user.Id, target.Id);
            if (areFriends)
                return this.Protocol(ErrorType.HasDoneAlready, "她已经是您的好友了!");
            Request request;
            lock (Obj)
            {
                var pending = _dbContext.Requests
                    .Where(t => t.CreatorId == user.Id)
                    .Where(t => t.TargetId == id)
                    .Any(t => !t.Completed);
                if (pending)
                    return this.Protocol(ErrorType.HasDoneAlready, "您已经给她发送了好友请求!");
                request = new Request { CreatorId = user.Id, TargetId = id };
                _dbContext.Requests.Add(request);
                _dbContext.SaveChanges();
            }
            await _pusher.NewFriendRequestEvent(target.Id, user.Id);
            return this.ChatJson(new AiurValue<int>(request.Id)
            {
                Code = ErrorType.Success,
                Message = "成功的发送了好友请求!"
            });
        }

        [HttpPost]
        public async Task<IActionResult> CompleteRequest(CompleteRequestAddressModel model)
        {
            var user = await _userManager.GetUserAsync(User);
            var request = await _dbContext.Requests.FindAsync(model.Id);
            if (request == null)
                return this.Protocol(ErrorType.NotFound, "无法找到对应的好友请求.");
            if (request.TargetId != user.Id)
                return this.Protocol(ErrorType.Unauthorized, "对应的好友请求的对象不是你.");
            if (request.Completed)
                return this.Protocol(ErrorType.HasDoneAlready, "这个好友请求已经完成了.");
            request.Completed = true;
            if (model.Accept)
            {
                if (await _dbContext.AreFriends(request.CreatorId, request.TargetId))
                {
                    await _dbContext.SaveChangesAsync();
                    return this.Protocol(ErrorType.RequireAttention, "你们两个已经是朋友了.");
                }
                _dbContext.AddFriend(request.CreatorId, request.TargetId);
                await _pusher.FriendAcceptedEvent(request.CreatorId);
            }
            await _dbContext.SaveChangesAsync();
            return this.Protocol(ErrorType.Success, "成功的完成了这个好友请求.");
        }

        public async Task<IActionResult> MyRequests()
        {
            var user = await _userManager.GetUserAsync(User);
            var requests = await _dbContext
                .Requests
                .AsNoTracking()
                .Include(t => t.Creator)
                .Where(t => t.TargetId == user.Id)
                .OrderByDescending(t => t.CreateTime)
                .ToListAsync();
            return this.ChatJson(new AiurCollection<Request>(requests)
            {
                Code = ErrorType.Success,
                Message = "成功的获取了好友请求列表."
            });
        }

        public async Task<IActionResult> SearchFriends(SearchFriendsAddressModel model)
        {
            var users = await _dbContext
                .Users
                .AsNoTracking()
                .Where(t => t.NickName.Contains(model.NickName, StringComparison.CurrentCultureIgnoreCase))
                .Take(model.Take)
                .ToListAsync();

            return this.ChatJson(new AiurCollection<ApplicationUser>(users)
            {
                Code = ErrorType.Success,
                Message = "成功获取了搜索结果."
            });
        }

        public async Task<IActionResult> DiscoverFriends(int take = 15)
        {
            var cuser = await _userManager.GetUserAsync(User);
            var myFriends = await _dbContext.MyPersonalFriendsId(cuser.Id);
            var calculated = new List<KeyValuePair<int, ApplicationUser>>();
            foreach (var user in await _dbContext.Users.ToListAsync())
            {
                if (await _dbContext.AreFriends(user.Id, cuser.Id) || user.Id == cuser.Id)
                {
                    continue;
                }
                var hisFriends = await _dbContext.MyPersonalFriendsId(user.Id);
                var commonFriends = myFriends.Intersect(hisFriends).Count();
                if (commonFriends > 0)
                {
                    calculated.Add(new KeyValuePair<int, ApplicationUser>(commonFriends, user));
                }
                if (calculated.Count >= take)
                {
                    break;
                }
            }
            var ordered = calculated.OrderByDescending(t => t.Key);
            return this.ChatJson(new AiurCollection<KeyValuePair<int, ApplicationUser>>(ordered)
            {
                Code = ErrorType.Success,
                Message = "成功的发现了可能与您有关的好友."
            });
        }

        public async Task<IActionResult> UserDetail([Required]string id)
        {
            var user = await _userManager.GetUserAsync(User);
            var target = await _dbContext.Users.AsNoTracking().SingleOrDefaultAsync(t => t.Id == id);
            var model = new UserDetailViewModel();
            if (target == null)
            {
                model.Message = "无法找到对应的用户.";
                model.Code = ErrorType.NotFound;
                return this.ChatJson(model);
            }
            var conversation = await _dbContext.FindConversationAsync(user.Id, target.Id);
            if (conversation != null)
            {
                model.AreFriends = true;
                model.ConversationId = conversation.Id;
            }
            else
            {
                model.AreFriends = false;
                model.ConversationId = null;
            }
            model.User = target;
            model.Message = "成功获取了对应用户的信息.";
            model.Code = ErrorType.Success;
            return this.ChatJson(model);
        }

        public async Task<IActionResult> FriendIsOnline([Required] string id)
        {
            var user = await _dbContext.Users.SingleOrDefaultAsync(t => t.Id == id);
            return this.ChatJson(new AiurProtocol
            {
                Code = ErrorType.Success,
                Message = user.IsOnline.ToString()
            });
        }

        [HttpPost]
        public async Task<IActionResult> ReportHim(ReportHimAddressModel model)
        {
            var cuser = await _userManager.GetUserAsync(User);
            var targetUser = await _dbContext.Users.SingleOrDefaultAsync(t => t.Id == model.TargetUserId);
            if (targetUser == null)
            {
                return this.Protocol(ErrorType.NotFound, $"无法找到对应的用户 `{model.TargetUserId}`!");
            }
            if (cuser.Id == targetUser.Id)
            {
                return this.Protocol(ErrorType.HasDoneAlready, "您不可以给您自己发送留言!");
            }
            var exists = await _dbContext
                .Reports
                .AnyAsync((t) => t.TriggerId == cuser.Id && t.TargetId == targetUser.Id && t.Status == ReportStatus.Pending);
            
            _dbContext.Reports.Add(new Report
            {
                TargetId = targetUser.Id,
                TriggerId = cuser.Id,
                Reason = model.Reason
            });
            await _dbContext.SaveChangesAsync();
            return this.Protocol(ErrorType.Success, $"成功的给这个用户发送了留言: {targetUser.Email}!");
        }
    }
}