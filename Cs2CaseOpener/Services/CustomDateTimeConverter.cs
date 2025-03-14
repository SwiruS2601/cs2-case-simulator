using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Cs2CaseOpener.Services;

public class CustomDateTimeConverter : JsonConverter<DateTime?>
{
    private readonly string[] formats = ["yyyy-MM-dd", "yyyy-MM-ddTHH:mm:ssZ", "O"];

    public override DateTime? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if(reader.TokenType == JsonTokenType.Null)
            return null;

        if(reader.TokenType == JsonTokenType.String)
        {
            string? dateString = reader.GetString();
            if(DateTime.TryParseExact(dateString, formats, CultureInfo.InvariantCulture,
                DateTimeStyles.AssumeUniversal | DateTimeStyles.AdjustToUniversal, out DateTime dt))
            {
                return dt;
            }
            if(DateTime.TryParse(dateString, out dt))
            {
                return dt;
            }
        }
        throw new JsonException();
    }

    public override void Write(Utf8JsonWriter writer, DateTime? value, JsonSerializerOptions options)
    {
        if(value.HasValue)
            writer.WriteStringValue(value.Value.ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ssZ"));
        else
            writer.WriteNullValue();
    }
}
