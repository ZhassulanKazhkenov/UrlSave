using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net.Mime;
using UrlSave.Contexts;
using UrlSave.Entities;
using UrlSave.Models;

namespace UrlSave.Controllers.v1
{
    [ApiController]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiVersion("1.0")]
    [Produces("application/json")]
    public class LinkController : ControllerBase
    {
        private readonly LinkContext _context;

        public LinkController(LinkContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Post a specific.
        /// </summary>
        /// <response code="204">Empty</response>
        /// <response code="400">If the item is null</response>
        [HttpPost("create")]
        [Consumes(MediaTypeNames.Application.Json)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult> PostLink([FromBody] LinkDto model)
        {
            if (!Uri.IsWellFormedUriString(model.Url, UriKind.Absolute))
            {
                return BadRequest();
            }
            //var users = await _context.Users.ToListAsync();
            //var userr = users.FirstOrDefault(x => x.Email == model.Email);
            var user = await _context.Users.FirstOrDefaultAsync(x => x.Email == model.Email);
            int userId = 0;

            if (user == null)
            {
                var newUser = new User { Email = model.Email };
                await _context.Users.AddAsync(newUser);
                await _context.SaveChangesAsync();
                userId = newUser.Id;
            } else 
            {
                userId = user.Id;
            }

            var link = new Link 
            {
                Url = model.Url,
                UserId = userId,
                ProductId = null
            };

            var uri = new Uri(link.Url);
            var host = uri.Host.ToLower(); 
            if (!host.EndsWith("kaspi.kz"))
            {
                return BadRequest("Invalid domain. Only kaspi.kz URLs are allowed.");
            }
            link.CreatedDate = DateTime.Now;
            _context.Links.Add(link);
            await _context.SaveChangesAsync();
            return NoContent();
        }
        /// <summary>
        /// Delete a specific.
        /// </summary>
        /// <response code="204">Empty</response>
        /// <response code="400">If the item is null</response>
        [HttpDelete("id/{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult> DeleteLink([FromRoute] int id)
        {
            var link = await _context.Links.FindAsync(id);
            if (link == null)
            {
                return NotFound();
            }

            _context.Links.Remove(link);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool LinkExists(int id)
        {
            return _context.Links.Any(e => e.Id == id);
        }

    }
}