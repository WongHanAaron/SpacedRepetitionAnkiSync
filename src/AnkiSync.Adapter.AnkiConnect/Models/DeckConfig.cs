using System.Text.Json;
using System.Text.Json.Serialization;

namespace AnkiSync.Adapter.AnkiConnect.Models;

/// <summary>
/// Request to retrieve the configuration for a specific deck
/// </summary>
public record GetDeckConfigRequestDto : AnkiConnectRequest
{
    public GetDeckConfigRequestDto(string deckName)
    {
        Action = "getDeckConfig";
        Params = new DeckConfigParams { Deck = deckName };
    }

    private record DeckConfigParams
    {
        [JsonPropertyName("deck")]
        public string Deck { get; init; } = string.Empty;
    }

    public override string ToString() => JsonSerializer.Serialize(this);
}

/// <summary>
/// Response returned by AnkiConnect for a deck configuration.  We're primarily
/// interested in the "dyn" flag which indicates a filtered (dynamic) deck.
/// </summary>
public record DeckConfigResponse : AnkiConnectResponse
{
    [JsonPropertyName("result")]
    public DeckConfigResult? Result { get; init; }

    public override string ToString() => JsonSerializer.Serialize(this);
}

public record DeckConfigResult
{
    [JsonPropertyName("id")]
    public long Id { get; init; }

    [JsonPropertyName("name")]
    public string Name { get; init; } = string.Empty;

    [JsonPropertyName("dyn")]
    [JsonConverter(typeof(BoolIntJsonConverter))]
    public bool Dyn { get; init; }

    // There are many other configuration properties, but we don't need them here
}

/// <summary>
/// Converter that accepts JSON booleans or numeric (0/1) values when
/// deserializing to a <see cref="bool"/>.
/// </summary>
public class BoolIntJsonConverter : JsonConverter<bool>
{
    public override bool Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        return reader.TokenType switch
        {
            JsonTokenType.True => true,
            JsonTokenType.False => false,
            JsonTokenType.Number => reader.GetInt32() != 0,
            JsonTokenType.String => bool.TryParse(reader.GetString(), out var b) && b,
            _ => throw new JsonException($"Cannot convert token {reader.TokenType} to bool")
        };
    }

    public override void Write(Utf8JsonWriter writer, bool value, JsonSerializerOptions options)
    {
        writer.WriteBooleanValue(value);
    }
}
