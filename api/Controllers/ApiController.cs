using Microsoft.AspNetCore.Mvc;

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
    

    [HttpPut]
    public async Task<Response> Shorten(Request request)
    {
        return new Response { ShortUrl = "shorturl" };
    }
}
