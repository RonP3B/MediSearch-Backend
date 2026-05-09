using System.Globalization;
using System.Security.Cryptography;
using System.Text;

namespace MediSearch.Core.Application.Users.Commands.RegisterCompanyUser;

internal static class CompanyUserCredentialsGenerator
{
    /// <summary>
    /// Generates a unique username from first and last names with a random suffix.
    /// </summary>
    /// <param name="firstName">The user's first name.</param>
    /// <param name="lastName">The user's last name.</param>
    /// <returns>
    /// A username composed of up to 3 characters from each name plus an 8-character suffix,
    /// truncated to a maximum of 20 characters.
    /// </returns>
    public static string GenerateUsername(string firstName, string lastName)
    {
        string suffix = Guid.NewGuid().ToString("N")[..8];
        firstName = SanitizeName(firstName)[..Math.Min(3, SanitizeName(firstName).Length)];
        lastName = SanitizeName(lastName)[..Math.Min(3, SanitizeName(lastName).Length)];
        string username = $"{firstName}{lastName}_{suffix}";
        return username[..Math.Min(20, username.Length)];
    }

    /// <summary>
    /// Generates a cryptographically secure temporary password with mixed character types.
    /// </summary>
    /// <returns>
    /// An 8-character password containing at least one lowercase letter, one uppercase letter,
    /// one digit, and one special character in randomized order.
    /// </returns>
    public static string GenerateTempPassword()
    {
        int length = 8;
        string specials = "!@#$%^&()_+-=[]{}|;:,.<>?";

        Func<int, int> rnd = RandomNumberGenerator.GetInt32;

        var pwd = new char[length];
        int pos = 0;

        pwd[pos++] = (char)('a' + rnd(26));
        pwd[pos++] = (char)('A' + rnd(26));
        pwd[pos++] = (char)('0' + rnd(10));
        pwd[pos++] = specials[rnd(specials.Length)];

        while (pos < length)
        {
            pwd[pos++] = rnd(4) switch
            {
                0 => (char)('a' + rnd(26)),
                1 => (char)('A' + rnd(26)),
                2 => (char)('0' + rnd(10)),
                _ => specials[rnd(specials.Length)],
            };
        }

        for (int i = length - 1; i > 0; i--)
        {
            int j = rnd(i + 1);
            (pwd[j], pwd[i]) = (pwd[i], pwd[j]);
        }

        return new string(pwd);
    }

    private static string SanitizeName(string name)
    {
        string normalized = name.Normalize(NormalizationForm.FormD);

        var clean = normalized
            .Where(c =>
                CharUnicodeInfo.GetUnicodeCategory(c) != UnicodeCategory.NonSpacingMark
                && c is >= 'a' and <= 'z' or >= 'A' and <= 'Z'
            )
            .ToArray();

        return new string(clean).ToLower();
    }
}
