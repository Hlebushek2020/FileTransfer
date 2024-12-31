using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using System.Text;
using FileTransfer;

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
        IList<FileItemJson> items = FileItemJson.ListParse(binaryReader.ReadString());
        ConsoleTable table = TableConverter.CreateTable("Root", items);
        table.Print(Console.Out);

        bool isCommandExecute = true;
        while (isCommandExecute)
        {
            Console.Write("#: ");

            string command = Console.ReadLine();
            command = command.Replace("  ", " ");

            if (command.Equals(Commands.Exit.ToString(), StringComparison.OrdinalIgnoreCase))
            {
                binaryWriter.Write(new CommandJson { Command = Commands.Exit }.ToString());
                isCommandExecute = false;
                continue;
            }

            string[] commandParts = command.Split(' ');

            if (commandParts.Length > 1)
            {
                //download check
                continue;
            }

            int itemIndex = int.Parse(commandParts[0]);

            if (itemIndex >= items.Count)
            {
                Console.WriteLine("Invalid index");
                continue;
            }

            FileItemJson selectItem = items[itemIndex];

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

            items = FileItemJson.ListParse(binaryReader.ReadString());
            table = TableConverter.CreateTable(selectItem.FileName, items);
            table.Print(Console.Out);
        }
    }
}