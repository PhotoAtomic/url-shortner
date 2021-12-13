using Microsoft.Azure.Cosmos;
using System.Net;

namespace api.Persistence.Cosmos
{
    internal class CosmosRepository<T> : IRepository<T> where T:IIdentifiable
    {
        private Container container;
        private readonly CosmosUnitOfWork uow;
        private readonly PartitionKey idPartitionKey;

        internal CosmosRepository(Container container, CosmosUnitOfWork uow)
        {
            this.container = container;
            this.uow = uow;
            idPartitionKey = new PartitionKey(nameof(IIdentifiable.Id));
        }

        public async Task Add(T item, bool mustNotExists = false)
        {

            if (item is null) throw new ArgumentNullException(nameof(item));
            if (string.IsNullOrEmpty(item.Id)) throw new ArgumentException($"The provided item has an empty {nameof(IIdentifiable.Id)}");
            ItemRequestOptions? requestOptions = null;

            if (uow.TryGetEtag(item,out var eTag)) {
                requestOptions  = new ItemRequestOptions { IfMatchEtag = eTag };                
            }

            try
            {
                ItemResponse<T> response;
                if (mustNotExists)
                {
                    response = await container.CreateItemAsync<T>(item, GetPartitionKeyForItem(item));
                }
                else
                {
                    response = await container.UpsertItemAsync<T>(item, GetPartitionKeyForItem(item), requestOptions: requestOptions);
                }

                if(response.StatusCode == HttpStatusCode.Created)
                {
                    uow.TrackETag(item, response.ETag);
                }
            }
            catch (CosmosException ex) when (ex.StatusCode == HttpStatusCode.PreconditionFailed)
            {
                throw new ConcurrencyException($"Updating item with id {item.Id} failed because of a concurrent update");
            }
            catch (CosmosException ex) when (ex.StatusCode == HttpStatusCode.Conflict)
            {
                throw new ConcurrencyException($"Creating item with id {item.Id} failed because already exists");
            }

        }




        public async Task<T?> GetById(string id)
        {
            try
            {
                var response = await container.ReadItemAsync<T>(id, GetPartitionKeyForId(id));

                if (response.StatusCode == HttpStatusCode.NotFound) return default;
                if (response.StatusCode != HttpStatusCode.OK) throw new Exception($"Retriving document {id} returned with error {Enum.GetName<HttpStatusCode>(response.StatusCode)}");

                uow.TrackETag(response.Resource, response.ETag);

                return response.Resource;                
            }
            catch (CosmosException ex)
            {
                if(ex.StatusCode == HttpStatusCode.NotFound) return default;
                throw new Exception($"Retriving document {id} returned with error {Enum.GetName<HttpStatusCode>(ex.StatusCode)}", ex);
            }
        }


        public Task RemoveById(string id)
        {
            if (string.IsNullOrEmpty(id)) throw new Exception("Id can't be null or empty for deletion operation");
            return container.DeleteItemAsync<T>(id, GetPartitionKeyForId(id));
        }

        private PartitionKey? GetPartitionKeyForItem(T item)
        {
            if (item is null || item.Id is null) return null;
            return GetPartitionKeyForId(item.Id);
        }

        private PartitionKey GetPartitionKeyForId(string id)
        {
            return new PartitionKey(id);
        }
    }
}