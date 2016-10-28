using System;
using System.Data.Entity;
using System.Linq;

namespace Utilities
{
    public static class EntityExtensions
    {
        public static IQueryable<TEntity> IncludeMany<TEntity>(this IQueryable<TEntity> builder, params String[] entityNames) where TEntity : class
        {
            if (entityNames == null || !entityNames.Any())
            {
                return builder;
            }

            foreach (var entityName in entityNames)
            {
                builder = builder.Include(entityName);
            }
            return builder;
        }
    }
}
