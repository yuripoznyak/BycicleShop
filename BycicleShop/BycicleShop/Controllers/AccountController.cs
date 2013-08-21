using System;
using System.Transactions;
using System.Web.Mvc;
using System.Web.Security;
using BycicleShop.Common.SqlContext.Concrete;
using BycicleShop.Models.AccountModels;
using Microsoft.Web.WebPages.OAuth;
using Recaptcha;
using WebMatrix.WebData;

namespace BycicleShop.Controllers
{
    [Authorize]
    //[InitializeSimpleMembership]
    public class AccountController : Controller
    {
        //
        // GET: /Account/Login
        private UsersDataContext _usersDataContext = new UsersDataContext();
        [AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        //
        // POST: /Account/Login

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult Login(LoginModel model, string returnUrl)
        {
            if (ModelState.IsValid && WebSecurity.Login(model.UserName, model.Password, model.RememberMe))
            {
                return RedirectToLocal(returnUrl);
            }

            // If we got this far, something failed, redisplay form
            ModelState.AddModelError("", "The user name or password provided is incorrect.");
            return View(model);
        }

        //
        // POST: /Account/LogOff

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LogOff()
        {
            WebSecurity.Logout();

            return RedirectToAction("Index", "Home");
        }

        //
        // GET: /Account/Register

        [AllowAnonymous]
        public ActionResult Register()
        {
            return View();
        }

        //
        // POST: /Account/Register
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        [RecaptchaControlMvc.CaptchaValidator]
        public ActionResult Register(RegisterModel model, bool captchaValid, string captchaErrorMessage)
        {
            if (ModelState.IsValid)
            {
                if (!captchaValid)
                {
                    ModelState.AddModelError("recaptcha", captchaErrorMessage);
                    return View(model);
                }
                // Attempt to register the user
                try
                {
                    var membership =
                        (Itransition.Membership.CustomMembershipProvider)
                        Membership.Providers["CustomMembershipProvider"];
                    MembershipCreateStatus createStatus;
                    membership.CreateUser(model.UserName, model.Password, model.EmailAdress, "", "", true, null, out createStatus);
                    if (createStatus == MembershipCreateStatus.Success)
                    {
                        FormsAuthentication.SetAuthCookie(model.UserName, false);
                    }
                    return RedirectToAction("Index", "Home");
                }
                catch (MembershipCreateUserException e)
                {
                    ModelState.AddModelError("", ErrorCodeToString(e.StatusCode));
                }
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }


        //
        // GET: /Account/Account

        public ActionResult Account()
        {
            return View();
        }

        [HttpGet]
        public ActionResult Manage()
        {
            var userId = WebSecurity.GetUserId(User.Identity.Name);
            if (userId == -1)
            {
                return RedirectToAction("Login");
            }
            var user = _usersDataContext.Entity.Find(userId);

            var model = new ManageModel
                {
                    Adress = user.Adress,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    PhoneNumber = user.PhoneNumber
                };
            return View(model);
        }

        [HttpPost]
        public ActionResult Manage(ManageModel model)
        {
            if (ModelState.IsValid)
            {
                var userId = WebSecurity.GetUserId(User.Identity.Name);
                if (userId == -1)
                {
                    return RedirectToAction("Login");
                }
                var user = _usersDataContext.Entity.Find(userId);
                user.Adress = model.Adress;
                user.FirstName = model.FirstName;
                user.LastName = model.LastName;
                user.PhoneNumber = model.PhoneNumber;
                _usersDataContext.SaveChanges();
                return RedirectToAction("Account");
            }
            return View(model);
        }

        [HttpGet]
        public ActionResult ChangePassword()
        {
            return View();
        }

        [HttpPost]
        public ActionResult ChangePassword(ChangePasswordModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var membership =
                        (Itransition.Membership.CustomMembershipProvider)
                        Membership.Providers["CustomMembershipProvider"];
                    if (membership.ChangePassword(User.Identity.Name, model.OldPassword, model.NewPassword))
                    {
                        return RedirectToAction("Account");
                    }
                    return View(model);
                }
                catch (Exception e)
                {
                    ModelState.AddModelError("", e.Message);
                }
            }
            return View(model);
        }

        [HttpGet]
        public ActionResult ChangeEmail()
        {
            var userId = WebSecurity.GetUserId(User.Identity.Name);
            if (userId == -1)
            {
                return RedirectToAction("Login");
            }
            var user = _usersDataContext.Entity.Find(userId);

            var model = new ChangeEmailModel
            {
                OldEmail = user.EmailAdress
            };
            return View(model);
        }

        [HttpPost]
        public ActionResult ChangeEmail(ChangeEmailModel model)
        {
            if (ModelState.IsValid)
            {
                var userId = WebSecurity.GetUserId(User.Identity.Name);
                if (userId == -1)
                {
                    return RedirectToAction("Login");
                }
                var user = _usersDataContext.Entity.Find(userId);
                user.EmailAdress = model.NewEmail;
                _usersDataContext.SaveChanges();
                return RedirectToAction("Account");
            }
            return View(model);
        }


        #region Helpers
        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            return RedirectToAction("Index", "Home");
        }

        public enum ManageMessageId
        {
            ChangePasswordSuccess,
            SetPasswordSuccess,
            RemoveLoginSuccess,
        }

        private static string ErrorCodeToString(MembershipCreateStatus createStatus)
        {
            // See http://go.microsoft.com/fwlink/?LinkID=177550 for
            // a full list of status codes.
            switch (createStatus)
            {
                case MembershipCreateStatus.DuplicateUserName:
                    return "User name already exists. Please enter a different user name.";

                case MembershipCreateStatus.DuplicateEmail:
                    return "A user name for that e-mail address already exists. Please enter a different e-mail address.";

                case MembershipCreateStatus.InvalidPassword:
                    return "The password provided is invalid. Please enter a valid password value.";

                case MembershipCreateStatus.InvalidEmail:
                    return "The e-mail address provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.InvalidAnswer:
                    return "The password retrieval answer provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.InvalidQuestion:
                    return "The password retrieval question provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.InvalidUserName:
                    return "The user name provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.ProviderError:
                    return "The authentication provider returned an error. Please verify your entry and try again. If the problem persists, please contact your system administrator.";

                case MembershipCreateStatus.UserRejected:
                    return "The user creation request has been canceled. Please verify your entry and try again. If the problem persists, please contact your system administrator.";

                default:
                    return "An unknown error occurred. Please verify your entry and try again. If the problem persists, please contact your system administrator.";
            }
        }
        #endregion
    }
}
