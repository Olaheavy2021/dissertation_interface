namespace Shared.Helpers;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;

public class PagedListConverter<T> : JsonConverter<PagedList<T>>
{
    public override void WriteJson(JsonWriter writer, PagedList<T> value, JsonSerializer serializer)
    {
        var obj = new JObject
        {
            ["CurrentPage"] = value.CurrentPage,
            ["TotalPages"] = value.TotalPages,
            ["PageSize"] = value.PageSize,
            ["TotalCount"] = value.TotalCount,
            ["HasPrevious"] = value.HasPrevious,
            ["HasNext"] = value.HasNext,
            ["Items"] = JArray.FromObject(value, serializer)
        };

        obj.WriteTo(writer);
    }

    public override PagedList<T> ReadJson(JsonReader reader, Type objectType, PagedList<T> existingValue, bool hasExistingValue, JsonSerializer serializer)
    {
        var obj = JObject.Load(reader);
        List<T>? items = obj["Items"]?.ToObject<List<T>>(serializer);
        var count = obj["TotalCount"]!.Value<int>();
        var pageNumber = obj["CurrentPage"]!.Value<int>();
        var pageSize = obj["PageSize"]!.Value<int>();

        return new PagedList<T>(items, count, pageNumber, pageSize);
    }
}
