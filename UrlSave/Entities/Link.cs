namespace UrlSave.Entities
{
    public class Link : BaseEntity
    {
        public int UserId { get; set; }
        public string Url { get; set; }
        public virtual User User { get; set; }

    }
}
