using System.ComponentModel.DataAnnotations;

namespace UrlSave.Entities
{
    public class Supplier : BaseEntity
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }
}
