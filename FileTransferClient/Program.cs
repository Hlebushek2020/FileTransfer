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
            string command = binaryReader.ReadString();
            if (command.Equals(Commands.Exit.ToString(), StringComparison.OrdinalIgnoreCase))
            {
                binaryWriter.Write(new CommandJson { Command = Commands.Exit }.ToString());
                isCommandExecute = false;
                continue;
            }
        }
    }
}