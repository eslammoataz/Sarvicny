using System.Linq.Expressions;

namespace Sarvicny.Domain.Specification;

public interface ISpecifications<T> where T : class
{
    public Expression<Func<T, bool>> Criteria { get; set; }
    public List<Expression<Func<T, object>>> Includes { get; set; }
   

    // Expression<Func<T, object>> OrderBy { get; }
    // Expression<Func<T, object>> OrderByDescending { get; }
    // int Take { get; }
    // int Skip { get; }
    // bool IsPagingEnabled { get; }
    // List<string> IncludeStrings { get; }



}