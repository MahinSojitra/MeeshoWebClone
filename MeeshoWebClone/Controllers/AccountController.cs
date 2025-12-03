using MeeshoWebClone.Data;
using MeeshoWebClone.Enums;
using MeeshoWebClone.Models;
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

        public AccountController(UserManager<User> userManager, SignInManager<User> signInManager, MeeshoAppDbContext context)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _context = context;
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
                PhoneNumber = model.PhoneNumber
            };

            var result = await _userManager.CreateAsync(user, model.Password!);

            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(user, model.Role);

                TempData["VerificationSource"] = "Signup";

                return RedirectToAction("Verify", "Account");
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

                if (user.Status == VerificationStatus.Pending)
                {
                    ModelState.AddModelError("Email", "Your account is pending verification.");
                    ViewBag.LastLoginMethod = "email";
                    return View(model);
                }
                else if (user.Status == VerificationStatus.Rejected)
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

                if (user.Status == VerificationStatus.Pending)
                {
                    ModelState.AddModelError("PhoneNumber", "Your account is pending verification.");
                    ViewBag.LastLoginMethod = "phone";
                    return View(model);
                }
                else if (user.Status == VerificationStatus.Rejected)
                {
                    ModelState.AddModelError("PhoneNumber", "Your account is rejected. reach out to support.");
                    ViewBag.LastLoginMethod = "phone";
                    return View(model);
                }

                await _signInManager.SignInAsync(user, model.RememberMe);

                TempData["VerificationSource"] = "Login";

                return RedirectToAction("Verify", "Account");
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

                // In a production environment, you would send this link via email
                // For demonstration purposes, we'll store it in TempData
                TempData["ResetLink"] = resetLink;
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
