namespace UrlSave.Entities
{
    public class Notification : BaseEntity
    {
        public int LinkId {  get; set; }
        public string Title {  get; set; }
        public string Body { get; set; }
        public string Recipient { get; set; }
        public bool IsSend { get; set; }
        public virtual Link Link { get; set; }
    }
}
