using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using UrlSave;

namespace UrlSave
{
    [ApiController]
    [Route("[controller]")]
    public class LinkController : ControllerBase
    {
        private readonly LinkContext _context;

        public LinkController(LinkContext context)
        {
            _context = context;
        }

        public object GetLink { get; private set; }

        [HttpPost]
        [Route("create")]
        public async Task<ActionResult> PostLink([FromBody]Link link)
        {
            link.CreatedDate= DateTime.Now;
            _context.Links.Add(link);
            await _context.SaveChangesAsync();

            return Ok();
        }

        private bool LinkExists(int id)
        {
            return _context.Links.Any(e => e.Id == id);
        }
    }
}