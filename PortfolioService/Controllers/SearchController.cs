using PortfolioService.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PortfolioService.Controllers {
    public class SearchController : Controller {
        [HttpGet]
        public ActionResult Index() {
            return Content(@"
                            <html>
                            <body>
                                <p>nije implementirano</p>
                            </body>
                            </html>");
        }

        [HttpGet]
        public ActionResult Search() {
            return Content(@"
                            <html>
                            <body>
                                <p>nije implementirano</p>
                            </body>
                            </html>");
        }
    }
}