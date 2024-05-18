using PortfolioService.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PortfolioService.Controllers
{
    public class SetAlertController : Controller
    {
        [HttpGet]
        public ActionResult Index()
        {
            SetAlertViewModel model = new SetAlertViewModel();
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SetAlert(SetAlertViewModel model)
        {
            if (ModelState.IsValid)
            {
                //TODOOO SET ALERT!

                return RedirectToAction("Index", "Portfolio");
            }

            return View(model);
        }
    }
}