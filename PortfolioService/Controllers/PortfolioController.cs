using PortfolioService.Models;
using PortfolioService_Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Threading.Tasks;

namespace PortfolioService.Controllers
{
    public class PortfolioController : Controller
    {
        private readonly CryptocurrencyDataRepository _cryptocurrencyTableService;
        private CryptoAPI criptoAPI = new CryptoAPI();

        public PortfolioController()
        {
            //string storageConnectionString = System.Configuration.ConfigurationManager.AppSettings["DataConnectionString"];
            _cryptocurrencyTableService = new CryptocurrencyDataRepository();
            _cryptocurrencyTableService.Initialize();
        }

        public ActionResult Index()
        {
            if(Session["User"] != null) {
                string email = Session["User"].ToString();
                CryptocurrencyTransaction crypto = _cryptocurrencyTableService.RetrieveCryptocurrency(email);
                if(crypto == null) {
                    PortfolioViewModel model2 = new PortfolioViewModel
                    {
                        Holdings = new List<CryptocurrencyHolding>
                    {
                        new CryptocurrencyHolding { Name = "Bitcoin", Amount = 0.5m, Value = 25000m, ProfitOrLoss = 5000m },
                        new CryptocurrencyHolding { Name = "Ethereum", Amount = 2m, Value = 6000m, ProfitOrLoss = 2000m }
                    },
                        TotalValue = 31000m,
                        TotalProfitOrLoss = 7000m
                    };
                    return View(model2);
                }
                PortfolioViewModel model = new PortfolioViewModel
                {
                    Holdings = new List<CryptocurrencyHolding>()
                };

                decimal totalValue = 0;
                decimal totalProfitOrLoss = 0;

                decimal currentCryptoValue = 0; // await criptoAPI.GetCryptoPrice("BTCUSD");
                decimal currentValue = crypto.Amount * currentCryptoValue;
                decimal profitOrLoss = currentValue - crypto.PurchaseOrSellValueInUSD;

                var holdingViewModel = new CryptocurrencyHolding
                {
                     Name = crypto.Name,
                     Amount = crypto.Amount,
                     Value = currentValue,
                     ProfitOrLoss = profitOrLoss
                };

                model.Holdings.Add(holdingViewModel);

                totalValue += currentValue;
                totalProfitOrLoss += profitOrLoss;

                model.TotalValue = totalValue;
                model.TotalProfitOrLoss = totalProfitOrLoss;
                return View(model);
            }
            else
            {
                PortfolioViewModel model = new PortfolioViewModel
                {
                    Holdings = new List<CryptocurrencyHolding>
                    {
                        new CryptocurrencyHolding { Name = "Bitcoin", Amount = 0.5m, Value = 25000m, ProfitOrLoss = 5000m },
                        new CryptocurrencyHolding { Name = "Ethereum", Amount = 2m, Value = 6000m, ProfitOrLoss = 2000m }
                    },
                    TotalValue = 31000m,
                    TotalProfitOrLoss = 7000m
                };
                return View(model);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddTransaction(string name, decimal amount, decimal value)
        {
            // TOODOO DODAJ TRANSAKCIJU
            return RedirectToAction("Index");
        }
    }
}