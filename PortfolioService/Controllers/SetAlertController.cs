using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Queue;
using PortfolioService.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace PortfolioService.Controllers {
    public class SetAlertController : Controller {
        [HttpGet]
        public ActionResult Index() {
            SetAlertViewModel model = new SetAlertViewModel();
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> SetAlert(SetAlertViewModel model) {
            if (Session["UserEmail"] == null) {
                return RedirectToAction("LogIn", "Account");
            }

            if (ModelState.IsValid) {
                CloudStorageAccount storageAccount = CloudStorageAccount.Parse("UseDevelopmentStorage=true");
                CloudQueueClient queueClient = storageAccount.CreateCloudQueueClient();
                CloudQueue queue = queueClient.GetQueueReference("alarms");
                await queue.CreateIfNotExistsAsync();

                string userEmail = (string)Session["UserEmail"];

                string messageContent = $"{model.CryptocurrencyName}|{model.AlertThreshold}|{userEmail}|{model.IsLowerTreshold}";
                CloudQueueMessage message = new CloudQueueMessage(messageContent);
                await queue.AddMessageAsync(message);

                ViewBag.Message = "Alert has been set successfully!";

                return RedirectToAction("Index", "Portfolio");
            }

            return View(model);
        }
    }
}