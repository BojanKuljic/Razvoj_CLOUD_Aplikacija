using CryptocurrencyConversion;
using PortfolioService.Models;
using PortfolioServiceStorage.Repos;
using PortfolioServiceStorage.TableEntityClasses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace PortfolioService.Controllers {
    public class SearchController : Controller {
        CryptocurrencyRepository cryptoRepo = new CryptocurrencyRepository();
        CryptocurrencyConverter cryptoConverter = new CryptocurrencyConverter();

        [HttpGet]
        public ActionResult Index() {
            if (Session["UserEmail"] == null) {
                return RedirectToAction("LogIn", "Account");
            }

            SearchViewModel model = new SearchViewModel();
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> SearchCrypto(SearchViewModel model) {
            if (Session["UserEmail"] == null) {
                return RedirectToAction("LogIn", "Account");
            }

            if (ModelState.IsValid) {
                ViewBag.CryptoType = model.CryptocurrencyType;
                ViewBag.CryptoPrice = await cryptoConverter.ConvertWithCurrentPrice(model.CryptocurrencyType, "USDT", 1);
                var cryptocurrencies = cryptoRepo.ReadUsersCryptocurrencies((string)Session["UserEmail"]);
                List<Cryptocurrency> newCurrencies = new List<Cryptocurrency>();


                foreach (Cryptocurrency c in cryptocurrencies) {
                    if (c.Name == model.CryptocurrencyType) {
                        newCurrencies.Add(c);
                    }
                }

                ViewBag.Cryptocurrencies = newCurrencies;
            }
            return View(model);
        }
    }
}