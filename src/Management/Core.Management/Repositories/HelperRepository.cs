using System.Net;
using System.Text.RegularExpressions;

using Core.Domain.Exceptions;

namespace Core.Management.Repositories;

public partial class HelperRepository
{

    public static void ValidatedParameter(string parameter, string? value, out string result, bool throwException = false)
    {
        result = value?.Trim() ?? string.Empty;
        if (result.Length < 1 && throwException)
            throw new GenericException($"{parameter} must be provided to complete this request", "CA003", HttpStatusCode.BadRequest);
    }

    /// <summary>
    /// Validates hexadecimal color code.  Throws exception if not valid code by default otherwise specified boolean  
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public static bool IsValidHexColorCode(string value, bool throwException = false)
    {  
        Regex regex = HexColorCodeRegex();

        // Check if the input string matches the regex pattern.
        bool matches = regex.IsMatch(value);

        if(!matches && throwException)
            throw new GenericException($"Invalid color code '{value}' provided", "CA004", HttpStatusCode.BadRequest);

        return matches;
    }

    [GeneratedRegex(@"^#[a-fA-F0-9]{6}$")]
    private static partial Regex HexColorCodeRegex();
}