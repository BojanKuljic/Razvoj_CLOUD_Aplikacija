using PortfolioService.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PortfolioService.Controllers
{
    public class PortfolioController : Controller
    {
        public ActionResult Index()
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

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddTransaction(string name, decimal amount, decimal value)
        {
            // TOODOO DODAJ TRANSAKCIJU

            return RedirectToAction("Index");
        }
    }
}