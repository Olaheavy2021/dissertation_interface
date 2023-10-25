namespace Shared.Helpers;

public class StringHelpers
{
    public static string ReplaceEmail(string text, string email) => text.Replace("{email}", email);
}