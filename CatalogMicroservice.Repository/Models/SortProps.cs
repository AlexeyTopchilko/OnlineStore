using CatalogMicroservice.Domain.Entities;
using CatalogMicroservice.Repository.Models.Enums;
using System;
using System.Linq.Expressions;

namespace CatalogMicroservice.Repository.Models
{
    public class SortProps<TEntity, TKey> where TEntity : BaseEntity
    {
        public SortDirection Direction { get; set; }

        public Expression<Func<TEntity, TKey>> Expression { get; set; }

        public int Take { get; set; }

        public int Skip { get; set; }
    }
}