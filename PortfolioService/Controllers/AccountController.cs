using System;
using System.Collections.Generic;
using System.Web.Mvc;
using System.Linq;
using System.Web;
using PortfolioServiceStorage;
using PortfolioService.Models;
using System.Web.WebPages;
using System.IO;
using Microsoft.Azure;
using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.WindowsAzure.Storage;
using System.Reflection;

namespace PortfolioService.Controllers {
    public class AccountController : Controller {
        private readonly UserRepository _userTableService;
        private ProfilePictureRepository _profilePictureRepository;

        public AccountController() {
            //string storageConnectionString = System.Configuration.ConfigurationManager.AppSettings["DataConnectionString"];
            _userTableService = new UserRepository();

            _profilePictureRepository = new ProfilePictureRepository();
        }

        [HttpGet]
        public ActionResult Register() {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Register(RegisterViewModel model) {
            if (ModelState.IsValid) {
                if (model.ProfilePicture != null && model.ProfilePicture.ContentLength > 0) {
                    _profilePictureRepository.Create(model.Email, model.ProfilePicture);
                } else {
                    return Content(@"
                                    <html>
                                    <body>
                                        <p>error slika, napravite validaciju</p>
                                    </body>
                                    </html>");
                }

                User user = new User(model.Email) {
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    Email = model.Email,
                    Address = model.Address,
                    City = model.City,
                    Country = model.Country,
                    PhoneNumber = model.PhoneNumber,
                    PasswordHash = model.Password,
                };

                _userTableService.InsertOrMergeUser(user);
                return RedirectToAction("Index", "Portfolio");
            }

            return View(model);
        }

        [HttpGet]
        public ActionResult LogIn() {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LogIn(LogInViewModel model) {
            if (ModelState.IsValid) {
                User user = _userTableService.RetrieveUser(model.Email);
                if (user != null && user.PasswordHash == model.Password) {
                    //TODO DRUGACIJI PRIKAZ
                    Session["UserEmail"] = user.Email;
                    return RedirectToAction("Index", "Portfolio");
                }
                ModelState.AddModelError("", "Invalid login attempt.");
            }

            return View(model);
        }

        [HttpGet]
        public ActionResult Logout() {
            //Kasnije dograditi po potrebi
            Session["UserEmail"] = null;
            return RedirectToAction("LogIn");
        }

        [HttpGet]
        public ActionResult EditProfile() {
            if (Session["UserEmail"] == null) {
                return View("LogIn");
            }

            ViewBag.User = _userTableService.RetrieveUser((string)Session["UserEmail"]);
            ViewBag.Edited = false;
            ViewBag.PictureUri = _profilePictureRepository.GetUri(ViewBag.User.Email);

            return View();
        }

        [HttpPost]
        public ActionResult ApplyProfileEdit() {
            User oldUser = _userTableService.RetrieveUser((string)Session["UserEmail"]);
            _userTableService.DeleteUser(oldUser);

            User newUser = new User(Request["Email"]);
            newUser.FirstName = Request["FirstName"];
            newUser.LastName = Request["LastName"];
            newUser.Email = Request["Email"];
            newUser.Address = Request["Address"];
            newUser.City = Request["City"];
            newUser.Country = Request["Country"];
            newUser.PhoneNumber = Request["PhoneNumber"];

            HttpPostedFileBase picture = Request.Files["ProfilePicture"];
            if (picture != null && picture.ContentLength > 0) {
                _profilePictureRepository.Delete(oldUser.Email);
                ViewBag.PictureUri = _profilePictureRepository.Create(newUser.Email, picture);
            } else if (newUser.Email != oldUser.Email) {
                _profilePictureRepository.UpdateUri(oldUser.Email, newUser.Email);
            }

            if (Request["Password"].IsEmpty()) {
                newUser.PasswordHash = oldUser.PasswordHash;
            } else {
                newUser.PasswordHash = Request["Password"];
            }

            _userTableService.InsertOrMergeUser(newUser);

            Session["UserEmail"] = newUser.Email;

            ViewBag.User = newUser;
            ViewBag.Edited = true;
            ViewBag.PictureUri = _profilePictureRepository.GetUri(newUser.Email);

            return View("EditProfile");
        }
    }
}
