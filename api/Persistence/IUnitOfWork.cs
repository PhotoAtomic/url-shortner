namespace api.Persistence
{
    public interface IUnitOfWork : IDisposable
    {
        Task<IRepository<T>> RepositoryOf<T>() where T : IIdentifiable;
    }
}
