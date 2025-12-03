using MeeshoWebClone.Data;
using MeeshoWebClone.Enums;
using MeeshoWebClone.Models;
using MeeshoWebClone.Services;
using MeeshoWebClone.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MeeshoWebClone.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly MeeshoAppDbContext _context;
        private readonly IEmailService _emailService;
        private readonly ILogger<AccountController> _logger;

        public AccountController(UserManager<User> userManager, SignInManager<User> signInManager, MeeshoAppDbContext context, IEmailService emailService, ILogger<AccountController> logger)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _context = context;
            _emailService = emailService;
            _logger = logger;
        }

        public IActionResult Signup()
        {
            if (_signInManager.IsSignedIn(User))
            {
                return RedirectToAction("Index", "Home");
            }

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Signup(SignupViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var existingUserByEmail = await _userManager.FindByEmailAsync(model.Email!);
            if (existingUserByEmail != null)
            {
                ModelState.AddModelError("Email", "Email already in use.");
                return View(model);
            }

            var existingUserByPhone = await _userManager.Users.FirstOrDefaultAsync(u => u.PhoneNumber == model.PhoneNumber);
            if (existingUserByPhone != null)
            {
                ModelState.AddModelError("PhoneNumber", "Phone number is already in use.");
                return View(model);
            }

            var user = new User
            {
                UserName = model.Email,
                Email = model.Email,
                PhoneNumber = model.PhoneNumber,
                Status = VerificationStatus.Approved // Set as approved, email verification will be used instead
            };

            var result = await _userManager.CreateAsync(user, model.Password!);

            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(user, model.Role);

                // Generate email confirmation token and send verification email
                var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                var confirmationLink = Url.Action("ConfirmEmail", "Account",
                    new { userId = user.Id, token = token }, Request.Scheme);

                if (string.IsNullOrEmpty(confirmationLink))
                {
                    _logger.LogError("Failed to generate confirmation link for user {Email}", user.Email);
                    ModelState.AddModelError("", "Failed to create account. Please try again.");
                    // Delete the created user since we can't send verification email
                    await _userManager.DeleteAsync(user);
                    return View(model);
                }

                try
                {
                    await _emailService.SendEmailConfirmationAsync(user.Email!, confirmationLink);
                    _logger.LogInformation("Email confirmation sent successfully to {Email}", user.Email);
                    return RedirectToAction("Login", "Account", new { emailVerificationSent = true });
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Failed to send email confirmation to {Email}", user.Email);
                    // Delete the created user since email sending failed
                    await _userManager.DeleteAsync(user);
                    ModelState.AddModelError("", "Failed to send verification email. Please try again or check your email address.");
                    return View(model);
                }
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error.Description);
            }

            return View(model);
        }

        public IActionResult Login()
        {
            if (_signInManager.IsSignedIn(User))
            {
                return RedirectToAction("Index", "Home");
            }

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model, string LoginMethod)
        {
            User? user = null;

            if (LoginMethod == "email" && !string.IsNullOrEmpty(model.Email) && !string.IsNullOrEmpty(model.Password))
            {
                user = await _userManager.FindByEmailAsync(model.Email);
                if (user == null)
                {
                    ModelState.AddModelError("Email", "No account found with this email.");
                    ViewBag.LastLoginMethod = "email";
                    return View(model);
                }

                if (user.IsDeleted)
                {
                    ModelState.AddModelError("Email", "Your account no longer exists or deleted.");
                    ViewBag.LastLoginMethod = "email";
                    return View(model);
                }

                // Check if email is verified
                if (!user.EmailConfirmed)
                {
                    ModelState.AddModelError("Email", "Please verify your email address before logging in. Check your inbox for the verification link.");
                    ViewBag.LastLoginMethod = "email";
                    return View(model);
                }

                if (user.Status == VerificationStatus.Rejected)
                {
                    ModelState.AddModelError("Email", "Your account is rejected. reach out to support.");
                    ViewBag.LastLoginMethod = "email";
                    return View(model);
                }

                var result = await _signInManager.PasswordSignInAsync(user, model.Password!, model.RememberMe, true);
                if (result.Succeeded)
                {
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    ModelState.AddModelError("Password", "Incorrect password.");
                    ViewBag.LastLoginMethod = "email";
                    return View(model);
                }
            }
            else if (LoginMethod == "phone" && !string.IsNullOrEmpty(model.PhoneNumber))
            {
                user = await _userManager.Users.FirstOrDefaultAsync(u => u.PhoneNumber == model.PhoneNumber);
                if (user == null)
                {
                    ModelState.AddModelError("PhoneNumber", "No account found with this phone number.");
                    ViewBag.LastLoginMethod = "phone";
                    return View(model);
                }

                if (user.IsDeleted)
                {
                    ModelState.AddModelError("PhoneNumber", "Your account no longer exists or deleted.");
                    ViewBag.LastLoginMethod = "phone";
                    return View(model);
                }

                // Check if email is verified
                if (!user.EmailConfirmed)
                {
                    ModelState.AddModelError("PhoneNumber", "Please verify your email address before logging in. Check your inbox for the verification link.");
                    ViewBag.LastLoginMethod = "phone";
                    return View(model);
                }

                if (user.Status == VerificationStatus.Rejected)
                {
                    ModelState.AddModelError("PhoneNumber", "Your account is rejected. reach out to support.");
                    ViewBag.LastLoginMethod = "phone";
                    return View(model);
                }

                await _signInManager.SignInAsync(user, model.RememberMe);
                return RedirectToAction("Index", "Home");
            }

            return View(model);
        }

        public IActionResult Verify()
        {
            if (TempData["VerificationSource"] == null)
            {
                return RedirectToAction("Login", "Account");
            }

            return View();
        }

        [HttpPost]
        public IActionResult Verify(OTPViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            if (model.OTP == 1234)
            {
                string? source = TempData["VerificationSource"] as string;

                if (source == "Signup")
                {
                    return RedirectToAction("Login", "Account", new { accountCreated = true });
                }
                else
                {
                    return RedirectToAction("Index", "Home");
                }
            }

            ModelState.AddModelError("OTP", "Invalid OTP.");
            return View(model);
        }

        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Login", "Account", new { signedOut = true });
        }

        public IActionResult ConfirmDelete()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> DeleteAccount()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return RedirectToAction("Login");
            }

            user.IsDeleted = true;
            await _userManager.UpdateAsync(user);

            await _signInManager.SignOutAsync();
            return RedirectToAction("Login", "Account", new { deleted = true });
        }

        public IActionResult AccessDenied()
        {
            return View();
        }

        public async Task<IActionResult> ConfirmEmail(string? userId, string? token)
        {
            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(token))
            {
                return RedirectToAction("Login", new { emailConfirmationFailed = true });
            }

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return RedirectToAction("Login", new { emailConfirmationFailed = true });
            }

            var result = await _userManager.ConfirmEmailAsync(user, token);
            if (result.Succeeded)
            {
                _logger.LogInformation("Email confirmed successfully for user {Email}", user.Email);
                return RedirectToAction("Login", new { emailConfirmed = true });
            }

            _logger.LogWarning("Email confirmation failed for user {Email}", user.Email);
            return RedirectToAction("Login", new { emailConfirmationFailed = true });
        }

        public IActionResult ForgotPassword()
        {
            if (_signInManager.IsSignedIn(User))
            {
                return RedirectToAction("Index", "Home");
            }

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = await _userManager.FindByEmailAsync(model.Email!);
            if (user != null && !user.IsDeleted)
            {
                var token = await _userManager.GeneratePasswordResetTokenAsync(user);

                var resetLink = Url.Action("ResetPassword", "Account",
                    new { email = user.Email, token = token }, Request.Scheme);

                // Send password reset email
                try
                {
                    await _emailService.SendPasswordResetEmailAsync(user.Email!, resetLink!);
                    _logger.LogInformation("Password reset email sent successfully to {Email}", user.Email);
                }
                catch (Exception ex)
                {
                    // Log the error but don't expose it to the user to prevent email enumeration attacks
                    _logger.LogError(ex, "Failed to send password reset email to {Email}", user.Email);
                }
            }

            // Always redirect to prevent email enumeration attacks
            return RedirectToAction("ForgotPassword", new { emailSent = true });
        }

        public IActionResult ResetPassword(string? email, string? token)
        {
            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(token))
            {
                return RedirectToAction("Login");
            }

            var model = new ResetPasswordViewModel
            {
                Email = email,
                Token = token
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = await _userManager.FindByEmailAsync(model.Email!);
            if (user == null)
            {
                // Don't reveal that the user does not exist
                return RedirectToAction("Login", new { passwordReset = true });
            }

            var result = await _userManager.ResetPasswordAsync(user, model.Token!, model.Password!);
            if (result.Succeeded)
            {
                return RedirectToAction("Login", new { passwordReset = true });
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error.Description);
            }

            return View(model);
        }
    }
}
