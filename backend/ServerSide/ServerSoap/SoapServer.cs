using System.Net.Http;
using System.Threading.Tasks;

namespace Proxy
{
    public class SoapServer : ISoapServer
    {
        public int Add(int num1, int num2)
        {
            return num1 + num2;
        }

        public string GetContracts(string city)
        {
            return CallGetContracts(city).Result;
        }

        public async Task<string> CallGetContracts(string city)
        {
            HttpClient client = new HttpClient();
            HttpResponseMessage response = await client.GetAsync("https://api.jcdecaux.com/vls/v1/stations?contract=" + city + "&apiKey=da2a1717115e9e7a90149fd7d0c4afcff086e014");
            string responseBody = await response.Content.ReadAsStringAsync();
            return responseBody;
        }
    }
}
