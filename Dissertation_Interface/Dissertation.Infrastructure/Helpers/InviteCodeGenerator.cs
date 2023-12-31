using System.Text;

namespace Dissertation.Infrastructure.Helpers;

public static class InviteCodeGenerator
{
    public static string GenerateCode(int length)
    {
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
        var random = new Random();
        var code = new StringBuilder();

        for (var i = 0; i < length; i++)
        {
            code.Append(chars[random.Next(chars.Length)]);
        }

        return code.ToString();
    }
}