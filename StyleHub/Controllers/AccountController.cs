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
            _apiBase = (cfg["ApiBaseUrl"] ?? "https://localhost:7158/").TrimEnd('/') + "/"; }

        private string? GetUserId() =>
            User?.FindFirst(ClaimTypes.NameIdentifier)?.Value
            ?? User?.FindFirst("sub")?.Value;

        private HttpClient NewClientWithUser()
        {
            var c = new HttpClient { BaseAddress = new Uri(_apiBase) };
            c.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            var uid = GetUserId();
            if (!string.IsNullOrWhiteSpace(uid))
                c.DefaultRequestHeaders.Add("X-User-Id", uid); // مطابق للـ API عندك
            return c;
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
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangePassword(string CurrentPassword, string NewPassword, string ConfirmPassword)
        {
            // تحقق بسيط
            if (string.IsNullOrWhiteSpace(CurrentPassword) ||
                string.IsNullOrWhiteSpace(NewPassword) ||
                string.IsNullOrWhiteSpace(ConfirmPassword))
            {
                ModelState.AddModelError(string.Empty, "من فضلك املأ جميع الحقول.");
                return RedirectBackWithModelState();
            }

            if (NewPassword != ConfirmPassword)
            {
                ModelState.AddModelError(string.Empty, "كلمة المرور الجديدة وتأكيدها غير متطابقين.");
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
            TempData["PwdMsg"] = "تم تحديث كلمة المرور بنجاح.";
            return Redirect(Request.Headers["Referer"].ToString());
        }

        // helper بسيط علشان نرجّع لنفس الصفحة ومعانا الأخطاء
        private IActionResult RedirectBackWithModelState()
        {
            // خزن الأخطاء في TempData (اختياري). أبسط بديل: اعرض الأخطاء داخل نفس View strongly-typed.
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
                return Challenge(); // لو مش لاقي المستخدم
            }

            // (اختياري) لو عايز تتأكد بكلمة المرور قبل الحذف
            // var check = await _userManager.CheckPasswordAsync(user, passwordFromForm);

            var result = await _userManager.DeleteAsync(user);
            if (!result.Succeeded)
            {
                TempData["StatusMessage"] = string.Join(" | ", result.Errors.Select(e => e.Description));
                return RedirectToAction("Settings"); // ارجع لنفس الصفحة لو حصل خطأ
            }

            await _signInManager.SignOutAsync(); // اعمل تسجيل خروج بعد الحذف
            return RedirectToAction("Index", "Home"); // رجّعه للهوم بيج
        }
        [Authorize]
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            using var http = NewClientWithUser();
          

            var addresses = await http.GetFromJsonAsync<List<AddressDto>>("api/addresses/mine") ?? new();
            var orders = await http.GetFromJsonAsync<List<Order>>("api/orders/mine") ?? new();

            // ممكن تعمل ViewModel، لكن مؤقتًا نستخدم ViewData
            ViewData["Orders"] = orders;
            // مهم: ابعت دايمًا list مش null
            return View(addresses ?? new List<AddressDto>());
        }
    }
}
