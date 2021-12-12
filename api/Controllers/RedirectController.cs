using api.Configurations;
using api.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace api.Controllers;




[Route("/")]
public class RedirectController : Controller
{
    private readonly string clientUrl;
    private readonly UrlShorteningService urlShorterService;

    public RedirectController(IOptions<UrlShorteningServiceConfiguration> config, UrlShorteningService urlShorterService)
    {
        this.clientUrl = config.Value.ClientUrl;
        this.urlShorterService = urlShorterService;
    }

    [HttpGet("{slug}")]
    public async Task<IActionResult> RedirectSlug(string slug)
    {
        if (!string.IsNullOrWhiteSpace(slug))
        {
            var longUrl = await urlShorterService.GetLongUrlForSlug(slug);
            if (longUrl is null) return NotFound();
            return Redirect(longUrl);
        }
        return NotFound(); // this should never happen
    }

    [HttpGet]
    public async Task<IActionResult> RedirectToClient(string slug)
    {
        return Redirect(clientUrl);
    }


}
