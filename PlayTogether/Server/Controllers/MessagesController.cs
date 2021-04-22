using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PlayTogether.Server.Data;
using PlayTogether.Server.Models;
using PlayTogether.Shared.DTOs;
using PlayTogether.Shared.Models;

namespace PlayTogether.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MessagesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public MessagesController(ApplicationDbContext context)
        {
            _context = context;
        }

        private string GetUserId()
        {
            return HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
        }

        // GET: api/Messages/5
        [HttpGet("{withWhoId}")]
        public async Task<ActionResult<IEnumerable<MessageDto>>> GetMessages(string withWhoId)
        {
            try
            {
                if (!HttpContext.User.Identity.IsAuthenticated)
                {
                    return StatusCode(StatusCodes.Status401Unauthorized, "Error retrieving messages");
                }

                var idUser = GetUserId();
                var existingConversation = await _context.Conversations
                                            .Include(c => c.Users)
                                            .Include(c => c.Messages)
                                            .ThenInclude(m => m.FromUser)
                                            .ThenInclude(u => u.ApplicationUserDetails)
                                            .Where(c => c.Users.Count == 2 && string.IsNullOrWhiteSpace(c.Name)
                                                && c.Users.Select(c => c.ApplicationUserId).Contains(idUser)
                                                && c.Users.Select(c => c.ApplicationUserId).Contains(withWhoId)).SingleOrDefaultAsync();

                if (existingConversation == null)
                    return Ok(new List<MessageDto>());
                else
                {
                    var messages = existingConversation.Messages.Select(m => new MessageDto()
                    {
                        ConversationId = m.ConversationId,
                        MessageText = m.MessageText,
                        DateSubmitted = m.DateSubmitted,
                        FromUserId = m.FromUserId,
                        FromUser = new UserBasicInfo()
                        {
                            UserId = m.FromUserId,
                            FirstName = m.FromUser.ApplicationUserDetails.FirstName,
                            LastName = m.FromUser.ApplicationUserDetails.LastName,
                            Email = m.FromUser.Email,
                            UserName = m.FromUser.UserName
                        }
                    });

                    return Ok(messages);
                }
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Error retrieving messages");
            }
        }

        [HttpGet("groupMessages/{conversationName}")]
        public async Task<ActionResult<IEnumerable<MessageDto>>> GetGroupMessages(string conversationName)
        {
            try
            {
                if (!HttpContext.User.Identity.IsAuthenticated)
                {
                    return StatusCode(StatusCodes.Status401Unauthorized, "Error retrieving messages");
                }

                var idUser = GetUserId();
                var existingConversation = await _context.Conversations
                                            .Include(c => c.Users)
                                            .Include(c => c.Messages)
                                            .ThenInclude(m => m.FromUser)
                                            .ThenInclude(u => u.ApplicationUserDetails)
                                            .Where(c => c.Name == conversationName).SingleOrDefaultAsync();

                if (!existingConversation.Users.Any(u => u.ApplicationUserId == idUser))
                {
                    return StatusCode(StatusCodes.Status401Unauthorized, "Sorry but you are not in the group!");
                }

                var messages = existingConversation.Messages.Select(m => new MessageDto()
                {
                    ConversationId = m.ConversationId,
                    MessageText = m.MessageText,
                    DateSubmitted = m.DateSubmitted,
                    FromUserId = m.FromUserId,
                    FromUser = new UserBasicInfo()
                    {
                        UserId = m.FromUserId,
                        FirstName = m.FromUser.ApplicationUserDetails.FirstName,
                        LastName = m.FromUser.ApplicationUserDetails.LastName,
                        Email = m.FromUser.Email,
                        UserName = m.FromUser.UserName
                    }
                });

                return Ok(messages);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, (ex.InnerException != null) ? ex.InnerException.Message : ex.Message);
            }
        }

        [HttpGet("directMessageConversations")]
        public async Task<ActionResult<IEnumerable<DirectMessageConversation>>> GetDirectMessageConversations()
        {
            try
            {
                if (!HttpContext.User.Identity.IsAuthenticated)
                {
                    return StatusCode(StatusCodes.Status401Unauthorized, "Error retrieving conversations");
                }

                var idUser = GetUserId();
                var conversations = await _context.Conversations
                                            .Include(c => c.Users)
                                            .ThenInclude(u => u.ApplicationUser)
                                            .ThenInclude(u => u.ApplicationUserDetails)
                                            .Where(c => c.Users.Count == 2 && string.IsNullOrWhiteSpace(c.Name) && c.Users.Select(c => c.ApplicationUserId).Contains(idUser))
                                            .ToListAsync();

                var directMessageConversations = conversations.Select(c =>
                {
                    var user = c.Users.SingleOrDefault(user => user.ApplicationUserId == idUser);
                    var otherUser = c.Users.SingleOrDefault(user => user.ApplicationUserId != idUser);
                    return new DirectMessageConversation()
                    {
                        Id = c.Id,
                        WithUser = new UserBasicInfo()
                        {
                            UserId = otherUser.ApplicationUserId,
                            FirstName = otherUser.ApplicationUser.ApplicationUserDetails.FirstName,
                            LastName = otherUser.ApplicationUser.ApplicationUserDetails.LastName,
                            Email = otherUser.ApplicationUser.Email,
                            UserName = otherUser.ApplicationUser.UserName
                        },
                        HasUnreadMessages = user.HasUnreadMessages
                    };
                });

                return Ok(directMessageConversations);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Error retrieving conversations");
            }
        }

        [HttpGet("directMessageConversation/{withWhoId}")]
        public async Task<ActionResult<DirectMessageConversation>> GetDirectMessageConversation(string withWhoId)
        {
            try
            {
                if (!HttpContext.User.Identity.IsAuthenticated)
                {
                    return StatusCode(StatusCodes.Status401Unauthorized, "Error retrieving conversations");
                }

                var idUser = GetUserId();
                var conversation = await _context.Conversations
                                            .Include(c => c.Users)
                                            .ThenInclude(u => u.ApplicationUser)
                                            .ThenInclude(u => u.ApplicationUserDetails)
                                            .Where(c => c.Users.Count == 2 
                                                && string.IsNullOrWhiteSpace(c.Name) 
                                                && c.Users.Select(c => c.ApplicationUserId).Contains(idUser) 
                                                && c.Users.Select(c => c.ApplicationUserId).Contains(withWhoId))
                                            .SingleOrDefaultAsync();

                var user = conversation.Users.SingleOrDefault(user => user.ApplicationUserId == idUser);
                var otherUser = conversation.Users.SingleOrDefault(user => user.ApplicationUserId == withWhoId);
                var directMessageConversation = new DirectMessageConversation()
                {
                    Id = conversation.Id,
                    WithUser = new UserBasicInfo()
                    {
                        UserId = otherUser.ApplicationUserId,
                        FirstName = otherUser.ApplicationUser.ApplicationUserDetails.FirstName,
                        LastName = otherUser.ApplicationUser.ApplicationUserDetails.LastName,
                        Email = otherUser.ApplicationUser.Email,
                        UserName = otherUser.ApplicationUser.UserName
                    },
                    HasUnreadMessages = user.HasUnreadMessages
                };
                return Ok(directMessageConversation);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Error retrieving conversations");
            }
        }

        [HttpGet("chatGroupConversation/{conversationName}")]
        public async Task<ActionResult<ChatGroupConversation>> GetChatGroupConversations(string conversationName)
        {
            try
            {
                if (!HttpContext.User.Identity.IsAuthenticated)
                {
                    return StatusCode(StatusCodes.Status401Unauthorized, "Error retrieving conversations");
                }

                var idUser = GetUserId();
                var conversation = await _context.Conversations
                                            .Include(c => c.Users)
                                            .ThenInclude(u => u.ApplicationUser)
                                            .ThenInclude(u => u.ApplicationUserDetails)
                                            .SingleOrDefaultAsync(c => c.Name == conversationName);

                var chatGroupConversation = new ChatGroupConversation()
                {
                    Id = conversation.Id,
                    Name = conversation.Name,
                    CreatedByUserId = conversation.CreatedByUserId,
                    WithUsers = conversation.Users.Select(u => new UserBasicInfo()
                    {
                        UserId = u.ApplicationUserId,
                        FirstName = u.ApplicationUser.ApplicationUserDetails.FirstName,
                        LastName = u.ApplicationUser.ApplicationUserDetails.LastName,
                        Email = u.ApplicationUser.Email,
                        UserName = u.ApplicationUser.UserName
                    }).ToList(),
                    HasUnreadMessages = conversation.Users.SingleOrDefault(user => user.ApplicationUserId == idUser).HasUnreadMessages
                };

                return Ok(chatGroupConversation);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Error retrieving conversations");
            }
        }

        [HttpGet("chatGroupConversations")]
        public async Task<ActionResult<IEnumerable<ChatGroupConversation>>> GetChatGroupConversations()
        {
            try
            {
                if (!HttpContext.User.Identity.IsAuthenticated)
                {
                    return StatusCode(StatusCodes.Status401Unauthorized, "Error retrieving conversations");
                }

                var idUser = GetUserId();
                var conversations = await _context.Conversations
                                            .Include(c => c.Users)
                                            .Where(c => !string.IsNullOrWhiteSpace(c.Name) && c.Users.Select(c => c.ApplicationUserId).Contains(idUser))
                                            .ToListAsync();

                var chatGroupConversations = conversations.Select(c =>
                {
                    var user = c.Users.SingleOrDefault(user => user.ApplicationUserId == idUser);
                    return new ChatGroupConversation()
                    {
                        Id = c.Id,
                        Name = c.Name,
                        CreatedByUserId = c.CreatedByUserId,
                        HasUnreadMessages = user.HasUnreadMessages
                    };
                });

                return Ok(chatGroupConversations);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Error retrieving conversations");
            }
        }

        [HttpPost("createChatGroup")]
        public async Task<ActionResult> PostChatGroup(ChatGroupDto chatGroupDto)
        {
            try
            {
                if (!HttpContext.User.Identity.IsAuthenticated)
                {
                    return StatusCode(StatusCodes.Status401Unauthorized, "You are not authorized to create a chat group!");
                }

                if (_context.Conversations.Any(c => c.Name == chatGroupDto.GroupName))
                {
                    throw new Exception("There already exists a chat group with this name!");
                }

                var idUser = GetUserId();
                var groupGuid = Guid.NewGuid().ToString();

                var chatGroup = new Conversation()
                {
                    Id = groupGuid,
                    Name = chatGroupDto.GroupName,
                    CreatedByUserId = idUser
                };

                _context.Conversations.Add(chatGroup);
                await _context.SaveChangesAsync();

                var userConversation = new ApplicationUser_Conversation()
                {
                    ApplicationUserId = idUser,
                    ConversationId = groupGuid
                };

                _context.ApplicationUser_Conversations.Add(userConversation);
                await _context.SaveChangesAsync();

                return StatusCode(StatusCodes.Status202Accepted);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, (ex.InnerException != null) ? ex.InnerException.Message : ex.Message);
            }
        }

        [HttpPut("updateChatGroup")]
        public async Task<ActionResult> PutChatGroup(ChatGroupDto chatGroupDto)
        {
            try
            {
                if (!HttpContext.User.Identity.IsAuthenticated)
                {
                    return StatusCode(StatusCodes.Status401Unauthorized, "You are not authorized to create a chat group!");
                }

                if (_context.Conversations.Any(c => c.Name == chatGroupDto.GroupName))
                {
                    throw new Exception("There already exists a chat group with this name!");
                }

                var idUser = GetUserId();
                var existingGroup = await _context.Conversations.SingleOrDefaultAsync(c => c.Id == chatGroupDto.ConversationId);

                existingGroup.Name = chatGroupDto.GroupName;
                _context.Entry(existingGroup).State = EntityState.Modified;
                await _context.SaveChangesAsync();

                return StatusCode(StatusCodes.Status202Accepted);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, (ex.InnerException != null) ? ex.InnerException.Message : ex.Message);
            }
        }

        // PUT: api/Messages/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutMessage(long id, Message message)
        {
            if (id != message.Id)
            {
                return BadRequest();
            }

            _context.Entry(message).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!MessageExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Messages
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Message>> PostMessage(Message message)
        {
            _context.Messages.Add(message);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetMessage", new { id = message.Id }, message);
        }

        // DELETE: api/Messages/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteMessage(long id)
        {
            var message = await _context.Messages.FindAsync(id);
            if (message == null)
            {
                return NotFound();
            }

            _context.Messages.Remove(message);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool MessageExists(long id)
        {
            return _context.Messages.Any(e => e.Id == id);
        }
    }
}
