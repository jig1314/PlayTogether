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
                                            .Where(c => c.Users.Count == 2
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

        [HttpGet("directMessageConversations")]
        public async Task<ActionResult<IEnumerable<DirectMessageConversation>>> GetDirectMessageConversations(string withWhoId)
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
                                            .Where(c => c.Users.Count == 2 && c.Users.Select(c => c.ApplicationUserId).Contains(idUser))
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
