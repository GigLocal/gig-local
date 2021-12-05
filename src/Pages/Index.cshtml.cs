namespace GigLocal.Pages;

public class IndexModel : PageModel
{
    public void OnGet()
    {

    }

    public async Task<IActionResult> OnPostLogoutAsync()
    {
        await HttpContext.SignOutAsync();
        return RedirectToPage();
    }
}
