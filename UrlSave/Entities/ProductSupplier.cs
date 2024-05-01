namespace UrlSave.Entities
{
    public class ProductSupplier : BaseEntity
    {
        public int ProductId {  get; set; }
        public int SupplierId { get; set; }
        public virtual Product Product { get; set; }
        public virtual Supplier Supplier { get; set; }
    }
}
