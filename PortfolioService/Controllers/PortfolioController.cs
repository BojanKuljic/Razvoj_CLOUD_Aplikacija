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
        public async Task<ActionResult> ApplyAddTransaction(AddTransactionViewModel model) {
            if (Session["UserEmail"] == null) {
                return RedirectToAction("LogIn", "Account");
            }

            string userEmail = Session["UserEmail"] as string;

            if (!ModelState.IsValid)
            {
                return View("AddTransaction", model);
            }

            ViewBag.AddAttempt = true;

            string cryptocurrencyName = model.CryptocurrencyType;
            string transactionType = model.TransactionType;
            DateTime transactionDateAndTime = model.DateAndTime;
            string transactionDateAndTimeST = transactionDateAndTime.ToString();
            double transactionAmountUSD = model.Amount;
            
            double convertedTransactionAmount = await cryptoConverter.ConvertWithPastPrice("USDT", cryptocurrencyName, transactionAmountUSD, transactionDateAndTime);

            if (convertedTransactionAmount == -1)
            {
                ViewBag.ConversionError = true;
                return View("AddTransaction", model);
            }
            else
            {
                ViewBag.ConversionError = false;
            }

            if (transactionDateAndTime > DateTime.Now)
            {
                ModelState.AddModelError("DateAndTime", "Transaction date and time cannot be in the future.");
                return View("AddTransaction", model);
            }

            List<Transaction> userTransactions = transactionRepo.ReadUsersTransactions(userEmail).ToList();

            foreach(Transaction t in userTransactions)
            {
                if(DateTime.Parse(t.DateAndTime) >= transactionDateAndTime)
                {
                    ModelState.AddModelError("DateAndTime", "Transaction must be newer than all previous transactions");
                    return View("AddTransaction", model);
                }
            }

            ViewBag.Added = cryptoRepo.UpdateAmountAndProfitOrLoss((string)Session["UserEmail"], cryptocurrencyName, transactionType, transactionAmountUSD, convertedTransactionAmount);
            if (!ViewBag.Added)
            {
                return View("AddTransaction", model);
            }

            transactionRepo.Create(new Transaction(cryptocurrencyName, transactionType, transactionAmountUSD, convertedTransactionAmount, transactionDateAndTimeST, (string)Session["UserEmail"]));

            return View("AddTransaction");
        }
    }
}