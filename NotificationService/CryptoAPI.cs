using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;

namespace NotificationService
{
    public class CryptoAPI
    {
        public static async Task<double> GetCryptoPrice(string symbol)
        {
            string url = $"https://api.binance.com/api/v3/ticker/price?symbol={symbol}";

            using (HttpClient client = new HttpClient())
            {
                HttpResponseMessage response = await client.GetAsync(url);
                response.EnsureSuccessStatusCode();

                string responseBody = await response.Content.ReadAsStringAsync();
                var cryptoData = JsonConvert.DeserializeObject<CryptoPrice>(responseBody);

                return cryptoData.Price;
            }

        }

    }

    public class CryptoPrice
    {
        [JsonProperty("symbol")]
        public string Symbol { get; set; }

        [JsonProperty("price")]
        public double Price { get; set; }
    }
}