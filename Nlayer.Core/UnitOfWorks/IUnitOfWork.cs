namespace Nlayer.Core.UnitOfWorks
{
    public interface IUnitOfWork
    {
        Task CommitAsync();
        void Commit();
    }
}
