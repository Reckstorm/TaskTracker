using Azure.Core;
using Microsoft.EntityFrameworkCore;
using Models;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using TaskTrackerServer;

const int PORT = 4444;
IPEndPoint endPoint = new IPEndPoint(IPAddress.Any, PORT);
Socket? socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
socket.Bind(endPoint);
socket.Listen();
Console.ForegroundColor = ConsoleColor.Green;
Console.WriteLine("Server start...");



while (true)
{
    Socket? inboundConnection = null;
    inboundConnection = socket.Accept();
    if (inboundConnection != null)
    {
        Task.Factory.StartNew(() =>
        {
            ProcessConncetion(inboundConnection);
        });
    }
}

Console.ReadLine();

void ProcessConncetion(Socket? clientSocket)
{
    while (true)
    {
        try
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine($"Client connect... {clientSocket.RemoteEndPoint}");

            byte[] data;
            while (true)
            {
                if (!clientSocket.Connected) break;
                data = ReceiveAll(clientSocket);
                string temp = Encoding.Unicode.GetString(data);
                Console.ForegroundColor = ConsoleColor.Blue;

                if (temp == Requests.SendStatuses.ToString()) SendRequest(temp, clientSocket);
                if (temp == Requests.SendRoles.ToString()) SendRequest(temp, clientSocket);
                if (temp == Requests.SendCards.ToString()) SendRequest(temp, clientSocket);
                if (temp == Requests.SendUsers.ToString()) SendRequest(temp, clientSocket);

                if (temp == Requests.ReceiveStatuses.ToString()) ReceiveRequest(temp, clientSocket);
                if (temp == Requests.ReceiveRoles.ToString()) ReceiveRequest(temp, clientSocket);
                if (temp == Requests.ReceiveCards.ToString()) ReceiveRequest(temp, clientSocket);
                if (temp == Requests.ReceiveUsers.ToString()) ReceiveRequest(temp, clientSocket);
            }
            clientSocket.Close();
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Client disconnect...");
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            Console.ForegroundColor = ConsoleColor.Red;
        }
    }
}

byte[] ReceiveAll(Socket socket)
{
    List<byte> buffer = new List<byte>();

    while (socket.Available > 0)
    {
        var currByte = new Byte[1];
        var byteCounter = socket.Receive(currByte, currByte.Length, SocketFlags.None);

        if (byteCounter.Equals(1))
        {
            buffer.Add(currByte[0]);
        }
    }

    return buffer.ToArray();
}

void SendRequest(string request, Socket currentSocket)
{
    Console.WriteLine($"New {request} request from client {currentSocket.RemoteEndPoint}");
    using (Context context = GetContext())
    {
        if (request == Requests.SendStatuses.ToString())
        {
            currentSocket.Send(Encoding.Unicode.GetBytes(JsonSerializer.Serialize(context.Statuses)));
            Console.WriteLine($"New reply to the request {request} has been sent to {currentSocket.RemoteEndPoint}");
        }
        if (request == Requests.SendRoles.ToString())
        {
            currentSocket.Send(Encoding.Unicode.GetBytes(JsonSerializer.Serialize(context.Roles)));
            Console.WriteLine($"New reply to the request {request} has been sent to {currentSocket.RemoteEndPoint}");
        }
        if (request == Requests.SendCards.ToString())
        {
            currentSocket.Send(Encoding.Unicode.GetBytes(JsonSerializer.Serialize(context.Cards.
                Include(c => c.LastUserModified).
                Include(c => c.UserCreated).
                Include(c => c.Assignee).
                Include(c => c.Status))));
            Console.WriteLine($"New reply to the request {request} has been sent to {currentSocket.RemoteEndPoint}");
        }
        if (request == Requests.SendUsers.ToString())
        {
            currentSocket.Send(Encoding.Unicode.GetBytes(JsonSerializer.Serialize(context.Users)));
            Console.WriteLine($"New reply to the request {request} has been sent to {currentSocket.RemoteEndPoint}");
        }
    }
}

void ReceiveRequest(string request, Socket currentSocket)
{
    Console.WriteLine($"New {request} request from client {currentSocket.RemoteEndPoint}");
    using (Context context = GetContext())
    {
        byte[] data = ReceiveAll(currentSocket);
        string temp = Encoding.Unicode.GetString(data);
        if (request == Requests.ReceiveStatuses.ToString())
        {
            context.Statuses.RemoveRange(context.Statuses);
            context.Statuses.UpdateRange(JsonSerializer.Deserialize<List<Status>>(temp));
            context.SaveChanges();
            Console.WriteLine($"{request} has been processed by the server");
        }
        if (request == Requests.ReceiveRoles.ToString())
        {
            context.Roles.RemoveRange(context.Roles);
            context.Roles.UpdateRange(JsonSerializer.Deserialize<List<Role>>(temp));
            context.SaveChanges();
            Console.WriteLine($"{request} has been processed by the server");
        }
        if (request == Requests.ReceiveCards.ToString())
        {
            context.Cards.RemoveRange(context.Cards);
            context.Cards.UpdateRange(JsonSerializer.Deserialize<List<Card>>(temp));
            context.SaveChanges();
            Console.WriteLine($"{request} has been processed by the server");
        }
        if (request == Requests.ReceiveUsers.ToString())
        {
            context.Users.RemoveRange(context.Users);
            context.Users.UpdateRange(JsonSerializer.Deserialize<List<User>>(temp));
            context.SaveChanges();
            Console.WriteLine($"{request} has been processed by the server");
        }
    }
}

Context GetContext()
{
    object locker = new object();
    lock (locker)
    {
        return new Context();
    }
}