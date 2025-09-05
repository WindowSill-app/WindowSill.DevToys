using System.Text.Json;
using Microsoft.Extensions.Logging;
using WindowSill.API;

namespace WindowSill.DevToys.Core;

internal static partial class JsonHelper
{
    private static readonly ILogger logger = typeof(JsonHelper).Log();

    /// <summary>
    /// Format a string to the specified JSON format.
    /// </summary>
    internal static string Format(string? input, Indentation indentationMode)
    {
        if (string.IsNullOrWhiteSpace(input))
        {
            return string.Empty;
        }

        System.Text.StringBuilder stringBuilder = PooledStringBuilder.Instance.Get();

        try
        {
            // Parse the JSON input
            using var document = JsonDocument.Parse(input);

            using var stream = new MemoryStream();
            var writerOptions = new JsonWriterOptions();

            writerOptions.Indented = indentationMode switch
            {
                Indentation.TwoSpaces => true,
                Indentation.FourSpaces => true,
                Indentation.OneTab => true,
                Indentation.Minified => false,
                _ => throw new NotSupportedException(),
            };

            using (var writer = new Utf8JsonWriter(stream, writerOptions))
            {
                document.RootElement.WriteTo(writer);
            }

            string jsonString = System.Text.Encoding.UTF8.GetString(stream.ToArray());

            // Handle custom indentation for spaces and tabs
            if (indentationMode == Indentation.TwoSpaces)
            {
                jsonString = jsonString.Replace("  ", "  ");
            }
            else if (indentationMode == Indentation.FourSpaces)
            {
                jsonString = jsonString.Replace("  ", "    ");
            }
            else if (indentationMode == Indentation.OneTab)
            {
                jsonString = jsonString.Replace("  ", "\t");
            }

            return jsonString;
        }
        catch (JsonException ex)
        {
            return string.Empty;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Invalid JSON format '{indentationMode}'", indentationMode);
            return string.Empty;
        }
        finally
        {
            PooledStringBuilder.Instance.Return(stringBuilder);
        }
    }
}
