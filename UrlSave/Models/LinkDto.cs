using UrlSave.Entities;

namespace UrlSave.Models
{
    public class LinkDto
    {
        public string Url { get; set; }
    }
    public static class LinkExtension
    {
        public static Link ToLink(this LinkDto linkDto)
        {
            return new Link { Url = linkDto.Url };
        }
    } 
}
