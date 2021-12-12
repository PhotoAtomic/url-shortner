namespace api.Persistence
{
    public interface IUnitOfWorkFactory
    {
        Task<IUnitOfWork> NewUnitOfWork();

    }
}
