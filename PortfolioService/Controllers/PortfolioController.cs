using PortfolioService.Models;
using PortfolioServiceStorage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Threading.Tasks;
using PortfolioServiceStorage.Repos;
using CryptocurrencyConversion;
using PortfolioServiceStorage.TableEntityClasses;

namespace PortfolioService.Controllers {
    public class PortfolioController : Controller {
        CryptocurrencyRepository cryptoRepo = new CryptocurrencyRepository();
        TransactionRepository transactionRepo = new TransactionRepository();
        UserRepository userRepo = new UserRepository();
        CryptocurrencyConverter cryptoConverter = new CryptocurrencyConverter();

        [HttpGet]
        public async Task<ActionResult> Index() {
            if (Session["UserEmail"] == null) {
                return RedirectToAction("LogIn", "Account");
            }

            var cryptocurrencies = cryptoRepo.ReadUsersCryptocurrencies((string)Session["UserEmail"]);

            if (cryptocurrencies.Count() == 0) {
                ViewBag.Cryptocurrencies = null;
                return View();
            }

            ViewBag.Cryptocurrencies = cryptocurrencies;

            double totalSumUSD = 0;
            double totalProfitOrLoss = 0;
            foreach (Cryptocurrency cryptocurrency in cryptocurrencies) {
                totalSumUSD += await cryptoConverter.ConvertWithCurrentPrice(cryptocurrency.Name, "USDT", cryptocurrency.Amount);
                totalProfitOrLoss += cryptocurrency.ProfitOrLossUSD;
            }

            ViewBag.TotalSumUSD = String.Format("{0:F2}", totalSumUSD);
            ViewBag.TotalProfitOrLoss = String.Format("{0:F2}", totalProfitOrLoss);

            return View();
        }

        [HttpGet]
        public ActionResult AddTransaction() {
            if (Session["UserEmail"] == null) {
                return RedirectToAction("LogIn", "Account");
            }

            ViewBag.AddAttempt = false;

            return View();
        }

        [HttpPost]
        public async Task<ActionResult> ApplyAddTransaction() {
            if (Session["UserEmail"] == null) {
                return RedirectToAction("LogIn", "Account");
            }

            ViewBag.AddAttempt = true;

            string cryptocurrencyName = Request["Name"];
            string transactionType = Request["Type"];

            string transactionDateAndTime = Request["DateAndTime"];
            DateTime transactionDatAndTimeDT = DateTime.Parse(transactionDateAndTime);

            double transactionAmountUSD = Double.Parse(Request["Amount"]);
            double convertedTransactionAmount = await cryptoConverter.ConvertWithPastPrice("USDT", cryptocurrencyName, transactionAmountUSD, transactionDatAndTimeDT);

            if (convertedTransactionAmount == -1) {
                ViewBag.ConversionError = true;
                return View("AddTransaction");
            } else {
                ViewBag.ConversionError = false;
            }

            // treba uraditi validaciju da se provjeri da li je transactionDateAndTime noviji datum od datuma svih ostalih transakcija u bazi jer moramo natjerati korisnika da hornoloski unosi transakcije da bi izbjegli neke komplikovane probleme
            // treba provjeriti da li je transactionDateAndTime noviji datum od DateTime.Now

            ViewBag.Added = cryptoRepo.UpdateAmountAndProfitOrLoss((string)Session["UserEmail"], cryptocurrencyName, transactionType, transactionAmountUSD, convertedTransactionAmount);
            if (!ViewBag.Added) {
                return View("AddTransaction");
            }

            transactionRepo.Create(new Transaction(cryptocurrencyName, transactionType, transactionAmountUSD, convertedTransactionAmount, transactionDateAndTime, (string)Session["UserEmail"]));

            return View("AddTransaction");
        }
    }
}