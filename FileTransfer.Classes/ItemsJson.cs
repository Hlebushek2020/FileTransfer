using System;
using System.Collections.Generic;
using System.Text.Json;

namespace FileTransfer.Classes;

public class ItemsJson
{
    public IList<FileItemJson> Items { get; set; }
    public string Error { get; set; }

    public ItemsJson()
    {
    }

    public ItemsJson(IList<FileItemJson> items)
    {
        Items = items;
    }

    public ItemsJson(Exception exception)
    {
        Error = exception.Message;
    }

    public static ItemsJson Parse(string json)
    {
        return JsonSerializer.Deserialize<ItemsJson>(json, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            WriteIndented = false
        });
    }

    public override string ToString()
    {
        return JsonSerializer.Serialize(this, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            WriteIndented = false
        });
    }
}