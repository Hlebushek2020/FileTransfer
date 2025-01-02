using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using FileTransfer.Classes;

namespace FileTransferServer;

internal class Program
{
    private const int BufferSize = 10240;

    static void Main(string[] args)
    {
        Console.Write("Port: ");
        int port = Convert.ToInt32(Console.ReadLine());
        //Console.Write("Internet speed (mbit/sec): ");
        //int internetSpeed = Convert.ToInt32(Console.ReadLine());

        //int packedPeerSecond = internetSpeed * 1024 * 1024 / 8 / BufferSize;
        //int packedInterval = 1000 / packedPeerSecond;

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
                    DirectoryCommandHandler(commandJson, binaryWriter);
                    break;

                case Commands.Download:
                    DownloadCommandHandler(commandJson, binaryWriter);
                    break;
            }
        }
    }

    private static void DirectoryCommandHandler(CommandJson commandJson, BinaryWriter binaryWriter)
    {
        try
        {
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

            binaryWriter.Write(new ItemsJson(entries).ToString());
        }
        catch (Exception ex)
        {
            binaryWriter.Write(new ItemsJson(ex).ToString());
        }
    }

    private static void DownloadCommandHandler(CommandJson commandJson, BinaryWriter binaryWriter)
    {
        DirectoryInfo directoryInfo = new DirectoryInfo(commandJson.Args);
        int startPathIndex = commandJson.Args.Length + 1;

        FileInfo[] files = directoryInfo.GetFiles("*", SearchOption.AllDirectories);

        binaryWriter.Write(files.Length > 0);

        for (int numFileInfo = 0; numFileInfo < files.Length; numFileInfo++)
        {
            FileInfo fileInfo = files[numFileInfo];

            binaryWriter.Write(fileInfo.FullName.Substring(startPathIndex));

            using FileStream readStream = fileInfo.OpenRead();

            byte[] buffer = new byte[BufferSize];
            int readCount;

            while ((readCount = readStream.Read(buffer, 0, BufferSize)) != 0)
            {
                binaryWriter.Write(readCount);
                binaryWriter.Write(buffer, 0, readCount);
            }

            binaryWriter.Write(0);
            binaryWriter.Write(numFileInfo < files.Length - 1);
        }
    }
}