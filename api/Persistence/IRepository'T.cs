namespace api.Persistence
{
    public interface IRepository<T>
    {
        Task<T?> GetById(string id);

        Task RemoveById(string id);

        Task Add(T item, bool mustNotExists = false);

        
    }
}
