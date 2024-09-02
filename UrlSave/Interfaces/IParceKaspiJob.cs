namespace UrlSave.Interfaces
{
    public interface IParceKaspiJob
    {
        Task ParseKaspiLinks();
        Task ParcerCode(Link link);
        Task AddNewPrice(long price, Product product);

    }
}
