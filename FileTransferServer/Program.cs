using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;

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

        bool isDisconnected = false;
        while (!isDisconnected)
        {
            string text = binaryReader.ReadString();
        }
    }
}