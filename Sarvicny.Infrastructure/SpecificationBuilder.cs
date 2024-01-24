using Microsoft.EntityFrameworkCore;
using Sarvicny.Domain.Specification;

namespace Sarvicny.Infrastructure;

public static class SpecificationBuilder<T> where T : class
{
    //DbSet , Specification ( where , includes )
    public static IQueryable<T> Build(IQueryable<T> source, ISpecifications<T> spec)
    {
        var query = source;

        if (spec.Criteria is not null)
        {
            query = query.Where(spec.Criteria);
        }


        query = spec.Includes.Aggregate(query,
            (currentQuery, includeExpressions) => currentQuery.Include(includeExpressions));

        return query;
    }
}