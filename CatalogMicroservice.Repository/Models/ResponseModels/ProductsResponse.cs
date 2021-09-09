using CatalogMicroservice.Domain.Entities;
using System.Collections.Generic;

namespace CatalogMicroservice.Repository.Models.ResponseModels
{
    //Do not use for categories!!!
    public class ProductsResponse<TEntity> where TEntity : BaseEntity
    {
        public IEnumerable<TEntity> Products { get; set; }

        public int TotalPages { get; set; }
    }
}