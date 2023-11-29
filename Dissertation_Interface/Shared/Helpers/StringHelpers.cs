namespace Shared.Helpers;

public class StringHelpers
{
    public static string ReplaceEmail(string text, string email) => text.Replace("{email}", email);

    public static string CapitalizeFirstLetterAndLowercaseRest(string word)
    {
        if (string.IsNullOrEmpty(word))
        {
            return word;
        }

        var lowercasedWord = word.ToLower();
        var letters = lowercasedWord.ToCharArray();
        letters[0] = char.ToUpper(letters[0]);

        return new string(letters);
    }
}