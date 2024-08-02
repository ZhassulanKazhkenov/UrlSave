namespace UrlSave.Entities;

public class Notification : BaseEntity
{
    public string Title {  get; set; }
    public string Body { get; set; }
    public string Recipient { get; set; }
    public bool IsSend { get; set; }
    
    public int LinkId { get; set; }
    public virtual Link Link { get; set; }

    public int PriceId { get; set; }
    public virtual Price Price { get; set; }
}
