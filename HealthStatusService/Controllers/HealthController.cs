using HealthMonitoringService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace HealthStatusService.Controllers {
    public class HealthController : Controller {
        HealthCheckRepository repo = new HealthCheckRepository();

        [HttpGet]
        public ActionResult Index() {
            double aliveCounter = 0;
            double deadCounter = 0;

            foreach (HealthCheck healthCheck in repo.ReadPortfolioServiceHealthChecksFromLast24Hours()) {
                if (healthCheck.IsAlive) {
                    aliveCounter++;
                } else {
                    deadCounter++;
                }
            }

            ViewBag.UptimePercentage = (aliveCounter / (aliveCounter + deadCounter)) * 100;
            ViewBag.HealthChecks = repo.ReadPortfolioServiceHealthChecksFromLastHour();

            return View();
        }
    }
}