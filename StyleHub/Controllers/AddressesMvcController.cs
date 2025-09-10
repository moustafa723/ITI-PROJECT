using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StyleHub.Models;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Security.Claims;
[Authorize]

public class AddressesMvcController : Controller
{
    private readonly string _apiBase;

    public AddressesMvcController(IConfiguration config)
    {
        _apiBase = (config["ApiBaseUrl"] ?? "https://localhost:7158")
                     .TrimEnd('/') + "/";
    }


    private HttpClient NewClientWithUser()
    {
        var c = new HttpClient { BaseAddress = new Uri(_apiBase) };
        var uid = User?.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value
                  ?? User?.FindFirst("sub")?.Value;
        if (!string.IsNullOrWhiteSpace(uid))
            c.DefaultRequestHeaders.Add("X-User-Id", uid);
        return c;

    }


    [HttpGet]
    public async Task<IActionResult> Index()
    {
        using var http = NewClientWithUser();
        var list = await http.GetFromJsonAsync<List<AddressDto>>("api/addresses/mine");
        return View(list ?? new List<AddressDto>());
    }

    [HttpGet]
    public IActionResult CreateInline()
    {
        return PartialView("_CreateForm", new AddressDto());
    }
   [HttpPost]
[ValidateAntiForgeryToken]
public async Task<IActionResult> Create(AddressDto dto)
{
    if (!ModelState.IsValid)
    {
        // عشان الأخطاء تبان في الفورم، حط ValidationSummary في الـ Partial (البند 3)
        return PartialView("_CreateForm", dto);
    }

    using var http = NewClientWithUser();
    var res = await http.PostAsJsonAsync("api/addresses/mine", dto);

    if (!res.IsSuccessStatusCode)
    {
        var err = await res.Content.ReadAsStringAsync();
        // رجّع كود الحالة ونص الخطأ — الجافاسكريبت عندك هيطلع alert
        return StatusCode((int)res.StatusCode, err);
    }

    // نجاح → رجّع الجريد
    return await ListPartial();
}

    [HttpGet]
    public async Task<IActionResult> ListPartial()
    {
        using var http = NewClientWithUser();
        var list = await http.GetFromJsonAsync<List<AddressDto>>("api/addresses/mine") ?? new();
        return PartialView("_List", list);
    }

    [HttpGet]
    public async Task<IActionResult> EditInline(int id)
    {
        using var http = NewClientWithUser();

        // لو عندك endpoint منفصل هاته مباشرة
        // var dto = await http.GetFromJsonAsync<AddressDto>($"api/addresses/mine/{id}");

        var all = await http.GetFromJsonAsync<List<AddressDto>>("api/addresses/mine") ?? new();
        var dto = all.FirstOrDefault(a => a.Id == id);
        if (dto == null) return NotFound();

        return PartialView("_EditForm", dto);
    }


    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(AddressDto dto)
    {
        if (!dto.Id.HasValue) return NotFound();
        if (!ModelState.IsValid) return View(dto);

        using var http = NewClientWithUser();
        var res = await http.PutAsJsonAsync($"api/addresses/mine/{dto.Id}", dto);
        if (!res.IsSuccessStatusCode)
        {
            ModelState.AddModelError("", $"API error: {(int)res.StatusCode}");
            return View(dto);
        }

        TempData["Msg"] = "Address updated.";
        return await ListPartial();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        using var http = NewClientWithUser();
        var res = await http.DeleteAsync($"api/addresses/mine/{id}");
        if (!res.IsSuccessStatusCode)
            TempData["Msg"] = $"API error: {(int)res.StatusCode}";
        else
            TempData["Msg"] = "Address removed.";

        return await ListPartial();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> MakeDefault(int id)
    {
        using var http = NewClientWithUser();
        var res = await http.PostAsync($"api/addresses/mine/{id}/default", content: null);
        TempData["Msg"] = res.IsSuccessStatusCode ? "Default address set." : $"API error: {(int)res.StatusCode}";
        return await ListPartial();
    }
}
