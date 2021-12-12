using api.Domain;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;

namespace api.Cache
{
    public class ShortUrlCache
    {
        private readonly MemoryCache cache;
        private readonly MemoryCacheEntryOptions itemOptions;

        public ShortUrlCache(IOptions<MemoryCacheOptions> options)
        {
            cache = new MemoryCache(options);
            itemOptions = new MemoryCacheEntryOptions().SetSize(1);
            //cache options can detailed more to specifiy the time to live and other features
        }


        public void Store(ShortUrl? shortUrl)
        {
            if (shortUrl is null) return;
            cache.Set<ShortUrl>(shortUrl.Id, shortUrl, itemOptions);
        }

        public ShortUrl? Get(string slug)
        {
            if (slug is null) return null;
            cache.TryGetValue<ShortUrl>(slug, out var shortUrl);
            return shortUrl;
        }
    }
}
