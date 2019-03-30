using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using XiangruiCloudChat.Server.Core;
using XiangruiCloudChat.Server.Core.Attributes;
using XiangruiCloudChat.Server.Core.Models;
using XiangruiCloudChat.Server.Data;
using XiangruiCloudChat.Server.Models;
using XiangruiCloudChat.Server.Models.ApiAddressModels;
using XiangruiCloudChat.Server.Services;

namespace XiangruiCloudChat.Server.Controllers
{
    [APIExpHandler]
    [APIModelStateChecker]
    [ForceAuthorize(directlyReject: true)]
    public class ConversationController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDbContext _dbContext;
        private readonly ChatPushService _pusher;

        public ConversationController(
            UserManager<ApplicationUser> userManager,
            ApplicationDbContext dbContext,
            ChatPushService pushService)
        {
            _userManager = userManager;
            _dbContext = dbContext;
            _pusher = pushService;
        }

        public async Task<IActionResult> GetMessage([Required]int id, int skipTill = -1, int take = 15)
        {
            var user = await _userManager.GetUserAsync(User);
            var target = await _dbContext.Conversations.FindAsync(id);
            if (!await _dbContext.VerifyJoined(user.Id, target))
                return this.Protocol(ErrorType.Unauthorized, "您没有权限访问这个会话.");

            IQueryable<Message> allMessages = _dbContext
                .Messages
                .AsNoTracking()
                .Where(t => t.ConversationId == target.Id);
            if (skipTill != -1)
                allMessages = allMessages.Where(t => t.Id < skipTill);
            var allMessagesList = await allMessages
                .OrderByDescending(t => t.Id)
                .Take(take)
                .OrderBy(t => t.Id)
                .ToListAsync();
            if (target.Discriminator == nameof(PrivateConversation))
            {
                await _dbContext.Messages
                    .Where(t => t.ConversationId == target.Id)
                    .Where(t => t.SenderId != user.Id)
                    .Where(t => t.Read == false)
                    .ForEachAsync(t => t.Read = true);
            }
            else if (target.Discriminator == nameof(GroupConversation))
            {
                var relation = await _dbContext.UserGroupRelations
                    .SingleOrDefaultAsync(t => t.UserId == user.Id && t.GroupId == target.Id);
                relation.ReadTimeStamp = DateTime.UtcNow;
            }
            await _dbContext.SaveChangesAsync();
            return this.ChatJson(new AiurCollection<Message>(allMessagesList)
            {
                Code = ErrorType.Success,
                Message = $"成功获取了{target.DisplayName}的消息!"
            });
        }

        [HttpPost]
        public async Task<IActionResult> SendMessage(SendMessageAddressModel model)
        {
            var user = await _userManager.GetUserAsync(User);
            var target = await _dbContext.Conversations.FindAsync(model.Id);
            if (!await _dbContext.VerifyJoined(user.Id, target))
                return this.Protocol(ErrorType.Unauthorized, "您没有权限访问这个会话.");
            if (model.Content.Trim().Length == 0)
                return this.Protocol(ErrorType.InvalidInput, "不要发送空白消息哦.");

            var message = new Message
            {
                Content = model.Content,
                SenderId = user.Id,
                ConversationId = target.Id
            };
            _dbContext.Messages.Add(message);
            await _dbContext.SaveChangesAsync();

            if (target is PrivateConversation privateConversation)
            {
                var requester = await _userManager.FindByIdAsync(privateConversation.RequesterId);
                var targetUser = await _userManager.FindByIdAsync(privateConversation.TargetId);
                await _pusher.NewMessageEvent(requester, target, model.Content, user, true);

                if (requester.Id != targetUser.Id)
                {
                    await _pusher.NewMessageEvent(targetUser, target, model.Content, user, true);
                }
            }
            else if (target is GroupConversation)
            {
                var usersJoined = await _dbContext
                    .UserGroupRelations
                    .Include(t => t.User)
                    .Where(t => t.GroupId == target.Id)
                    .ToListAsync();
                var taskList = new List<Task>();
                foreach (var relation in usersJoined)
                {
                    async Task SendNotification()
                    {
                        await _pusher.NewMessageEvent(
                            reciever: relation.User,
                            conversation: target,
                            content: model.Content,
                            sender: user,
                            alert: !relation.Muted);
                    }
                    taskList.Add(SendNotification());
                }
                await Task.WhenAll(taskList);
            }

            return this.Protocol(ErrorType.Success, "您的消息成功发送.");
        }

        public async Task<IActionResult> ConversationDetail([Required]int id)
        {
            var user = await _userManager.GetUserAsync(User);
            var conversations = await _dbContext.MyConversations(user.Id);
            var target = conversations.SingleOrDefault(t => t.Id == id);
            if (target == null)
            {
                return this.Protocol(ErrorType.NotFound, "无法找到对应的会话.");
            }
            target.DisplayName = target.GetDisplayName(user.Id);
            target.DisplayImage = target.GetDisplayImage(user.Id);
            if (target is PrivateConversation privateTarget)
            {
                privateTarget.AnotherUserId = privateTarget.AnotherUser(user.Id).Id;
                return this.ChatJson(new AiurValue<PrivateConversation>(privateTarget)
                {
                    Code = ErrorType.Success,
                    Message = "成功获取了对应的会话信息"
                });
            }
            else if (target is GroupConversation groupTarget)
            {
                var relations = await _dbContext
                    .UserGroupRelations
                    .AsNoTracking()
                    .Include(t => t.User)
                    .Where(t => t.GroupId == groupTarget.Id)
                    .ToListAsync();
                groupTarget.Users = relations;
                return this.ChatJson(new AiurValue<GroupConversation>(groupTarget)
                {
                    Code = ErrorType.Success,
                    Message = "成功获取了对应的会话信息."
                });
            }
            else
            {
                throw new InvalidOperationException("Target is:" + target.Discriminator);
            }
        }
    }
}