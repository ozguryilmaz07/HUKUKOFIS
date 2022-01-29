﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.Profile;
using System.Web.Security;
using AutoMapper;

namespace OpenLawOffice.Web.Controllers
{
    [HandleError(View = "Errors/Index", Order = 10)]
    public class AccountController : BaseController
    {
        public IFormsAuthenticationService FormsService { get; set; }
        public AccountMembershipService MembershipService { get; set; }

        protected override void Initialize(RequestContext requestContext)
        {
            if (FormsService == null) { FormsService = new FormsAuthenticationService(); }
            if (MembershipService == null) { MembershipService = new AccountMembershipService(); }

            base.Initialize(requestContext);
        }

        [Authorize(Roles = "Login, User")]
        public ActionResult Index()
        {
            return View();
        }

        [AllowAnonymous]
        public ActionResult Login()
        {
            List<Common.Models.Account.Users> users = null;

            try
            {
                users = Data.Account.Users.List();
            }
            catch (Exception)
            {
                return RedirectToAction("Index", "Installation");
            }

            if (users == null || users.Count < 1)
                return RedirectToAction("Register", "Account");

            return View();
        }

        [AllowAnonymous]
        [HttpPost]
        public ActionResult Login(ViewModels.Account.LoginViewModel viewModel, string returnUrl)
        {
            if (ModelState.IsValid)
            {
                if (MembershipService.ValidateUser(viewModel.Username, viewModel.Password))
                {
                    FormsService.SignIn(viewModel.Username, viewModel.RememberMe);
                    if (!string.IsNullOrEmpty(returnUrl))
                    {
                        return Redirect(returnUrl);
                    }
                    else
                    {
                        return RedirectToAction("Index", "Home");
                    }
                }
                else
                {
                    ModelState.AddModelError("", "The user name or password provided is incorrect.");
                }
            }

            // If we got this far, something failed, redisplay form
            return View(viewModel);
        }

        public ActionResult Logout()
        {
            FormsService.SignOut();

            return RedirectToAction("Login", "Account");
        }

        [AllowAnonymous]
        public ActionResult Register()
        {
            ViewBag.PasswordLength = MembershipService.MinPasswordLength;
            return View();
        }

        [AllowAnonymous]
        [HttpPost]
        public ActionResult Register(ViewModels.Account.RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                // Attempt to register the user
                MembershipCreateStatus createStatus = MembershipService.CreateUser(model.UserName, model.Password, model.Email);

                if (createStatus == MembershipCreateStatus.Success)
                {
                    FormsService.SignIn(model.UserName, false /* createPersistentCookie */);
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    ModelState.AddModelError("", ViewModels.AccountValidation.ErrorCodeToString(createStatus));
                }
            }

            // If we got this far, something failed, redisplay form
            ViewBag.PasswordLength = MembershipService.MinPasswordLength;
            return View(model);
        }

        [Authorize(Roles = "Login")]
        public ActionResult ChangePassword(string currentPassword)
        {
            ViewBag.PasswordLength = MembershipService.MinPasswordLength;
            if (!string.IsNullOrEmpty(currentPassword))
                return View(new ViewModels.Account.ChangePasswordViewModel()
                {
                    OldPassword = currentPassword
                });
            else
                return View(new ViewModels.Account.ChangePasswordViewModel());
        }

        [HttpPost]
        [Authorize(Roles = "Login")]
        public ActionResult ChangePassword(ViewModels.Account.ChangePasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                if (MembershipService.ChangePassword(User.Identity.Name, model.OldPassword, model.NewPassword))
                {
                    return RedirectToAction("ChangePasswordSuccess");
                }
                else
                {
                    ModelState.AddModelError("", "The current password is incorrect or the new password is invalid.");
                }
            }

            // If we got this far, something failed, redisplay form
            ViewBag.PasswordLength = MembershipService.MinPasswordLength;
            return View(model);
        }

        public ActionResult ChangePasswordSuccess()
        {
            return View();
        }

        [Authorize(Roles = "Login, User")]
        public ActionResult EditUser()
        {
            Common.Models.Account.Users model;
            ViewModels.Account.UsersViewModel viewModel;

            model = Data.Account.Users.Get(Membership.GetUser().UserName);

            viewModel = Mapper.Map<ViewModels.Account.UsersViewModel>(model);

            viewModel.Password = null;
            return View(viewModel);
        }

        [HttpPost]
        [Authorize(Roles = "Login, User")]
        public ActionResult EditUser(string username, ViewModels.Account.UsersViewModel viewModel)
        {
            MembershipUser user;

            user = MembershipService.GetUser(Membership.GetUser().UserName);

            user.Email = viewModel.Email;

            MembershipService.UpdateUser(user);

            return RedirectToAction("Index", "Home");
        }

        [Authorize(Roles = "Login, User")]
        public new ActionResult Profile()
        {
            int? contactId = null;
            string externalAppKey = null;
            dynamic profile;
            List<ViewModels.Contacts.ContactViewModel> employeeList;

            profile = ProfileBase.Create(Membership.GetUser().UserName);

            if (profile != null && profile.ContactId != null
                && !string.IsNullOrEmpty(profile.ContactId))
                contactId = int.Parse(profile.ContactId);

            if (profile != null && profile.ExternalAppKey != null
                && !string.IsNullOrEmpty(profile.ExternalAppKey))
                externalAppKey = profile.ExternalAppKey;

            if (string.IsNullOrEmpty(externalAppKey) ||
                (Request["newAppKey"] != null && Request["newAppKey"] == "true"))
            {
                Common.Encryption enc = new Common.Encryption();
                enc.GenerateKey();
                externalAppKey = enc.Key;
            }

            employeeList = new List<ViewModels.Contacts.ContactViewModel>();
            employeeList.Add(new ViewModels.Contacts.ContactViewModel()
            {
                Id = null,
                DisplayName = "< None >"
            });

            Data.Contacts.Contact.ListEmployeesOnly().ForEach(x =>
            {
                employeeList.Add(Mapper.Map<ViewModels.Contacts.ContactViewModel>(x));
            });

            ViewBag.EmployeeContactList = employeeList;

            return View(new ViewModels.Account.ProfileViewModel() { ContactId = contactId, ExternalAppKey = externalAppKey });
        }

        [HttpPost]
        [Authorize(Roles = "Login, User")]
        public new ActionResult Profile(ViewModels.Account.ProfileViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                bool update = false;
                dynamic profile;

                profile = ProfileBase.Create(Membership.GetUser().UserName);

                if (viewModel.ContactId != null && viewModel.ContactId.HasValue)
                {
                    profile.ContactId = viewModel.ContactId.Value.ToString();
                    update = true;
                }
                else
                {
                    profile.ContactId = "";
                    update = true;
                }

                if (!string.IsNullOrEmpty(viewModel.ExternalAppKey))
                {
                    profile.ExternalAppKey = viewModel.ExternalAppKey;
                    update = true;
                }

                if (update)
                    profile.Save();
            }

            return RedirectToAction("Index", "Home");
        }

        public ActionResult Recovery(string reset, string username)
        {
            if ((reset != null) && (username != null))
            {
                MembershipUser currentUser = Membership.GetUser(username);
                if (HashResetParams(currentUser.UserName, currentUser.ProviderUserKey.ToString()) == reset)
                {
                    string tempPw = currentUser.ResetPassword();
                    FormsService.SignIn(currentUser.UserName, true);
                    return RedirectToAction("ChangePassword", new { currentPassword = tempPw });
                }
            }

            return View();
        }

        public static string HashResetParams(string username, string guid)
        {
            byte[] bytesofLink = System.Text.Encoding.UTF8.GetBytes(username + guid);
            System.Security.Cryptography.MD5 md5 = new System.Security.Cryptography.MD5CryptoServiceProvider();
            string HashParams = BitConverter.ToString(md5.ComputeHash(bytesofLink)).Replace("-", "").ToLower();

            return HashParams;
        }

        public void SendResetEmail(System.Web.Security.MembershipUser user)
        {
            string site = Common.Settings.Manager.Instance.System.WebsiteUrl.ToString();

            if (!site.EndsWith("\\") && !site.EndsWith("/"))
                site += "/";

            EmailView<ViewModels.Account.RecoveryViewModel>("~/Views/Templates/AccountRecoveryEmail.aspx",
                new ViewModels.Account.RecoveryViewModel()
                {
                    Email = user.Email,
                    ResetPwAddress = site + "Account/Recovery/?username=" + user.UserName + "&reset=" + HashResetParams(user.UserName, user.ProviderUserKey.ToString()),
                    IpAddress = Common.Utilities.GetClientIpAddress(Request),
                    UserName = user.UserName
                }, user.Email, "Account Recovery");
        }

        [HttpPost]
        public ActionResult Recovery(ViewModels.Account.RecoveryViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                if (!string.IsNullOrEmpty(viewModel.UserName))
                {
                    MembershipUser user = Membership.GetUser(viewModel.UserName);
                    if (user == null)
                        ViewBag.Error = "username";
                    else
                    {
                        SendResetEmail(user);
                        return View("RecoveryEmailSent");
                    }
                }
                else
                {
                    MembershipUserCollection coll = Membership.FindUsersByEmail(viewModel.Email);
                    if (coll.Count != 1)
                    {
                        ViewBag.Error = "email";
                    }
                    else
                    {
                        foreach (MembershipUser user in coll)
                        {
                            SendResetEmail(user);
                            return View("RecoveryEmailSent");
                        }
                    }
                }
            }

            return View();
        }
    }
}