using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Logging;
using TutoringMarket.WebIdentity.Models;
using TutoringMarket.WebIdentity.Models.AccountViewModels;
using TutoringMarket.WebIdentity.Services;
using LoginAuthentication;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using TutoringMarket.WebIdentity.Data;
using System.DirectoryServices.Protocols;
using TutoringMarket.Core.Contracts;
using TutoringMarket.Core.Enities;

namespace TutoringMarket.WebIdentity.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IEmailSender _emailSender;
        private readonly ISmsSender _smsSender;
        private readonly ILogger _logger;
        private IUnitOfWork uow;

        public AccountController(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            IEmailSender emailSender,
            ISmsSender smsSender,
            ILoggerFactory loggerFactory,
            IUnitOfWork _uow)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _emailSender = emailSender;
            _smsSender = smsSender;
            _logger = loggerFactory.CreateLogger<AccountController>();
            uow = _uow;
        }

        //
        // GET: /Account/Login
        [HttpGet]
        [AllowAnonymous]
        public IActionResult Login(string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        //
        // POST: /Account/Login
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model, string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            if (ModelState.IsValid)
            {
                //Ursprünglicher Code

                //This doesn't count login failures towards account lockout
                // To enable password failures to trigger account lockout, set lockoutOnFailure: true
                //var result = await _signInManager.PasswordSignInAsync(model.UserName, model.Password,false, lockoutOnFailure: false);

                //var user = _userManager.FindByNameAsync(model.UserName);
                //var result = await _signInManager.SignInAsync(user, true);
                //if (result.Succeeded)
                //{
                //    _logger.LogInformation(1, "User logged in.");
                //    return RedirectToLocal(returnUrl);
                //}
                //if (result.RequiresTwoFactor)
                //{
                //    return RedirectToAction(nameof(SendCode), new { ReturnUrl = returnUrl});
                //}
                //if (result.IsLockedOut)
                //{
                //    _logger.LogWarning(2, "User account locked out.");
                //    return View("Lockout");
                //}
                //else
                //{
                //    ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                //    return View(model);
                //}

                //for offline testing (Server ist nicht erreichbar von daheim, daher müssen Name, Klasse und Abteilung hard codiert werden)
                //string fullName = "Test Test";
                //string schoolClass = "3AHIF";
                //string department = "Informatik";

                //real code
                string fullName = "";
                string schoolClass = "";
                string department = "";

                bool isTeacher = false;

                //Liefert Name, Klasse, Abteilung, isTeacher und ob die Anmeldedaten gültig sind
                bool result = GetResult(model.UserName, model.Password, ref fullName, ref schoolClass, ref department, ref isTeacher);

                //offline testing (Die Anmeldedaten sind gültig daheim)
                //bool result = true; //server was offline
                if (result)
                {
                    //Neuer User (Achtung Name muss ein Leerzeichen enthalten!)
                    var user = new ApplicationUser { UserName = model.UserName, FirstName=fullName.Split(' ')[1], LastName=fullName.Split(' ')[0], SchoolClass=schoolClass, Department = department};

                    //Existiert der User schon?
                    if (_userManager.Users.Where(u => u.UserName == model.UserName).FirstOrDefault() == null)
                    {
                        //=>User exisitert noch nicht
                        //User wird angelegt
                        await _userManager.CreateAsync(user);
                        if(isTeacher)
                        {
                            //Lehrer-Rolle hinzufügen
                            await _userManager.AddToRoleAsync(user, "Teacher");
                        }
                        else
                        {
                            //Besucher ist normal
                            await _userManager.AddToRoleAsync(user, "Visitor");
                        }
                    }
                    //Der User wird eingeloggt
                    //TODO try catch sollte eingefügt werden, falls etwas schief geht
                    await _signInManager.SignInAsync(_userManager.Users.Where(u => u.UserName == model.UserName).FirstOrDefault(), true);
                    _logger.LogInformation(1, "User logged in.");
                    return RedirectToLocal(returnUrl);
                }
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        //TODO: Delete unnecassary code
        //TODO: selbstsigniertes Zertifikat verwendet
        /// <summary>
        /// //Liefert Name, Klasse, Abteilung, isTeacher und ob die Anmeldedaten gültig sind
        /// </summary>
        /// <param name="name"></param>
        /// <param name="password"></param>
        /// <param name="fullName"></param>
        /// <param name="schoolClass"></param>
        /// <param name="department"></param>
        /// <param name="isTeacher"></param>
        /// <returns></returns>
        private bool GetResult(string name, string password, ref string fullName, ref string schoolClass, ref string department, ref bool isTeacher)
        {
            bool result = true;
            if (!string.IsNullOrEmpty(name) && !string.IsNullOrEmpty(password))
            {
                try
                {
                    //Verbindung zum LDAP-Server aufbauen
                    LdapConnection con = new LdapConnection(new LdapDirectoryIdentifier("addc01.edu.htl-leonding.ac.at:636"), new System.Net.NetworkCredential(name + "@EDU", password));
                    con.Bind();
                    //Ist der Benutzer ein Lehrer?
                    if(name.Contains('.'))
                    {
                        //TODO full name of teacher
                        //Derzeit wird p.bauer in Vorname=p und Nachname=bauer gespeichert (Sysadmin fragen)
                        fullName = name.Replace('.',' ');
                        isTeacher = true;
                    }
                    else
                    {
                        //Andere Daten erhalten
                        //Code könnte schöner sein
                        String[] array = { "dn", "displayName", "gecos" };
                        DirectoryRequest dr = new SearchRequest("ou=Students,ou=HTL,DC=EDU,DC=HTL-LEONDING,DC=AC,DC=AT", "(cn="+name+")", System.DirectoryServices.Protocols.SearchScope.Subtree, array);
                        var dresp = (System.DirectoryServices.Protocols.SearchResponse)con.SendRequest(dr);
                        var entries = dresp.Entries[0].DistinguishedName.Split(',');
                        schoolClass = entries[1].Split('=')[1];
                        department = this.GetFullDepartmentName(entries[2].Split('=')[1]);
                        var values = dresp.Entries[0].Attributes.Values;
                        foreach (var item in values)
                        {
                            var directoryAttributte = (DirectoryAttribute)item;
                            fullName = directoryAttributte.GetValues(typeof(string))[0].ToString();
                            break;
                        }
                    }
                    this.InsertClasses(con);
                }
                catch(Exception e)
                {
                    //Falls Anmeldedaten nicht gültig sind, Server offline usw..
                    ModelState.AddModelError(String.Empty, e.Message);
                    result = false;
                    Console.WriteLine(e.Message);
                }
   
            }
            return result;
        }
        private void InsertClasses(LdapConnection con)
        {
            try
            {
                //alle Klassen finden
                String[] a = { "dn", "displayName", "gecos" };
                DirectoryRequest directoryR = new SearchRequest("ou=Students,ou=HTL,DC=EDU,DC=HTL-LEONDING,DC=AC,DC=AT", "(ou=*)", System.DirectoryServices.Protocols.SearchScope.Subtree, a);
                var re = (System.DirectoryServices.Protocols.SearchResponse)con.SendRequest(directoryR);

                foreach (var item in re.Entries)
                {
                    SearchResultEntry entry = item as SearchResultEntry;
                    int tmp;
                    if (entry.DistinguishedName.StartsWith("OU=") && int.TryParse(entry.DistinguishedName[3].ToString(), out tmp))
                    {
                        var name = entry.DistinguishedName.Split(',')[0].Split('=')[1];
                        if (uow.ClassRepository.Get(filter: f => f.Name == name).FirstOrDefault() == null)
                            uow.ClassRepository.Insert(new SchoolClass { Name = name });
                    }
                }
                uow.Save();
            }
            catch(Exception e)
            {
                
            }
        }
        /// <summary>
        /// Abteilung wird abgekürzt vom Server geholt => Durch vollen Namen ersetzen
        /// </summary>
        /// <param name="shortName"></param>
        /// <returns></returns>
        private string GetFullDepartmentName(string shortName)
        {
            switch (shortName)
            {
                case "IF":
                    return "Informatik";
                case "BG":
                    return "Medizintechnik";
                case "FE":
                    return "Fachschule Elektronik";
                case "HE":
                    return "Elektronik";
                case "IT":
                    return "Medientechnik";
                case "AD":
                    return "Abendschule";
                case "KD":
                    return "Kolleg";
                default:
                    return "";
            }
        }

        //
        // GET: /Account/Register
        [HttpGet]
        [AllowAnonymous]
        public IActionResult Register(string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        //
        // POST: /Account/Register
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model, string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            if (ModelState.IsValid)
            {
                var user = new ApplicationUser { UserName = model.Email, Email = model.Email };
                var result = await _userManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    // For more information on how to enable account confirmation and password reset please visit http://go.microsoft.com/fwlink/?LinkID=532713
                    // Send an email with this link
                    //var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                    //var callbackUrl = Url.Action("ConfirmEmail", "Account", new { userId = user.Id, code = code }, protocol: HttpContext.Request.Scheme);
                    //await _emailSender.SendEmailAsync(model.UserName, "Confirm your account",
                    //    $"Please confirm your account by clicking this link: <a href='{callbackUrl}'>link</a>");
                    await _signInManager.SignInAsync(user, isPersistent: false);
                    _logger.LogInformation(3, "User created a new account with password.");
                    return RedirectToLocal(returnUrl);
                }
                AddErrors(result);
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        //
        // POST: /Account/LogOff
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> LogOff()
        {
            await _signInManager.SignOutAsync();
            _logger.LogInformation(4, "User logged out.");
            return RedirectToAction(nameof(HomeController.Index), "Home");
        }

        //
        // POST: /Account/ExternalLogin
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public IActionResult ExternalLogin(string provider, string returnUrl = null)
        {
            // Request a redirect to the external login provider.
            var redirectUrl = Url.Action("ExternalLoginCallback", "Account", new { ReturnUrl = returnUrl });
            var properties = _signInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl);
            return Challenge(properties, provider);
        }

        //
        // GET: /Account/ExternalLoginCallback
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> ExternalLoginCallback(string returnUrl = null, string remoteError = null)
        {
            if (remoteError != null)
            {
                ModelState.AddModelError(string.Empty, $"Error from external provider: {remoteError}");
                return View(nameof(Login));
            }
            var info = await _signInManager.GetExternalLoginInfoAsync();
            if (info == null)
            {
                return RedirectToAction(nameof(Login));
            }

            // Sign in the user with this external login provider if the user already has a login.
            var result = await _signInManager.ExternalLoginSignInAsync(info.LoginProvider, info.ProviderKey, isPersistent: false);
            if (result.Succeeded)
            {
                _logger.LogInformation(5, "User logged in with {Name} provider.", info.LoginProvider);
                return RedirectToLocal(returnUrl);
            }
            if (result.RequiresTwoFactor)
            {
                return RedirectToAction(nameof(SendCode), new { ReturnUrl = returnUrl });
            }
            if (result.IsLockedOut)
            {
                return View("Lockout");
            }
            else
            {
                // If the user does not have an account, then ask the user to create an account.
                ViewData["ReturnUrl"] = returnUrl;
                ViewData["LoginProvider"] = info.LoginProvider;
                var email = info.Principal.FindFirstValue(ClaimTypes.Email);
                return View("ExternalLoginConfirmation", new ExternalLoginConfirmationViewModel { Email = email });
            }
        }

        //
        // POST: /Account/ExternalLoginConfirmation
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ExternalLoginConfirmation(ExternalLoginConfirmationViewModel model, string returnUrl = null)
        {
            if (ModelState.IsValid)
            {
                // Get the information about the user from the external login provider
                var info = await _signInManager.GetExternalLoginInfoAsync();
                if (info == null)
                {
                    return View("ExternalLoginFailure");
                }
                var user = new ApplicationUser { UserName = model.Email, Email = model.Email };
                var result = await _userManager.CreateAsync(user);
                if (result.Succeeded)
                {
                    result = await _userManager.AddLoginAsync(user, info);
                    if (result.Succeeded)
                    {
                        await _signInManager.SignInAsync(user, isPersistent: false);
                        _logger.LogInformation(6, "User created an account using {Name} provider.", info.LoginProvider);
                        return RedirectToLocal(returnUrl);
                    }
                }
                AddErrors(result);
            }

            ViewData["ReturnUrl"] = returnUrl;
            return View(model);
        }

        // GET: /Account/ConfirmEmail
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> ConfirmEmail(string userId, string code)
        {
            if (userId == null || code == null)
            {
                return View("Error");
            }
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return View("Error");
            }
            var result = await _userManager.ConfirmEmailAsync(user, code);
            return View(result.Succeeded ? "ConfirmEmail" : "Error");
        }

        //
        // GET: /Account/ForgotPassword
        [HttpGet]
        [AllowAnonymous]
        public IActionResult ForgotPassword()
        {
            return View();
        }

        //
        // POST: /Account/ForgotPassword
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByNameAsync(model.Email);
                if (user == null || !(await _userManager.IsEmailConfirmedAsync(user)))
                {
                    // Don't reveal that the user does not exist or is not confirmed
                    return View("ForgotPasswordConfirmation");
                }

                // For more information on how to enable account confirmation and password reset please visit http://go.microsoft.com/fwlink/?LinkID=532713
                // Send an email with this link
                //var code = await _userManager.GeneratePasswordResetTokenAsync(user);
                //var callbackUrl = Url.Action("ResetPassword", "Account", new { userId = user.Id, code = code }, protocol: HttpContext.Request.Scheme);
                //await _emailSender.SendEmailAsync(model.UserName, "Reset Password",
                //   $"Please reset your password by clicking here: <a href='{callbackUrl}'>link</a>");
                //return View("ForgotPasswordConfirmation");
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        //
        // GET: /Account/ForgotPasswordConfirmation
        [HttpGet]
        [AllowAnonymous]
        public IActionResult ForgotPasswordConfirmation()
        {
            return View();
        }

        //
        // GET: /Account/ResetPassword
        [HttpGet]
        [AllowAnonymous]
        public IActionResult ResetPassword(string code = null)
        {
            return code == null ? View("Error") : View();
        }

        //
        // POST: /Account/ResetPassword
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var user = await _userManager.FindByNameAsync(model.Email);
            if (user == null)
            {
                // Don't reveal that the user does not exist
                return RedirectToAction(nameof(AccountController.ResetPasswordConfirmation), "Account");
            }
            var result = await _userManager.ResetPasswordAsync(user, model.Code, model.Password);
            if (result.Succeeded)
            {
                return RedirectToAction(nameof(AccountController.ResetPasswordConfirmation), "Account");
            }
            AddErrors(result);
            return View();
        }

        //
        // GET: /Account/ResetPasswordConfirmation
        [HttpGet]
        [AllowAnonymous]
        public IActionResult ResetPasswordConfirmation()
        {
            return View();
        }

        //
        // GET: /Account/SendCode
        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult> SendCode(string returnUrl = null, bool rememberMe = false)
        {
            var user = await _signInManager.GetTwoFactorAuthenticationUserAsync();
            if (user == null)
            {
                return View("Error");
            }
            var userFactors = await _userManager.GetValidTwoFactorProvidersAsync(user);
            var factorOptions = userFactors.Select(purpose => new SelectListItem { Text = purpose, Value = purpose }).ToList();
            return View(new SendCodeViewModel { Providers = factorOptions, ReturnUrl = returnUrl, RememberMe = rememberMe });
        }

        //
        // POST: /Account/SendCode
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SendCode(SendCodeViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

            var user = await _signInManager.GetTwoFactorAuthenticationUserAsync();
            if (user == null)
            {
                return View("Error");
            }

            // Generate the token and send it
            var code = await _userManager.GenerateTwoFactorTokenAsync(user, model.SelectedProvider);
            if (string.IsNullOrWhiteSpace(code))
            {
                return View("Error");
            }

            var message = "Your security code is: " + code;
            if (model.SelectedProvider == "UserName")
            {
                await _emailSender.SendEmailAsync(await _userManager.GetEmailAsync(user), "Security Code", message);
            }
            else if (model.SelectedProvider == "Phone")
            {
                await _smsSender.SendSmsAsync(await _userManager.GetPhoneNumberAsync(user), message);
            }

            return RedirectToAction(nameof(VerifyCode), new { Provider = model.SelectedProvider, ReturnUrl = model.ReturnUrl, RememberMe = model.RememberMe });
        }

        //
        // GET: /Account/VerifyCode
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> VerifyCode(string provider, bool rememberMe, string returnUrl = null)
        {
            // Require that the user has already logged in via username/password or external login
            var user = await _signInManager.GetTwoFactorAuthenticationUserAsync();
            if (user == null)
            {
                return View("Error");
            }
            return View(new VerifyCodeViewModel { Provider = provider, ReturnUrl = returnUrl, RememberMe = rememberMe });
        }

        //
        // POST: /Account/VerifyCode
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> VerifyCode(VerifyCodeViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // The following code protects for brute force attacks against the two factor codes.
            // If a user enters incorrect codes for a specified amount of time then the user account
            // will be locked out for a specified amount of time.
            var result = await _signInManager.TwoFactorSignInAsync(model.Provider, model.Code, model.RememberMe, model.RememberBrowser);
            if (result.Succeeded)
            {
                return RedirectToLocal(model.ReturnUrl);
            }
            if (result.IsLockedOut)
            {
                _logger.LogWarning(7, "User account locked out.");
                return View("Lockout");
            }
            else
            {
                ModelState.AddModelError(string.Empty, "Invalid code.");
                return View(model);
            }
        }

        #region Helpers

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
        }

        private Task<ApplicationUser> GetCurrentUserAsync()
        {
            return _userManager.GetUserAsync(HttpContext.User);
        }

        private IActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            else
            {
                return RedirectToAction(nameof(HomeController.Index), "Home");
            }
        }

        #endregion
    }
}
