using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text.Json;
using Crud.Application.Dtos;
using JsonSerializer = Newtonsoft.Json.JsonSerializer;

namespace Crud.Application.Util;
internal class QuoteJsonConverter : JsonConverter
{
    private readonly Dictionary<string, string> _fieldNameMapping = new(StringComparer.InvariantCultureIgnoreCase)
    {
        { "Name", "Author" }
    };

    public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
    {
        throw new NotImplementedException();
    }

    public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
    {
        var jsonObject = JObject.Load(reader);
        var correctedJson = new JObject();

        foreach (var property in jsonObject.Properties())
        {
            if (_fieldNameMapping.TryGetValue(property.Name, out var correctedKey))
            {
                correctedJson[correctedKey.ToLowerInvariant()] = property.Value;
            }
            else
            {
                correctedJson[property.Name] = property.Value;
            }
        }

        return correctedJson.ToObject<CreateUpdateQuoteDto>();
    }

    public override bool CanConvert(Type objectType)
    {
        return objectType == typeof(CreateUpdateQuoteDto);
    }
}
