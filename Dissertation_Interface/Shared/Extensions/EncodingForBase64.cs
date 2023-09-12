namespace Shared.Extensions;

public static class EncodingForBase64
{
    public static string? EncodeBase64(this System.Text.Encoding encoding, string? text)
    {
        if (text == null)
        {
            return string.Empty;
        }

        var textAsBytes = encoding.GetBytes(text);
        return Convert.ToBase64String(textAsBytes);
    }

    public static string DecodeBase64(this System.Text.Encoding encoding, string? encodedText)
    {
        if (encodedText == null)
        {
            return string.Empty;
        }

        var textAsBytes = Convert.FromBase64String(encodedText);
        return encoding.GetString(textAsBytes);
    }
}