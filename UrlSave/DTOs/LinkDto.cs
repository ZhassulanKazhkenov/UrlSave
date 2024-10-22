using System.ComponentModel.DataAnnotations;

namespace UrlSave.DTOs;

public class LinkDto
{
    [Required]
    public string Url { get; set; }

    [Required]
    public string Email { get; set; }
}
