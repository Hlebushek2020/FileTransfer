using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using FileTransfer;

namespace FileTransferServer;

internal class Program
{
    static void Main(string[] args)
    {
        Console.Write("Port: ");
        int port = Convert.ToInt32(Console.ReadLine());
        Console.Write("Internet speed (mbit/sec): ");
        int internetSpeed = Convert.ToInt32(Console.ReadLine());

        Console.WriteLine("Starting server...");
        TcpListener listener = new TcpListener(IPAddress.Any, port);
        listener.Start();
        Console.WriteLine($"Server {listener.LocalEndpoint} started");
        Console.WriteLine("Waiting for client...");

        TcpClient tcpClient = listener.AcceptTcpClient();
        Console.WriteLine("Client connected");

        Console.WriteLine("Server stopped");
        listener.Stop();

        NetworkStream networkStream = tcpClient.GetStream();
        BinaryReader binaryReader = new BinaryReader(networkStream, Encoding.UTF8);
        BinaryWriter binaryWriter = new BinaryWriter(networkStream, Encoding.UTF8);

        bool isCommandHandler = true;
        while (isCommandHandler)
        {
            CommandJson commandJson = CommandJson.Parse(binaryReader.ReadString());

            switch (commandJson.Command)
            {
                case Commands.Exit:
                    isCommandHandler = false;
                    break;

                case Commands.Directory:
                    IList<FileItemJson> entries = new List<FileItemJson>();
                    if (string.IsNullOrWhiteSpace(commandJson.Args))
                    {
                        foreach (string drive in Directory.GetLogicalDrives())
                            entries.Add(new FileItemJson { FileName = drive, IsDirectory = true });
                    }
                    else
                    {
                        DirectoryInfo directoryInfo = new DirectoryInfo(commandJson.Args);
                        foreach (DirectoryInfo directory in directoryInfo.GetDirectories())
                            entries.Add(new FileItemJson { FileName = directory.FullName, IsDirectory = true });
                        foreach (FileInfo fileInfo in directoryInfo.GetFiles())
                            entries.Add(new FileItemJson { FileName = fileInfo.FullName, Size = fileInfo.Length });
                    }

                    binaryWriter.Write(FileItemJson.ListSerialize(entries));
                    break;
            }
        }
    }
}