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
                if (data.Length == 0) continue;
                string temp = Encoding.Unicode.GetString(data);
                Console.ForegroundColor = ConsoleColor.Blue;

                if (temp.Contains(Requests.SendStatuses.ToString())) SendRequest(Requests.SendStatuses.ToString(), clientSocket);
                if (temp.Contains(Requests.SendRoles.ToString())) SendRequest(Requests.SendRoles.ToString(), clientSocket);
                if (temp.Contains(Requests.SendCards.ToString())) SendRequest(Requests.SendCards.ToString(), clientSocket);
                if (temp.Contains(Requests.SendUsers.ToString())) SendRequest(Requests.SendUsers.ToString(), clientSocket);

                if (temp.Contains(Requests.ReceiveCard.ToString())) ReceiveRequest(Requests.ReceiveCard.ToString(), temp, clientSocket);
                if (temp.Contains(Requests.RemoveCard.ToString())) RemoveRequest(Requests.RemoveCard.ToString(), temp, clientSocket);
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
                Include(c => c.Assignee).
                Include(c => c.Status))));
            Console.WriteLine($"New reply to the request {request} has been sent to {currentSocket.RemoteEndPoint}");
        }
        if (request == Requests.SendUsers.ToString())
        {
            currentSocket.Send(Encoding.Unicode.GetBytes(JsonSerializer.Serialize(context.Users.Include(c => c.Role))));
            Console.WriteLine($"New reply to the request {request} has been sent to {currentSocket.RemoteEndPoint}");
        }
    }
}

void ReceiveRequest(string request, string inboundData, Socket currentSocket)
{
    Console.WriteLine($"New {request} request from client {currentSocket.RemoteEndPoint}");
    using (Context context = GetContext())
    {
        inboundData = $"{inboundData.Substring(inboundData.IndexOf("&") + 1)}";
        if (request == Requests.ReceiveCard.ToString())
        {
            Card temp = JsonSerializer.Deserialize<Card>(inboundData);
            if (context.Cards.Contains(temp))
                context.Cards.Attach(temp).State = EntityState.Modified;
            else
                context.Cards.Attach(temp).State = EntityState.Added;
            context.SaveChanges();
            Console.WriteLine($"{request} has been processed by the server");
        }
    }
}

void RemoveRequest(string request, string inboundData, Socket currentSocket)
{
    Console.WriteLine($"New {request} request from client {currentSocket.RemoteEndPoint}");
    using (Context context = GetContext())
    {
        inboundData = $"{inboundData.Substring(inboundData.IndexOf("&") + 1)}";
        if (request == Requests.RemoveCard.ToString())
        {
            Card temp = JsonSerializer.Deserialize<Card>(inboundData);
            if (context.Cards.Contains(temp))
                context.Cards.Attach(temp).State = EntityState.Deleted;
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