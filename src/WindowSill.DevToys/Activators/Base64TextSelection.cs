using System.ComponentModel.Composition;
using System.Text;
using System.Text.RegularExpressions;
using WindowSill.API;

namespace WindowSill.DevToys.Activators;

[Export(typeof(ISillTextSelectionActivator))]
[ActivationType(ActivationTypeName, PredefinedActivationTypeNames.TextSelection)]
internal sealed partial class Base64TextSelection : ISillTextSelectionActivator
{
    internal const string ActivationTypeName = "base64Selection";

    public ValueTask<bool> GetShouldBeActivatedAsync(string selectedText, bool isReadOnly, CancellationToken cancellationToken)
    {
        return ValueTask.FromResult(IsBase64DataStrict(selectedText.AsSpan()));
    }

    private static bool IsBase64DataStrict(ReadOnlySpan<char> data)
    {
        if (data.IsEmpty)
        {
            return false;
        }

        // Manually trim whitespace to avoid string allocation
        int start = 0;
        int end = data.Length - 1;

        while (start <= end && char.IsWhiteSpace(data[start]))
        {
            start++;
        }

        while (end >= start && char.IsWhiteSpace(data[end]))
        {
            end--;
        }

        if (start > end)
        {
            return false;
        }

        data = data.Slice(start, end - start + 1);

        if (data.Length % 4 != 0)
        {
            return false;
        }

        // Check for invalid Base64 characters
        foreach (char c in data)
        {
            if (!((c >= 'A' && c <= 'Z') ||
                  (c >= 'a' && c <= 'z') ||
                  (c >= '0' && c <= '9') ||
                  c == '+' || c == '/' || c == '='))
            {
                return false;
            }
        }

        int equalIndex = data.IndexOf('=');
        int length = data.Length;

        if (!(equalIndex == -1 || equalIndex == length - 1 || equalIndex == length - 2 && data[length - 1] == '='))
        {
            return false;
        }

        Span<byte> decodedData = stackalloc byte[((data.Length * 3) + 3) / 4];

        if (!Convert.TryFromBase64Chars(data, decodedData, out int bytesWritten))
        {
            return false;
        }

        ReadOnlySpan<byte> actualDecoded = decodedData.Slice(0, bytesWritten);

        // Validate UTF-8 without allocating string
        Span<char> charBuffer = stackalloc char[Encoding.UTF8.GetMaxCharCount(actualDecoded.Length)];

        if (!Encoding.UTF8.TryGetChars(actualDecoded, charBuffer, out int charsWritten))
        {
            return false;
        }

        ReadOnlySpan<char> decoded = charBuffer.Slice(0, charsWritten);

        // Check for special chars that should not be there
        foreach (char current in decoded)
        {
            if (current == 65533)
            {
                return false;
            }

            if (!(current == 0x9
                || current == 0xA
                || current == 0xD
                || current >= 0x20 && current <= 0xD7FF
                || current >= 0xE000 && current <= 0xFFFD
                || current >= 0x10000 && current <= 0x10FFFF))
            {
                return false;
            }
        }

        return true;
    }

    [GeneratedRegex("[^A-Z0-9+/=]", RegexOptions.IgnoreCase, "en-US")]
    private static partial Regex Base64Regex();
}
