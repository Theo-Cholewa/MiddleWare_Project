using System;
using System.Net.Http;
using System.Threading.Tasks;
using System.Runtime.Caching;

namespace ProxyCache
{
    public class ProxyCache : IProxyCache
    {
        private GenericProxyCache<string> _cache = new GenericProxyCache<string>();

        public string GetData(int value)
        {
            return string.Format("You entered: {0}", value);
        }

        public string CallApi(string url)
        {
            return _cache.Get(url, async () => await CallApiIntern(url));
        }

        private async Task<string> CallApiIntern(string url)
        {
            HttpClient client = new HttpClient();
            HttpResponseMessage response = await client.GetAsync(url);
            return await response.Content.ReadAsStringAsync();
        }
    }
}
