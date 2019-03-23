using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
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
using XiangruiCloudChat.Server.Services;

namespace XiangruiCloudChat.Server.Controllers
{
    [APIExpHandler]
    [APIModelStateChecker]
    [AiurForceAuth(true)]
    public class GroupsController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDbContext _dbContext;

        public GroupsController(
            UserManager<ApplicationUser> userManager,
            ApplicationDbContext dbContext)
        {
            _userManager = userManager;
            _dbContext = dbContext;
        }

        public async Task<IActionResult> SearchGroup(SearchGroupAddressModel model)
        {
            var groups = await _dbContext
                .GroupConversations
                .AsNoTracking()
                .Where(t => t.GroupName.Contains(model.GroupName, StringComparison.CurrentCultureIgnoreCase))
                .Take(model.Take)
                .ToListAsync();

            return this.ChatJson(new AiurCollection<GroupConversation>(groups)
            {
                Code = ErrorType.Success,
                Message = "成功的搜索了对应的群组."
            });
        }

        [HttpPost]
        public async Task<IActionResult> CreateGroupConversation(CreateGroupConversationAddressModel model)
        {
            var user = await _userManager.GetUserAsync(User);

            model.GroupName = model.GroupName.Trim().ToLower();
            var exists = _dbContext.GroupConversations.Any(t => t.GroupName == model.GroupName);
            if (exists)
            {
                return this.Protocol(ErrorType.NotEnoughResources, $"这个群名: {model.GroupName} 已经被占用了!");
            }
            var limitedDate = DateTime.UtcNow - new TimeSpan(1, 0, 0, 0);
            var todayCreated = await _dbContext
                .GroupConversations
                .Where(t => t.OwnerId == user.Id)
                .Where(t => t.ConversationCreateTime > limitedDate)
                .CountAsync();
            if (todayCreated > 100)
            {
                return this.Protocol(ErrorType.NotEnoughResources, "您今天创建的群聊太多了!");
            }
            var createdGroup = await _dbContext.CreateGroup(model.GroupName, user.Id, model.JoinPassword);
            var newRelationship = new UserGroupRelation
            {
                UserId = user.Id,
                GroupId = createdGroup.Id,
                ReadTimeStamp = DateTime.MinValue
            };
            _dbContext.UserGroupRelations.Add(newRelationship);
            await _dbContext.SaveChangesAsync();
            return this.ChatJson(new AiurValue<int>(createdGroup.Id)
            {
                Code = ErrorType.Success,
                Message = "你已经成功的创建了一个群聊并加入了它!"
            });
        }

        [HttpPost]
        public async Task<IActionResult> JoinGroup([Required]string groupName, string joinPassword)
        {
            var user = await _userManager.GetUserAsync(User);

            var group = await _dbContext.GroupConversations.SingleOrDefaultAsync(t => t.GroupName == groupName);
            if (group == null)
            {
                return this.Protocol(ErrorType.NotFound, $"无法找到对应的群聊: {groupName}!");
            }
            var joined = _dbContext.UserGroupRelations.Any(t => t.UserId == user.Id && t.GroupId == group.Id);
            if (joined)
            {
                return this.Protocol(ErrorType.HasDoneAlready, $"您已经加入了这个群聊: {groupName}!");
            }
            if (group.HasPassword && group.JoinPassword != joinPassword?.Trim())
            {
                return this.Protocol(ErrorType.WrongKey, "这个群聊需要密码, 但是您的密码不正确!");
            }

            var newRelationship = new UserGroupRelation
            {
                UserId = user.Id,
                GroupId = group.Id
            };
            _dbContext.UserGroupRelations.Add(newRelationship);
            await _dbContext.SaveChangesAsync();
            return this.Protocol(ErrorType.Success, $"您成功加入了这个群聊: {groupName}!");
        }

        [HttpPost]
        public async Task<IActionResult> LeaveGroup([Required]string groupName)
        {
            var user = await _userManager.GetUserAsync(User);
            var group = await _dbContext.GroupConversations.SingleOrDefaultAsync(t => t.GroupName == groupName);
            if (group == null)
            {
                return this.Protocol(ErrorType.NotFound, $"无法找到对应的群聊名称: {groupName}!");
            }
            var joined = await _dbContext.GetRelationFromGroup(user.Id, group.Id);
            if (joined == null)
            {
                return this.Protocol(ErrorType.HasDoneAlready, $"您已经加入了这个群了: {groupName}!");
            }
            _dbContext.UserGroupRelations.Remove(joined);
            await _dbContext.SaveChangesAsync();

            var any = _dbContext.UserGroupRelations.Any(t => t.GroupId == group.Id);
            if (!any)
            {
                _dbContext.GroupConversations.Remove(group);
                await _dbContext.SaveChangesAsync();
            }
            return this.Protocol(ErrorType.Success, $"您成功的退出了这个群聊: {groupName}!");
        }

        [HttpPost]
        public async Task<IActionResult> SetGroupMuted([Required]string groupName, [Required]bool setMuted)
        {
            var user = await _userManager.GetUserAsync(User);
            var group = await _dbContext.GroupConversations.SingleOrDefaultAsync(t => t.GroupName == groupName);
            if (group == null)
            {
                return this.Protocol(ErrorType.NotFound, $"无法找到这个群聊: {groupName}!");
            }
            var joined = await _dbContext.GetRelationFromGroup(user.Id, group.Id);
            if (joined == null)
            {
                return this.Protocol(ErrorType.Unauthorized, $"您没有加入这个群聊: {groupName}!");
            }
            if (joined.Muted == setMuted)
            {
                return this.Protocol(ErrorType.HasDoneAlready, $"You have already {(joined.Muted ? "muted" : "unmuted")} the group: {groupName}!");
            }
            joined.Muted = setMuted;
            await _dbContext.SaveChangesAsync();
            return this.Protocol(ErrorType.Success, $"成功设置 {(setMuted ? "muted" : "unmuted")} 给群聊 '{groupName}'!");
        }
    }
}