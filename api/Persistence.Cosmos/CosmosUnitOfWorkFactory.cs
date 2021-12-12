using api.Configurations;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Options;

namespace api.Persistence.Cosmos
{
    public class CosmosUnitOfWorkFactory : IUnitOfWorkFactory
    {

        private readonly PersistenceConfiguration configuration;

        public CosmosUnitOfWorkFactory(IOptions<PersistenceConfiguration> configOption)
        {
            configuration = configOption.Value;
        }


        private Database? database;

        private async Task<Database> GetDataBase()
        {
            if (database is null)
            {
                CosmosClient client = new CosmosClient(configuration.EndPoint, configuration.Key);
                database = await client.CreateDatabaseIfNotExistsAsync(configuration.DBName);
            }
            return database;
        }


        public async Task<IUnitOfWork> NewUnitOfWork()
        {
            return new CosmosUnitOfWork(await GetDataBase(), configuration.Througput);
        }

       
    }
}
