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
        public ActionResult LogIn() {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LogIn(LogInViewModel model) {
            if (ModelState.IsValid) {
                User user = _userTableService.RetrieveUser(model.Email);
                if (user != null && user.PasswordHash == model.Password) {
                    Session["UserEmail"] = user.Email;
                    return RedirectToAction("Index", "Portfolio");
                }
                if (user == null) {
                    ModelState.AddModelError("Email", "Email not found.");
                } else if (user.PasswordHash != model.Password) {
                    ModelState.AddModelError("Password", "Incorrect password.");
                }
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
                    ModelState.AddModelError("ProfilePicture", "Profile picture is required.");
                    return View(model);
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
        [OutputCache(NoStore = true, Duration = 0)]
        public ActionResult EditProfile() {
            /*
            if (Session["UserEmail"] == null) {
                return View("LogIn");
            }
            
            ViewBag.User = _userTableService.RetrieveUser((string)Session["UserEmail"]);
            ViewBag.Edited = false;
            ViewBag.PictureUri = _profilePictureRepository.GetUri(ViewBag.User.Email);

            return View();*/

            Response.Headers["Pragma-directive"] = "no-cache";
            Response.Headers["Cache-directive"] = "no-cache";
            Response.Headers["Cache-control"] = "no-cache";
            Response.Headers["Pragma"] = "no-cache";
            Response.Headers["Expires"] = "0";

            if (Session["UserEmail"] == null) {
                return View("LogIn");
            }

            var user = _userTableService.RetrieveUser((string)Session["UserEmail"]);
            var model = new EditViewModel {
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                Address = user.Address,
                City = user.City,
                Country = user.Country,
                PhoneNumber = user.PhoneNumber
            };

            ViewBag.PictureUri = _profilePictureRepository.GetUri(user.Email);
            ViewBag.Edited = false;

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [OutputCache(NoStore = true, Duration = 0)]
        public ActionResult ApplyProfileEdit(EditViewModel model) {
            if (Session["UserEmail"] == null) {
                return View("LogIn");
            }

            Response.Headers["Pragma-directive"] = "no-cache";
            Response.Headers["Cache-directive"] = "no-cache";
            Response.Headers["Cache-control"] = "no-cache";
            Response.Headers["Pragma"] = "no-cache";
            Response.Headers["Expires"] = "0";

            if (ModelState.IsValid) {
                var oldUser = _userTableService.RetrieveUser((string)Session["UserEmail"]);
                _userTableService.DeleteUser(oldUser);

                var newUser = new User(model.Email) {
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    Email = model.Email,
                    Address = model.Address,
                    City = model.City,
                    Country = model.Country,
                    PhoneNumber = model.PhoneNumber,
                    PasswordHash = string.IsNullOrEmpty(model.NewPassword) ? oldUser.PasswordHash : model.NewPassword
                };

                if (model.ProfilePicture != null && model.ProfilePicture.ContentLength > 0) {
                    _profilePictureRepository.Delete(oldUser.Email);
                    ViewBag.PictureUri = _profilePictureRepository.Create(newUser.Email, model.ProfilePicture);
                } else if (newUser.Email != oldUser.Email) {
                    _profilePictureRepository.UpdateUri(oldUser.Email, newUser.Email);
                }

                _userTableService.InsertOrMergeUser(newUser);

                Session["UserEmail"] = newUser.Email;

                ViewBag.User = newUser;
                ViewBag.Edited = true;
                ViewBag.PictureUri = _profilePictureRepository.GetUri(newUser.Email);

                return RedirectToAction("EditProfile");
            }

            ViewBag.PictureUri = _profilePictureRepository.GetUri((string)Session["UserEmail"]);
            return RedirectToAction("EditProfile");
        }
    }
}
