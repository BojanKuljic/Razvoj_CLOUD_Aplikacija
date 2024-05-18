using PortfolioService.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PortfolioService.Controllers
{
    public class SearchController : Controller
    {
        [HttpGet]
        public ActionResult Index()
        {
            SearchViewModel model = new SearchViewModel();
            return View(model);
        }

        [HttpGet]
        public ActionResult Search(string searchQuery)
        {
            SearchViewModel model = new SearchViewModel
            {
                SearchQuery = searchQuery,
                Results = new List<Cryptocurrency>
                {
                    new Cryptocurrency { Name = "Bitcoin", CurrentPrice = 50000m },
                    new Cryptocurrency { Name = "Ethereum", CurrentPrice = 3000m }
                }
            };

            return View("Index", model);
        }
    }
}