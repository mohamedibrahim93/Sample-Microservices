using Shopping.Aggregator.Models;

namespace Shopping.Aggregator.Services.Contracts
{
    public interface ICatalogService
    {
        Task<IEnumerable<CatalogModel>> GetCatalog();

        Task<IEnumerable<CatalogModel>> GetCatalogByCategory(string category);

        Task<CatalogModel> GetCatalog(string id);
    }
}