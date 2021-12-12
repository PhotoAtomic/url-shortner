using api.Configurations;
using api.Domain;
using api.Persistence;
using api.Utilities;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Options;
using Polly;
using Polly.Retry;

namespace api.Services
{    

    public class UrlShorteningService
    {
        readonly string sequenceName = "0";
        private readonly ulong chunkSize;
        private readonly AsyncRetryPolicy chunkRetrivalPolicy;
        private readonly AsyncRetryPolicy urlSavePolicy;
        readonly IUnitOfWorkFactory database;

        public UrlShorteningService(IUnitOfWorkFactory database, IOptions <UrlShorteningServiceConfiguration> configOptions)
        {
            this.database = database;
            this.sequenceName = configOptions.Value.SequenceName.ToLower(); // sequence name, as they become part of the shortened url, are accepted only in lowercase otherwise the user have to input the correct case for the prefix
            this.chunkSize = configOptions.Value.ChunkSize;

            chunkRetrivalPolicy = Policy.Handle<ConcurrencyException>()
                .WaitAndRetryAsync(retryCount: 10, sleepDurationProvider: (attemptCount) => TimeSpan.FromMilliseconds(attemptCount * 2));

            urlSavePolicy = Policy.Handle<ConcurrencyException>()
                .WaitAndRetryAsync(retryCount: 2, sleepDurationProvider: (attemptCount) => TimeSpan.FromMilliseconds(attemptCount * 2));

        }

        
        private ulong sequenceValue = 0;
        private ulong sequenceChunkLimit = 0;

        private object synchRoot = new();
        private TaskCompletionSource? advancementTaskCompletionSource = null;


        private Task ObtainNextChunk()
        {
            return chunkRetrivalPolicy.ExecuteAsync(async () =>
            {

                using (var uow = await database.NewUnitOfWork())
                {
                    var sequenceRepository = await uow.RepositoryOf<Sequence>();
                    var sequence = await sequenceRepository.GetById(sequenceName);

                    ulong prevLimit = 0;

                    if (sequence is not null)
                    {
                        prevLimit = sequence.Limit;
                        sequence.Limit += chunkSize;
                    }
                    else
                    {
                        sequence = new Sequence() { Id = sequenceName, Limit = chunkSize }; // in order to ensure correct concurrency when more than one server is action on the same sequence, the sequence must be bootstapped first, this code is here just for convenience so to not manually botstap the sequence on the db!
                    }

                    await sequenceRepository.Add(sequence);

                    sequenceValue = prevLimit;
                    sequenceChunkLimit = sequence.Limit;

                }
            });
        }


        private async Task AdvanceToNextSequenceChunk()
        {
            bool inCharge = false;
            lock (synchRoot) // enqueue all the possible concurrent requests after the same update talk
            {
                if(advancementTaskCompletionSource is null || advancementTaskCompletionSource.Task.IsCompleted)
                {
                    advancementTaskCompletionSource = new TaskCompletionSource();
                    inCharge = true;                    
                }
            }


            if (inCharge)
            {
                await ObtainNextChunk();
                advancementTaskCompletionSource.SetResult();
            }
            else
            {
                await advancementTaskCompletionSource.Task;
            }
        }


        public async Task<ulong> GetSequenceNextValue()
        {
          

            do
            {
                var currentValue = Interlocked.Read(ref sequenceValue);
                if (currentValue == 0) await AdvanceToNextSequenceChunk();


                var nextValue = Interlocked.Add(ref sequenceValue, 1);
                var limit = Interlocked.Read(ref sequenceChunkLimit);

                if (nextValue == 0) // Add have wrapped around the ulong number space
                {
                    Interlocked.Exchange(ref nextValue, ulong.MaxValue);
                    throw new OverflowException("unable to generate more ids"); // this is when all the uint have been consumed
                }

                if (nextValue > limit) // the chunk is depleted, take the next
                {
                    await AdvanceToNextSequenceChunk();
                    continue;
                }
                return nextValue;
            }
            while (true);
        }



        public async Task<string> CreateShortSlugFor(string url)
        {
            if (!IsValid(url)) throw new Exception($"the provided url is invalid");

            var id = await GetSequenceNextValue(); // at this poit there should not be the risk that any other worker, nor any other thread in this worker have obtained the same id

            var slug = UlongEncoding.ToSpecialBase16(id, sequenceName);

            await StoreShortSlugForUrl(slug, url);

            return slug;

        }

        protected virtual bool IsValid(string url)
        {
            return true; // no assumptions here.... i will have checked that the URI brings to an existing page prior to generate a short uri but, maybe someone wants to shorten a intranet uri? or a malformed one for testing purposes? why not.
        }

        private Task StoreShortSlugForUrl(string slug, string url)
        {
            return chunkRetrivalPolicy.ExecuteAsync(async () =>
            {
                using (var uow = await database.NewUnitOfWork())
                {
                    var shortUrlRepository = await uow.RepositoryOf<ShortUrl>();
                    ShortUrl shortUrl = new()
                    {
                        Id = slug,
                        Url = url,
                    };
                    await shortUrlRepository.Add(shortUrl);
                }
            });
        }
         
    }
}
