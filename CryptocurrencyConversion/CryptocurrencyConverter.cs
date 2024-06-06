using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace CryptocurrencyConversion {
    public class CryptocurrencyConverter
    {
        private async Task<double> GetCurrentCryptoPrice(string symbol)
        {
            string url = $"https://api.binance.com/api/v3/ticker/price?symbol={symbol}";
            Trace.TraceInformation($"Sending request to URL: {url}");

            using (HttpClient client = new HttpClient())
            {
                HttpResponseMessage response = await client.GetAsync(url);
                if (!response.IsSuccessStatusCode)
                {
                    string responseContent = await response.Content.ReadAsStringAsync();
                    Trace.TraceError($"Error fetching current price. Status Code: {response.StatusCode}, Response: {responseContent}");
                    throw new HttpRequestException($"Response status code does not indicate success: {response.StatusCode}");
                }

                string responseBody = await response.Content.ReadAsStringAsync();
                var cryptoData = JsonConvert.DeserializeObject<CryptoPrice>(responseBody);

                return cryptoData.Price;
            }
        }

        private async Task<double> GetPastCryptoPrice(string symbol, DateTime dateTime)
        {
            long unixTime = (long)(dateTime.ToUniversalTime() - new DateTime(1970, 1, 1)).TotalMilliseconds;

            string url = $"https://api.binance.com/api/v3/klines?symbol=BTCUSDT&interval=1m&limit=1";
            Trace.TraceInformation($"Sending request to URL: {url}");

            using (HttpClient client = new HttpClient())
            {
                HttpResponseMessage response = await client.GetAsync(url);
                if (!response.IsSuccessStatusCode)
                {
                    string responseContent = await response.Content.ReadAsStringAsync();
                    Trace.TraceError($"Error fetching past price. Status Code: {response.StatusCode}, Response: {responseContent}");
                    throw new HttpRequestException($"Response status code does not indicate success: {response.StatusCode}");
                }

                string jsonResponse = await response.Content.ReadAsStringAsync();
                JArray jsonArray = JArray.Parse(jsonResponse);

                if (jsonArray.Count == 0)
                {
                    return -1;
                }
                else
                {
                    return (double)jsonArray[0][4];
                }
            }
        }

        public async Task<double> ConvertWithCurrentPrice(string currencyFrom, string currencyTo, double amount)
        {
            double rate;
            if (currencyFrom == "USDT")
            {
                rate = await GetCurrentCryptoPrice(currencyTo + currencyFrom);
                rate = 1 / rate;
            }
            else
            {
                rate = await GetCurrentCryptoPrice(currencyFrom + currencyTo);
            }

            return amount * rate;
        }

        public async Task<double> ConvertWithPastPrice(string currencyFrom, string currencyTo, double amount, DateTime dateTime)
        {
            double rate;
            if (currencyFrom == "USDT")
            {
                rate = await GetPastCryptoPrice(currencyTo + currencyFrom, dateTime);

                if (rate == -1)
                {
                    return -1;
                }

                rate = 1 / rate;
            }
            else
            {
                rate = await GetPastCryptoPrice(currencyFrom + currencyTo, dateTime);

                if (rate == -1)
                {
                    return -1;
                }
            }

            return amount * rate;
        }
    }

    public class CryptoPrice {
        [JsonProperty("symbol")]
        public string Symbol { get; set; }

        [JsonProperty("price")]
        public double Price { get; set; }
    }
}

