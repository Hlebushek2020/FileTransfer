using System.Collections.Generic;
using System.IO;

namespace FileTransfer;

public class ConsoleTable
{
    private readonly List<Column> _columns = new List<Column>();
    private readonly List<Row> _rows = new List<Row>();
    private readonly string _tableTitle;

    private int _fullLenght = 0;

    public ConsoleTable(string tableTitle)
    {
        _tableTitle = tableTitle;
    }

    public void AddColumn(string title)
    {
        Column column = new Column(title);
        _fullLenght += column.MaxLenght;
        _columns.Add(column);
    }

    public Row AddRow()
    {
        Row row = new Row(this);
        _rows.Add(row);
        return row;
    }

    public void Print(TextWriter writer)
    {
        if (_columns.Count <= 0)
            return;

        writer.WriteLine(_tableTitle);
        int lenghtSep = 3 * (_columns.Count - 1) + _fullLenght + 4;
        for (int numColumn = 0; numColumn < _columns.Count; numColumn++)
        {
            if (numColumn == 0)
                writer.Write("| ");

            Column column = _columns[numColumn];

            writer.Write(column.Title);
            writer.Write(new string(' ', column.MaxLenght - column.Title.Length + 1));
            writer.Write("| ");

            if (numColumn == _columns.Count - 1)
                writer.WriteLine();
        }

        writer.WriteLine(new string('-', lenghtSep));
        foreach (Row row in _rows)
        {
            for (int numColumn = 0; numColumn < _columns.Count; numColumn++)
            {
                if (numColumn == 0)
                    writer.Write("| ");

                string cellValue = row[numColumn];
                writer.Write(cellValue);
                writer.Write(new string(' ', _columns[numColumn].MaxLenght - cellValue.Length + 1));
                writer.Write("| ");

                if (numColumn == _columns.Count - 1)
                    writer.WriteLine();
            }
        }

        writer.WriteLine(new string('-', lenghtSep));
    }

    private class Column
    {
        public string Title { get; }
        public int MaxLenght { get; set; }

        public Column(string title)
        {
            Title = title;
            MaxLenght = title.Length;
        }
    }

    public class Row
    {
        private readonly ConsoleTable _writerTable;
        private readonly string[] _cells;

        public string this[int index]
        {
            get => _cells[index];
            set
            {
                _cells[index] = value;
                Column column = _writerTable._columns[index];

                if (value.Length > column.MaxLenght)
                {
                    _writerTable._fullLenght += value.Length - column.MaxLenght;
                    column.MaxLenght = value.Length;
                }
            }
        }

        public void SetAnyValue(int index, object value) => this[index] = value?.ToString() ?? string.Empty;

        internal Row(ConsoleTable writerTable)
        {
            _writerTable = writerTable;
            _cells = new string[writerTable._columns.Count];
        }
    }
}