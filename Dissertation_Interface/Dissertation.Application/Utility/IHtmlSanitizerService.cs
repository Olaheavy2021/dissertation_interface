namespace Dissertation.Application.Utility;

public interface IHtmlSanitizerService
{
    string Sanitize(string html);
}