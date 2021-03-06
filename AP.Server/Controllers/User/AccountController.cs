﻿using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using AP.ViewModel.Account;
using AP.Core.Model.User;
using AP.Shared.Sender.Contracts;
using AP.ViewModel.Account.Manage;
using AP.Core.Model;
using AP.Shared.Security.Contracts;
using AutoMapper;
using Microsoft.Extensions.Localization;
using AP.Server.Controllers;

namespace WebApplication.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    public class AccountController : IdentityController
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IEmailSender _emailSender;
        private readonly ISmsSender _smsSender;
        private readonly ILogger<AccountController> logger;
        private readonly IIdentityProvider _identity;
        private readonly IAccountService accountService;
        private readonly IStringLocalizer<AccountController> localizer;

        private readonly string _externalCookieScheme;

        public AccountController(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            IOptions<IdentityCookieOptions> identityCookieOptions,
            IEmailSender emailSender,
            ISmsSender smsSender,
            IIdentityProvider identity,
            IAccountService accountService, 
            ILogger<AccountController> logger,
            IStringLocalizer<AccountController> localizer) : base(accountService)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _externalCookieScheme = identityCookieOptions.Value.ExternalCookieAuthenticationScheme;
            _emailSender = emailSender;
            _smsSender = smsSender;
            _identity = identity;
            this.accountService = accountService;
            this.localizer = localizer;
            this.logger = logger;
        }

        [HttpGet]
        [AllowAnonymous]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<string> Login(string returnUrl = null)
        {
            // Clear the existing external cookie to ensure a clean login process
            await HttpContext.Authentication.SignOutAsync(_externalCookieScheme);

            return "Please enter your credentials to login";
        }

        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IdentityStatus> Login([FromBody]LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                var loginInfo = Mapper.Map<LoginInfo>(model);
                return await _identity.SignIn(loginInfo);
            }

            return IdentityStatus.InvalidRequestBody;
        }

        [HttpPost("token")]
        [AllowAnonymous]
        public StatusCodeResult Token([FromBody] LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                var loginInfo = Mapper.Map<LoginInfo>(model);
                return Ok();
            }

            return NotFound();
        }

        [HttpPost("refresh-token")]
        [AllowAnonymous]
        public StatusCodeResult RefreshToken([FromBody] RefreshToken model)
        {
            return Ok();
        }

        [HttpPost("register-email")]
        [AllowAnonymous]
        public async Task<IdentityStatus> RegisterByEmail(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = new ApplicationUser { UserName = model.Email, Email = model.Email };
                var result = await _userManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                    var callbackUrl = Url.Action(nameof(ConfirmEmail), "Account",
                        new { userId = user.Id, code = code }, protocol: HttpContext.Request.Scheme);
                    await _emailSender.SendEmailAsync(model.Email, "Confirm your account",
                            $"Please confirm your account by clicking this link: <a href='{callbackUrl}'>link</a>");

                    // Comment out following line to prevent a new user automatically logged on.
                    // await _signInManager.SignInAsync(user, isPersistent: false);
                    logger.LogInformation(3, "User created a new account with password.");
                    return IdentityStatus.AddLoginSuccess;
                }
                AddErrors(result);
            }

            return IdentityStatus.Error;
        }

        [HttpPost("register-phone")]
        [AllowAnonymous]
        public async Task<VerifyPhoneNumberViewModel> RegisterByPhoneNumber([FromBody]RegisterMobileViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = new ApplicationUser
                {
                    UserName = model.Email,
                    Email = model.Email,
                    PhoneNumber = model.Phone,
                };

                if (!(model.Role == Roles.Client || model.Role == Roles.Master))
                {
                    throw new ArgumentException(localizer["InvalidClientTypeEx"].Value);
                }

                var result = await _userManager.CreateAsync(user, model.Password);

                if (result.Succeeded)
                {
                    logger.LogInformation($"User {user.UserName} has been created successfully.");

                    await accountService.AddRole(user, model.Role);
                    var code = await _userManager.GenerateChangePhoneNumberTokenAsync(user, model.Phone);
                    //TODO: uncomment before Production
#if DEBUG
#else
                    //await _smsSender.SendSmsAsync(model.Phone, "Your security code is: " + code);
#endif
                    logger.LogInformation($"Your verification code is: {code}");

                    var jwt = await accountService.RefreshJwt(user);
                    return new VerifyPhoneNumberViewModel
                    {
                        Code = code,
                        Phone = model.Phone,
                        AccessToken = jwt.AccessToken,
                        RefreshToken = jwt.RefreshToken
                    };
                }
                else
                {
                    var errorMessage = result.Errors.First().Description;
                    throw new InvalidOperationException(errorMessage);
                }
            }
            else
            {
                var invalidParams = string.Join("; ", ModelState.Keys);
                throw new ArgumentException($"Invalid JSON params are {invalidParams}");

                return new VerifyPhoneNumberViewModel
                {
                    Message = $"Invalid JSON params are {invalidParams}"
                };
            }

            return null;
        }

        [HttpGet("resend-code")]
        [Authorize(Roles="Client,Master")]
        public async Task<VerifyPhoneNumberViewModel> ResendVerifyCode(string phone)
        {
            if (string.IsNullOrEmpty(phone))
            {
                return new VerifyPhoneNumberViewModel
                {
                    Message = "Phone couldn't be empty",
                    Status = IdentityStatus.TwoFactorRequiresError
                };
            }
            var formattedPhone = "+" + phone.TrimStart();
            var user = await GetCurrentUser();
            var code = await _userManager.GenerateChangePhoneNumberTokenAsync(user, formattedPhone);
            //TODO: uncomment before Production
#if DEBUG
#else
            //await _smsSender.SendSmsAsync(model.Phone, "Your security code is: " + code);
#endif
            logger.LogInformation($"Your verification code is: {code}");

            var jwt = await accountService.RefreshJwt(user);
            return new VerifyPhoneNumberViewModel
            {
                Code = code,
                Phone = formattedPhone,
                AccessToken = jwt.AccessToken,
                RefreshToken = jwt.RefreshToken
            };
        }

        [HttpPost("verify-phone")]
        [Authorize(Roles = "Client,Master")]
        public async Task<JwtResponse> VerifyPhoneNumber([FromBody]VerifyPhoneNumberViewModel model)
        {
            var jwt = new JwtResponse
            {
                Status = IdentityStatus.Error
            };

            if (!ModelState.IsValid)
            {
                jwt.Message = "Invalid JSON model";
                return jwt;
            }

            var user = await GetCurrentUser();
            if (user != null)
            {
                var jwtResponse = await accountService.CompleteUserVerification(user, model.Phone, model.Code);
                if (jwtResponse != null)
                {
                    jwtResponse.Status = IdentityStatus.AddPhoneSuccess;
                    return jwtResponse;
                }
            }

            jwt.Message = "Failed to verify phone number";
            return jwt;
        }

        [HttpPost("logout")]
        public async Task Logout()
        {
            await _signInManager.SignOutAsync();
            logger.LogInformation(4, "User logged out.");
            //return RedirectToAction(nameof(HomeController.Index), "Home");
        }

        [HttpPost("external-login")]
        [AllowAnonymous]
        public IActionResult ExternalLogin(string provider, string returnUrl = null)
        {
            // Request a redirect to the external login provider.
            var redirectUrl = Url.Action(nameof(ExternalLoginCallback), "Account", new { ReturnUrl = returnUrl });
            var properties = _signInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl);
            return Challenge(properties, provider);
        }

        [HttpGet("external-login-callback")]
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
                logger.LogInformation(5, "User logged in with {Name} provider.", info.LoginProvider);
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

        [HttpPost("external-login-confirmation")]
        [AllowAnonymous]
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
                        logger.LogInformation(6, "User created an account using {Name} provider.", info.LoginProvider);
                        return RedirectToLocal(returnUrl);
                    }
                }
                AddErrors(result);
            }

            ViewData["ReturnUrl"] = returnUrl;
            return View(model);
        }

        [HttpGet("confirm-email")]
        [AllowAnonymous]
        public async Task<bool> ConfirmEmail(string userId, string code)
        {
            if (userId == null || code == null)
            {
                throw new ArgumentException("userId or code is null");
            }
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                throw new Exception("User wasn't found");
            }
            var result = await _userManager.ConfirmEmailAsync(user, code);
            return result.Succeeded;
        }

        [HttpGet("forgot-password")]
        [AllowAnonymous]
        public IActionResult ForgotPassword()
        {
            return View();
        }

        [HttpPost("forgot-password")]
        [AllowAnonymous]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(model.Email);
                if (user == null || !(await _userManager.IsEmailConfirmedAsync(user)))
                {
                    // Don't reveal that the user does not exist or is not confirmed
                    return View("ForgotPasswordConfirmation");
                }

                // For more information on how to enable account confirmation and password reset please visit https://go.microsoft.com/fwlink/?LinkID=532713
                // Send an email with this link
                //var code = await _userManager.GeneratePasswordResetTokenAsync(user);
                //var callbackUrl = Url.Action(nameof(ResetPassword), "Account", new { userId = user.Id, code = code }, protocol: HttpContext.Request.Scheme);
                //await _emailSender.SendEmailAsync(model.Email, "Reset Password",
                //   $"Please reset your password by clicking here: <a href='{callbackUrl}'>link</a>");
                //return View("ForgotPasswordConfirmation");
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        [HttpPost("reset-password")]
        [AllowAnonymous]
        public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                // Don't reveal that the user does not exist
                throw new ArgumentException("The user doesn't exist");
            }
            var result = await _userManager.ResetPasswordAsync(user, model.Code, model.Password);
            if (result.Succeeded)
            {
                return Ok();
            }
            AddErrors(result);
            return View();
        }

        [HttpGet("send-code")]
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

        [HttpPost("send-code")]
        [AllowAnonymous]
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
            if (model.SelectedProvider == "Email")
            {
                await _emailSender.SendEmailAsync(await _userManager.GetEmailAsync(user), "Security Code", message);
            }
            else if (model.SelectedProvider == "Phone")
            {
                await _smsSender.SendSmsAsync(await _userManager.GetPhoneNumberAsync(user), message);
            }

            return RedirectToAction(nameof(VerifyCode), new { Provider = model.SelectedProvider, ReturnUrl = model.ReturnUrl, RememberMe = model.RememberMe });
        }

        [HttpGet("verify-code")]
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

        [HttpPost("verify-code")]
        [AllowAnonymous]
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
                logger.LogWarning(7, "User account locked out.");
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

        private IActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            else
            {
                return null;
                //return RedirectToAction(nameof(HomeController.Index), "Home");
            }
        }
#endregion
    }
}
