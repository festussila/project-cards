using System.Net;
using System.Text;

using Core.Domain.Exceptions;

namespace Core.Domain.Common;

public static class ObjectExtensions
{
    public static void IsDefinedEnum(this object? value, Type type, string parameter)
    {
        Enum.TryParse(type, value?.ToString(), true, out value);

        if (!Enum.IsDefined(type, value ?? string.Empty)) throw new GenericException($"Invalid {parameter} provided", "CA004", HttpStatusCode.BadRequest);
    }

    public static long ToEpoch(this DateTime value)
    {
        return new DateTimeOffset(value).ToUnixTimeSeconds();
    }
    public static string RemoveWhitespaces(this string value)
    {
        StringBuilder builder = new(value.Length);

        for (int i = 0; i < value.Length; i++)
        {
            if (!char.IsWhiteSpace(value[i])) builder.Append(value[i]);
        }

        return value.Length == builder.Length ? value : builder.ToString();
    }
}