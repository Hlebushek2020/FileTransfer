using System.Collections.Generic;
using System.Text.Json;

namespace FileTransfer;

public class FileItemJson
{
    public string FileName { get; set; }
    public bool IsDirectory { get; set; }
    public long? Size { get; set; }

    public static IList<FileItemJson> ListParse(string json)
    {
        return JsonSerializer.Deserialize<List<FileItemJson>>(json, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            WriteIndented = false
        });
    }

    public static string ListSerialize(IList<FileItemJson> list)
    {
        return JsonSerializer.Serialize(list, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            WriteIndented = false
        });
    }

    public static FileItemJson Parse(string json)
    {
        return JsonSerializer.Deserialize<FileItemJson>(json, new JsonSerializerOptions
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