using Ganss.Xss;

namespace Dissertation.Application.Utility;

public class HtmlSanitizerService : IHtmlSanitizerService
{
    private readonly HtmlSanitizer _sanitizer = new();

    // Configure the sanitizer as needed
    public string Sanitize(string html) => this._sanitizer.Sanitize(html);
}