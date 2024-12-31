using System;
using System.Collections.Generic;
using System.IO;

namespace FileTransfer;

public class TableConverter
{
    public static ConsoleTable CreateTable(string tableName, IList<FileItemJson> items)
    {
        ConsoleTable consoleTable = new ConsoleTable(tableName);

        consoleTable.AddColumn("Ind");
        consoleTable.AddColumn("Item");
        consoleTable.AddColumn("Is Directory");
        consoleTable.AddColumn("Size");

        int counter = 0;
        foreach (FileItemJson item in items)
        {
            ConsoleTable.Row row = consoleTable.AddRow();
            row[0] = (counter++).ToString();
            row[1] = Path.GetDirectoryName(item.FileName) ?? item.FileName;
            row[2] = item.IsDirectory.ToString();
            row[3] = item.Size.HasValue ? Convert(item.Size.Value) : string.Empty;
        }

        return consoleTable;
    }

    private static string Convert(long value)
    {
        long num1 = value;
        if (num1 < 1024L)
            return ConvertToString(num1, 0L, "B");
        long num2 = num1 / 1024L;
        if (num2 < 1024L)
            return ConvertToString(num2, 0L, "KB");
        long num3 = num2 / 1024L;
        return num3 < 1024L
            ? ConvertToString(num3, num2 % 1024L, "MB")
            : ConvertToString(num3 / 1024L, num3 % 1024L, "GB");
    }

    private static string ConvertToString(long value, long subValue, string sizeName)
    {
        return subValue >= 100L
            ? $"{value},{Math.Round(subValue * 0.01, MidpointRounding.AwayFromZero)} {sizeName}"
            : $"{value} {sizeName}";
    }
}