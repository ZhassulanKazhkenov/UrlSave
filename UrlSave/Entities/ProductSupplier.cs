namespace UrlSave.Entities
{
    public class ProductSupplier
    {
        public int Id { get; set; }
        public int ProductId {  get; set; }
        public int SupplierId { get; set; }
        public virtual Product Product { get; set; }
        public virtual Supplier Supplier { get; set; }
    }
}
