using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using StyleHub.Models;
using System.Net.Http.Headers;
using System.Security.Claims;

namespace StyleHub.Controllers
{
    public class AccountController : Controller
    {
        private readonly string _apiBase;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;

        public AccountController(
           UserManager<IdentityUser> userManager,
           IConfiguration cfg,
           SignInManager<IdentityUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _apiBase = (cfg["ApiBaseUrl"] ?? "https://stylehubteamde.runasp.net/").TrimEnd('/') + "/";
        }

        private string? GetUserId() =>
            User?.FindFirst(ClaimTypes.NameIdentifier)?.Value
            ?? User?.FindFirst("sub")?.Value;

        private HttpClient NewClientWithUser()
        {
            var c = new HttpClient { BaseAddress = new Uri(_apiBase) };
            c.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            var uid = GetUserId();
            if (!string.IsNullOrWhiteSpace(uid))
                c.DefaultRequestHeaders.Add("X-User-Id", uid);
            return c;
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateProfile(ProfileVm model)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Challenge();

            if (!string.Equals(user.Email, model.Email, StringComparison.OrdinalIgnoreCase))
            {
                user.Email = model.Email;
                user.UserName = model.Email;
            }

            if (user.PhoneNumber != model.PhoneNumber)
                user.PhoneNumber = model.PhoneNumber;

            var result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded)
            {
                TempData["StatusMessage"] = string.Join(" | ", result.Errors.Select(e => e.Description));
                return Redirect(Request.Headers["Referer"].ToString());
            }

            await UpsertClaimAsync(user, "given_name", model.FirstName ?? string.Empty);
            await UpsertClaimAsync(user, "family_name", model.LastName ?? string.Empty);
            await UpsertClaimAsync(user, "name", $"{model.FirstName} {model.LastName}".Trim());

            await _signInManager.RefreshSignInAsync(user);

            TempData["StatusMessage"] = "Changes were saved successfully.";
            return RedirectToAction("Index");
        }

        private async Task UpsertClaimAsync(IdentityUser user, string type, string value)
        {
            var existing = (await _userManager.GetClaimsAsync(user)).FirstOrDefault(c => c.Type == type);
            if (existing == null)
            {
                if (!string.IsNullOrEmpty(value))
                    await _userManager.AddClaimAsync(user, new Claim(type, value));
            }
            else if (existing.Value != value)
            {
                await _userManager.ReplaceClaimAsync(user, existing, new Claim(type, value));
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangePassword(string CurrentPassword, string NewPassword, string ConfirmPassword)
        {
            if (string.IsNullOrWhiteSpace(CurrentPassword) ||
                string.IsNullOrWhiteSpace(NewPassword) ||
                string.IsNullOrWhiteSpace(ConfirmPassword))
            {
                ModelState.AddModelError(string.Empty, "Please fill in all fields.");
                return RedirectBackWithModelState();
            }

            if (NewPassword != ConfirmPassword)
            {
                ModelState.AddModelError(string.Empty, "The new password and confirmation do not match.");
                return RedirectBackWithModelState();
            }

            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Challenge();

            var result = await _userManager.ChangePasswordAsync(user, CurrentPassword, NewPassword);
            if (!result.Succeeded)
            {
                foreach (var e in result.Errors)
                    ModelState.AddModelError(string.Empty, e.Description);

                return RedirectBackWithModelState();
            }

            await _signInManager.RefreshSignInAsync(user);
            TempData["PwdMsg"] = "Your password has been updated successfully.";
            return Redirect(Request.Headers["Referer"].ToString());
        }

        private IActionResult RedirectBackWithModelState()
        {
            TempData["PwdMsg"] = string.Join(" | ",
                ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage));

            return Redirect(Request.Headers["Referer"].ToString());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteAccount()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return Challenge();
            }

            var result = await _userManager.DeleteAsync(user);
            if (!result.Succeeded)
            {
                TempData["StatusMessage"] = string.Join(" | ", result.Errors.Select(e => e.Description));
                return RedirectToAction("Settings");
            }

            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            using var http = NewClientWithUser();

            var addresses = await http.GetFromJsonAsync<List<AddressDto>>("api/addresses/mine") ?? new();
            var orders = await http.GetFromJsonAsync<List<Order>>("api/orders/mine") ?? new();

            ViewData["Orders"] = orders;
            return View(addresses ?? new List<AddressDto>());
        }
    }
}