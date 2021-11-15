using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace HTTPMan.Pages;

public class HTTPsRequesterModel : PageModel
{
    private readonly ILogger<HTTPsRequesterModel> _logger;

    public HTTPsRequesterModel(ILogger<HTTPsRequesterModel> logger)
    {
        _logger = logger;
    }

    public void OnGet()
    {

    }
}
