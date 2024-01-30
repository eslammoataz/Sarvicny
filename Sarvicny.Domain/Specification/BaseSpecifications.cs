using System.Linq.Expressions;

namespace Sarvicny.Domain.Specification;

public class BaseSpecifications<T> : ISpecifications<T> where T : class
{
    public Expression<Func<T, bool>> Criteria { get; set; }  // represents the where clause
    public List<Expression<Func<T, object>>> Includes { get; set; } = new List<Expression<Func<T, object>>>();
    public List<string> IncludeStrings { get; set; } = new List<string>();

    public BaseSpecifications() // when not having any criteria aka where clause
    {

    }

    public BaseSpecifications(Expression<Func<T, bool>> criteriaExpression)
    {
        Criteria = criteriaExpression;
    }
    protected virtual void AddInclude(Expression<Func<T, object>> includeExpression)
    {
        Includes.Add(includeExpression);
    }
    
    protected virtual void AddInclude(string includeString)
    {
        IncludeStrings.Add(includeString);
    }

}