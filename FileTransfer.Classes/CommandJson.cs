using System.Text.Json;

namespace FileTransfer.Classes;

public class CommandJson
{
    public Commands Command { get; set; }
    public string Args { get; set; }

    public static CommandJson Parse(string json)
    {
        return JsonSerializer.Deserialize<CommandJson>(json, new JsonSerializerOptions
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