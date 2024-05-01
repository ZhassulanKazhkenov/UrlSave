using System.ComponentModel.DataAnnotations;
using UrlSave.Entities;

namespace UrlSave.Models
{
    public class LinkDto
    {
        [Required]
        public string Url { get; set; }

        [Required]
        public string Email { get; set; }
    }
}
