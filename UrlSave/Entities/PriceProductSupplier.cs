namespace UrlSave.Entities
{
    public class PriceProductSupplier : BaseEntity
    {
        public int PriceId { get; set; }
        public int ProductSupplierId { get; set; }
        public virtual Price Price { get; set; }
        public virtual ProductSupplier ProductSupplier { get; set; }
    }
}
