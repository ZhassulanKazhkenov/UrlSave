namespace UrlSave.Domain.Entities;

public class Link : BaseEntity
{
    public Link(string url, int userId, int? productId)
    {
        Url = url;
        UserId = userId;
        ProductId = productId;
    }

    public string Url { get; set; }

    public int UserId { get; set; }
    public virtual User User { get; set; }

    public int? ProductId { get; set; }
    public virtual Product Product { get; set; }
}
