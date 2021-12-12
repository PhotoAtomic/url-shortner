using Microsoft.Azure.Cosmos;
using Newtonsoft.Json;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace api.Persistence.Cosmos
{
    public class CosmosUnitOfWork : IUnitOfWork
    {
        private Database database;
        private readonly int? througput;

        public CosmosUnitOfWork(Database database, int? througput = null)
        {
            this.database = database;
            this.througput = througput;
        }

        public async Task<IRepository<T>> RepositoryOf<T>() where T : IIdentifiable
        {

            var jsonPropertyAttribute = typeof(IIdentifiable).GetProperty(nameof(IIdentifiable.Id))?.GetCustomAttribute<JsonPropertyAttribute>(true);
            var idName = jsonPropertyAttribute?.PropertyName ?? nameof(IIdentifiable.Id);

            Container container = await database.CreateContainerIfNotExistsAsync(
            typeof(T).Name,
            $"/{idName}",
            this.througput);
            return new CosmosRepository<T>(container, this);

        }

        private ConditionalWeakTable<object, string> eTags = new();


        internal bool TryGetEtag<T>(T item, out string? eTag) where T : IIdentifiable
        {
            return eTags.TryGetValue(item, out eTag);
        }

        internal void TrackETag<T>(T resource, string eTag) where T : IIdentifiable
        {
            eTags.AddOrUpdate(resource, eTag);
        }

        public void Dispose()
        {

        }


    }
}