using System;
using System.IO;
using System.Net.Sockets;
using System.Text;
using FileTransfer.Classes;

namespace FileTransferClient;

internal class Program
{
    static void Main(string[] args)
    {
        Console.Write("Address: ");
        string address = Console.ReadLine();

        Console.Write("Port: ");
        int port = Convert.ToInt32(Console.ReadLine());

        using TcpClient tcpClient = new TcpClient();
        tcpClient.Connect(address, port);

        using NetworkStream networkStream = tcpClient.GetStream();
        using BinaryReader binaryReader = new BinaryReader(networkStream, Encoding.UTF8);
        using BinaryWriter binaryWriter = new BinaryWriter(networkStream, Encoding.UTF8);

        binaryWriter.Write(new CommandJson { Command = Commands.Directory }.ToString());
        ItemsJson items = ItemsJson.Parse(binaryReader.ReadString());
        ConsoleTable table = TableConverter.CreateTable("Root", items.Items);
        table.Print(Console.Out);

        FileItemJson selectItem = null;
        ItemsJson newItems = null;

        bool isCommandExecute = true;
        while (isCommandExecute)
        {
            Console.Write("#: ");

            string command = Console.ReadLine();

            if (string.IsNullOrWhiteSpace(command))
                continue;

            command = command.Replace("  ", " ").Trim(' ');

            if (command.Equals(Commands.Exit.ToString(), StringComparison.OrdinalIgnoreCase))
            {
                binaryWriter.Write(new CommandJson { Command = Commands.Exit }.ToString());
                isCommandExecute = false;
                continue;
            }

            if (command.Equals(Commands.Back.ToString(), StringComparison.OrdinalIgnoreCase))
            {
                if (selectItem == null)
                {
                    Console.WriteLine("Command is unavailable");
                    continue;
                }

                binaryWriter.Write(new CommandJson
                {
                    Command = Commands.Directory,
                    Args = Path.GetDirectoryName(selectItem.FileName)
                }.ToString());

                newItems = ItemsJson.Parse(binaryReader.ReadString());

                if (string.IsNullOrWhiteSpace(newItems.Error))
                {
                    items = newItems;
                    table = TableConverter.CreateTable(selectItem.FileName, items.Items);
                    table.Print(Console.Out);
                }
                else
                {
                    Console.WriteLine(newItems.Error);
                }

                continue;
            }

            int firstEscapeIndex = command.IndexOf(' ');

            int itemIndex = int.Parse(command.Substring(0, firstEscapeIndex));

            if (itemIndex >= items.Items.Count)
            {
                Console.WriteLine("Invalid index");
                continue;
            }

            selectItem = items.Items[itemIndex];

            int secondEscapeIndex = command.IndexOf(' ', firstEscapeIndex);

            if (firstEscapeIndex != -1 && secondEscapeIndex != -1)
            {
                string subCommand = command.Substring(firstEscapeIndex, secondEscapeIndex - firstEscapeIndex);
                if (subCommand.Equals(Commands.Download.ToString(), StringComparison.OrdinalIgnoreCase))
                    Download(binaryReader, binaryWriter, selectItem, command.Substring(secondEscapeIndex));

                continue;
            }

            if (!selectItem.IsDirectory)
            {
                Console.WriteLine("Item is not a directory");
                continue;
            }

            binaryWriter.Write(new CommandJson
            {
                Command = Commands.Directory,
                Args = selectItem.FileName
            }.ToString());

            newItems = ItemsJson.Parse(binaryReader.ReadString());

            if (string.IsNullOrWhiteSpace(newItems.Error))
            {
                items = newItems;
                table = TableConverter.CreateTable(selectItem.FileName, items.Items);
                table.Print(Console.Out);
            }
            else
            {
                Console.WriteLine(newItems.Error);
            }
        }
    }

    private static void Download(
        BinaryReader binaryReader,
        BinaryWriter binaryWriter,
        FileItemJson selected,
        string outputPath)
    {
        binaryWriter.Write(new CommandJson
        {
            Command = Commands.Download,
            Args = selected.FileName
        }.ToString());

        while (binaryReader.ReadBoolean())
        {
            string relativeFileName = binaryReader.ReadString();
            string fullFileName = Path.Combine(outputPath, relativeFileName);
            string directory = Path.GetDirectoryName(fullFileName);

            if (!string.IsNullOrWhiteSpace(directory))
                Directory.CreateDirectory(directory);

            using FileStream fileStream = new FileStream(fullFileName, FileMode.Create, FileAccess.Write);

            int bufferSize;

            do
            {
                bufferSize = binaryReader.ReadInt32();

                byte[] buffer = new byte[bufferSize];
                binaryWriter.Write(buffer);

                fileStream.Write(buffer, 0, bufferSize);
            } while (bufferSize != 0);
        }
    }
}