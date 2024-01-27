using Sarvicny.Application.Common.Interfaces.Persistence;
using Sarvicny.Infrastructure.Data;

namespace Sarvicny.Infrastructure.Persistence;

public class UnitOfWork : IUnitOfWork
{
    private readonly AppDbContext _context;




    public UnitOfWork(AppDbContext context)
    {
        _context = context;

    }
    public void Commit()
    {
        _context.SaveChanges();
    }

    public void Rollback()
    {
        throw new NotImplementedException();
    }

    public void Dispose()
    {
        _context.Dispose();
    }

}