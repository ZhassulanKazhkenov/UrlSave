namespace UrlSave.Domain.Entities;

public class Price : BaseEntity
{
    public long Value { get; set; }

    public int ProductId { get; set; }
    public virtual Product Product { get; set; }
}
