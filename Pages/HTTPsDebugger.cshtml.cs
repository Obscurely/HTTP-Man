using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace HTTPMan.Pages;

public class HTTPsDebuggerModel : PageModel
{
    private readonly ILogger<HTTPsDebuggerModel> _logger;

    public HTTPsDebuggerModel(ILogger<HTTPsDebuggerModel> logger)
    {
        _logger = logger;
    }

    public void OnGet()
    {

    }
}
