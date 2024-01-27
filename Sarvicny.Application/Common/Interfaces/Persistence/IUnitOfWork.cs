namespace Sarvicny.Application.Common.Interfaces.Persistence;

public interface IUnitOfWork : IDisposable
{
    void Commit();
    void Rollback();


}