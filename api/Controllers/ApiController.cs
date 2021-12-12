using api.Configurations;
using api.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace api.Controllers;


public class Request
{
    public string Url { get; set; }
}

public class Response
{
    public string ShortUrl { get; set; }
}

[ApiController]
[Route("[controller]/[action]")]
public class ApiController : ControllerBase
{
    private readonly UrlShorteningServiceConfiguration config;
    private readonly UrlShorteningService urlShorterService;

    public ApiController(IOptions<UrlShorteningServiceConfiguration> config,UrlShorteningService urlShorterService)
    {
        this.config = config.Value;
        this.urlShorterService = urlShorterService;
    }

    [HttpPut]
    public async Task<Response> Shorten(Request request)
    {
        var shortSlug = await urlShorterService.CreateShortSlugFor(request.Url);        
        Uri finalUri = new (new Uri(config.ServerBaseUrl), shortSlug);
        return new Response() { ShortUrl = finalUri.AbsoluteUri };
    }
}
