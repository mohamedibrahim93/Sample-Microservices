﻿using Shopping.Web.Models;

namespace Shopping.Web.Services.Contracts
{
    public interface ICatalogService
    {
        Task<IEnumerable<CatalogModel>> GetCatalog();

        Task<IEnumerable<CatalogModel>> GetCatalogByCategory(string category);

        Task<CatalogModel> GetCatalog(string id);

        Task<CatalogModel> CreateCatalog(CatalogModel model);
    }
}