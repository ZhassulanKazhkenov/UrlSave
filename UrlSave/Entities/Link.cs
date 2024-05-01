namespace UrlSave.Entities
{
    public class Link : BaseEntity
    {
        public string Url { get; set; }

        public int UserId { get; set; }
        public virtual User User { get; set; }

        public int? ProductId { get; set; }
        public virtual Product Product { get; set; }
    }
}
