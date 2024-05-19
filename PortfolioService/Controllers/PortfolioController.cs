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
        private readonly UserDataRepository _userTableService;

        private CryptoAPI criptoAPI = new CryptoAPI();

        public PortfolioController()
        {
            //string storageConnectionString = System.Configuration.ConfigurationManager.AppSettings["DataConnectionString"];
            _cryptocurrencyTableService = new CryptocurrencyDataRepository();
            _userTableService = new UserDataRepository();
            _cryptocurrencyTableService.Initialize();
        }

        public ActionResult Index()
        {
            if(Session["User"] != null) {
                string email = Session["User"].ToString();
                User user = _userTableService.RetrieveUser(email);
                PortfolioViewModel model = new PortfolioViewModel
                {
                    Holdings = new List<CryptocurrencyHolding>()
                };
                double totalValue = 0;
                double totalProfitOrLoss = 0;
                if(user.TransactionIDs != null) { 
                    foreach (string TransKey in user.TransactionIDs)
                    {
                        CryptocurrencyTransaction crypto = _cryptocurrencyTableService.RetrieveCryptocurrency(TransKey);
                        Random rand = new Random();
                        double currentCryptoValue = rand.NextDouble(); // await criptoAPI.GetCryptoPrice("BTCUSD");
                        double currentValue = crypto.Amount * currentCryptoValue;
                        double profitOrLoss = currentValue - crypto.PurchaseOrSellValueInUSD;

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

                    }
                }
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
                        new CryptocurrencyHolding { Name = "Bitcoin", Amount = 0.5, Value = 25000, ProfitOrLoss = 5000 },
                        new CryptocurrencyHolding { Name = "Ethereum", Amount = 2, Value = 6000, ProfitOrLoss = 2000 }
                    },
                    TotalValue = 31000,
                    TotalProfitOrLoss = 7000
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