using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using StyleHub.Models;

namespace StyleHub.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;

        public AccountController(
            UserManager<IdentityUser> userManager,
            SignInManager<IdentityUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }
        [Authorize]

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateProfile(ProfileVm model)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Challenge();

            // تحديث الإيميل (ولو بتستخدمه كـ UserName)
            if (!string.Equals(user.Email, model.Email, StringComparison.OrdinalIgnoreCase))
            {
                user.Email = model.Email;
                user.UserName = model.Email; // احذف السطر لو مش عايز المساواة
            }

            // تحديث الهاتف
            if (user.PhoneNumber != model.PhoneNumber)
                user.PhoneNumber = model.PhoneNumber;

            var result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded)
            {
                TempData["StatusMessage"] = string.Join(" | ", result.Errors.Select(e => e.Description));
                // ارجع لنفس الصفحة اللي فيها التابات
                return Redirect(Request.Headers["Referer"].ToString());
            }


            // Upsert للـ claims (first/last/full name)
            await UpsertClaimAsync(user, "given_name", model.FirstName ?? string.Empty);
            await UpsertClaimAsync(user, "family_name", model.LastName ?? string.Empty);
            await UpsertClaimAsync(user, "name", $"{model.FirstName} {model.LastName}".Trim());

            // تحديث الكوكي عشان القيم الجديدة تظهر فورًا
            await _signInManager.RefreshSignInAsync(user);

            TempData["StatusMessage"] = "تم حفظ التغييرات بنجاح.";
            return RedirectToAction("Index");
        }

        private async Task UpsertClaimAsync(IdentityUser user, string type, string value)
        {
            var existing = (await _userManager.GetClaimsAsync(user)).FirstOrDefault(c => c.Type == type);
            if (existing == null)
            {
                if (!string.IsNullOrEmpty(value))
                    await _userManager.AddClaimAsync(user, new System.Security.Claims.Claim(type, value));
            }
            else if (existing.Value != value)
            {
                await _userManager.ReplaceClaimAsync(user, existing, new System.Security.Claims.Claim(type, value));
            }
        }

        public IActionResult Index()
        {
            return View();
        }
    }
}
