using System.Collections.Generic;
using System.Text.Json;

namespace FileTransfer.Classes;

public class ItemsJson
{
    public IList<FileItemJson> Items { get; set; }
    public string Error { get; set; }

    public static IList<FileItemJson> Parse(string json)
    {
        return JsonSerializer.Deserialize<List<FileItemJson>>(json, new JsonSerializerOptions
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