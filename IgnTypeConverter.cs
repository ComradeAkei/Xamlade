using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Xamlade;


public class IgnoredTypesConverter<T> : JsonConverter<T>
{
    private readonly HashSet<Type> ignoredTypes;

    public IgnoredTypesConverter(HashSet<Type> ignoredTypes)
    {
        this.ignoredTypes = ignoredTypes;
    }

    public override bool CanConvert(Type typeToConvert)
    {
        return !ignoredTypes.Contains(typeToConvert);
    }

    public override T Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        return JsonSerializer.Deserialize<T>(ref reader, options);
    }

    public override void Write(Utf8JsonWriter writer, T value, JsonSerializerOptions options)
    {
        JsonSerializer.Serialize(writer, value, options);
    }
}