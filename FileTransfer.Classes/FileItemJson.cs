using System.Text.Json;

namespace FileTransfer.Classes;

public class FileItemJson
{
    public string FileName { get; set; }
    public bool IsDirectory { get; set; }
    public long? Size { get; set; }


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