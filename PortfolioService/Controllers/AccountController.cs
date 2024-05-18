using System;
using System.Collections.Generic;
using System.Web.Mvc;
using System.Linq;
using System.Web;
using PortfolioService_Data;
using PortfolioService.Models;

namespace PortfolioService.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserDataRepository _userTableService;

        public AccountController()
        {
            string storageConnectionString = System.Configuration.ConfigurationManager.AppSettings["DataConnectionString"];
            _userTableService = new UserDataRepository();
            _userTableService.Initialize();
        }

        [HttpGet]
        public ActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                User user = new User(model.Email)
                {
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    Email = model.Email,
                    Address = model.Address,
                    City = model.City,
                    Country = model.Country,
                    PhoneNumber = model.PhoneNumber,
                    ProfilePicture = model.ProfilePicture,
                    PasswordHash = model.Password 
                };

                _userTableService.InsertOrMergeUser(user);
                return RedirectToAction("Index", "Portfolio");
            }

            return View(model);
        }

        [HttpGet]
        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                User user = _userTableService.RetrieveUser(model.Email);
                if (user != null && user.PasswordHash == model.Password)
                {
                    //TODO DRUGACIJI PRIKAZ
                    Session["User"] = user.Email;
                    return RedirectToAction("Index", "Portfolio");
                }
                ModelState.AddModelError("", "Invalid login attempt.");
            }

            return View(model);
        }

        [HttpGet]
        public ActionResult Logout()
        {
            //Kasnije dograditi po potrebi
            Session["User"] = null;
            return RedirectToAction("Index", "Portfolio");
        }

    }
}
