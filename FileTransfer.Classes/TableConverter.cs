using System.Collections.Generic;
using System.IO;

namespace FileTransfer.Classes;

public class TableConverter
{
    public static ConsoleTable CreateTable(string tableName, IList<FileItemJson> items)
    {
        ConsoleTable consoleTable = new ConsoleTable(tableName);

        consoleTable.AddColumn("#");
        consoleTable.AddColumn("Item");
        consoleTable.AddColumn("Directory");
        consoleTable.AddColumn("Size");

        int counter = 0;
        foreach (FileItemJson item in items)
        {
            ConsoleTable.Row row = consoleTable.AddRow();
            row[0] = (counter++).ToString();

            string fileName = Path.GetFileName(item.FileName);

            row[1] = string.IsNullOrWhiteSpace(fileName) ? item.FileName : fileName;
            row[2] = item.IsDirectory.ToString();
            row[3] = item.Size.HasValue ? Convert(item.Size.Value) : string.Empty;
        }

        return consoleTable;
    }

    private static string Convert(long value)
    {
        if (value < 1024)
            return $"{value} b";

        double size = value / 1024.0;

        if (size < 1024.0)
            return $"{size:f2} kb";

        size /= 1024.0;

        return size < 1024.0 ? $"{size:f2} mb" : $"{(size / 1024.0):f2} gb";
    }
}