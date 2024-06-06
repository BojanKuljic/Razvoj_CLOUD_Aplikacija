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
    public class CryptocurrencyConverter {
        private async Task<double> GetCurrentCryptoPrice(string symbol) {
            string url = $"https://api.binance.com/api/v3/ticker/price?symbol={symbol}";

            using (HttpClient client = new HttpClient()) {
                HttpResponseMessage response = await client.GetAsync(url);
                response.EnsureSuccessStatusCode();

                string responseBody = await response.Content.ReadAsStringAsync();
                var cryptoData = JsonConvert.DeserializeObject<CryptoPrice>(responseBody);

                return cryptoData.Price;
            }
        }

        private async Task<double> GetPastCryptoPrice(string symbol, DateTime dateTime) {
            string unixTime = (dateTime - new DateTime(1970, 1, 1)).TotalMilliseconds.ToString();

            string url = $"https://api.binance.com/api/v3/klines?symbol={symbol}&interval=1m&endTime={unixTime}&limit=1";

            using (HttpClient client = new HttpClient()) {
                HttpResponseMessage response = await client.GetAsync(url);
                response.EnsureSuccessStatusCode();

                string jsonResponse = await response.Content.ReadAsStringAsync();
                JArray jsonArray = JArray.Parse(jsonResponse);

                if (jsonArray.Count == 0) {
                    return -1;
                } else {
                    return (double)jsonArray[0][4];
                }
            }
        }

        // "amount" is value in "currencyFrom". Use "USDT" for USD.
        public async Task<double> ConvertWithCurrentPrice(string currencyFrom, string currencyTo, double amount) {
            double rate;
            if (currencyFrom == "USDT") {
                rate = await GetCurrentCryptoPrice(currencyTo + currencyFrom);
                rate = 1 / rate;
            } else {
                rate = await GetCurrentCryptoPrice(currencyFrom + currencyTo);
            }

            return amount * rate;
        }

        public async Task<double> ConvertWithPastPrice(string currencyFrom, string currencyTo, double amount, DateTime dateTime) {
            double rate;
            if (currencyFrom == "USDT") {
                rate = await GetPastCryptoPrice(currencyTo + currencyFrom, dateTime);

                if (rate == -1) {
                    return -1;
                }

                rate = 1 / rate;
            } else {
                rate = await GetPastCryptoPrice(currencyFrom + currencyTo, dateTime);

                if (rate == -1) {
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

