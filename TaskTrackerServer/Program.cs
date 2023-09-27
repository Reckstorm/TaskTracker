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
    Socket? inboundConnection = await socket.AcceptAsync();
    if (inboundConnection != null)
    {
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine($"Client connect... {inboundConnection.RemoteEndPoint}");
        Task.Factory.StartNew(() =>
        {
            ProcessConncetion(inboundConnection);
        });
    }
}

Console.ReadLine();
async void ProcessConncetion(Socket? clientSocket)
{
    try
    {
        while (true)
        {
            if (!clientSocket.Connected) break;
            List<RequestWrapper> requests = new List<RequestWrapper>();
            Thread.Sleep(2000);
            string temp = await ReceiveAll(clientSocket);
            if (temp.Equals(string.Empty)) continue;
            //requests.Add(JsonSerializer.Deserialize<RequestWrapper>(temp.Trim()));
            RequestWrapper request = JsonSerializer.Deserialize<RequestWrapper>(temp.Trim());
            Console.ForegroundColor = ConsoleColor.Blue;

            if (request.RequestType.Equals(Requests.SendStatuses)) SendRequest(Requests.SendStatuses.ToString(), request, clientSocket);
            if (request.RequestType.Equals(Requests.SendRoles)) SendRequest(Requests.SendRoles.ToString(), request, clientSocket);
            if (request.RequestType.Equals(Requests.SendCards)) SendRequest(Requests.SendCards.ToString(), request, clientSocket);
            if (request.RequestType.Equals(Requests.SendUsers)) SendRequest(Requests.SendUsers.ToString(), request, clientSocket);
            if (request.RequestType.Equals(Requests.SendUser)) SendRequest(Requests.SendUser.ToString(), request, clientSocket);

            if (request.RequestType.Equals(Requests.ReceiveCard)) ReceiveRequest(Requests.ReceiveCard.ToString(), request, clientSocket);
            if (request.RequestType.Equals(Requests.RemoveCard)) RemoveRequest(Requests.RemoveCard.ToString(), request, clientSocket);
            if (request.RequestType.Equals(Requests.ConnectionClose)) break;

        }
        clientSocket.Shutdown(SocketShutdown.Both);
        clientSocket.Disconnect(true);
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine("Client disconnect...");
    }
    catch (Exception ex)
    {
        Console.WriteLine(ex.Message);
        Console.ForegroundColor = ConsoleColor.Red;
    }
}
#region
//void ProcessConncetion(Socket? clientSocket)
//{
//    while (true)
//    {
//        try
//        {
//            Console.ForegroundColor = ConsoleColor.Yellow;
//            Console.WriteLine($"Client connect... {clientSocket.RemoteEndPoint}");

//            byte[] data;
//            while (true)
//            {
//                if (!clientSocket.Connected) break;
//                data = ReceiveAll(clientSocket);
//                if (data.Length == 0) continue;
//                string temp = Encoding.Unicode.GetString(data);
//                Console.ForegroundColor = ConsoleColor.Blue;

//                if (temp.Contains(Requests.SendStatuses.ToString())) SendRequest(Requests.SendStatuses.ToString(), clientSocket);
//                if (temp.Contains(Requests.SendRoles.ToString())) SendRequest(Requests.SendRoles.ToString(), clientSocket);
//                if (temp.Contains(Requests.SendCards.ToString())) SendRequest(Requests.SendCards.ToString(), clientSocket);
//                if (temp.Contains(Requests.SendUsers.ToString())) SendRequest(Requests.SendUsers.ToString(), clientSocket);

//                if (temp.Contains(Requests.ReceiveCard.ToString())) ReceiveRequest(Requests.ReceiveCard.ToString(), temp, clientSocket);
//                if (temp.Contains(Requests.RemoveCard.ToString())) RemoveRequest(Requests.RemoveCard.ToString(), temp, clientSocket);
//            }
//            clientSocket.Close();
//            Console.ForegroundColor = ConsoleColor.Green;
//            Console.WriteLine("Client disconnect...");
//        }
//        catch (Exception ex)
//        {
//            Console.WriteLine(ex.Message);
//            Console.ForegroundColor = ConsoleColor.Red;
//        }
//    }
//}

//byte[] ReceiveAll(Socket socket)
//{
//    List<byte> buffer = new List<byte>();

//    while (socket.Available > 0)
//    {
//        var currByte = new Byte[1];
//        var byteCounter = socket.Receive(currByte, currByte.Length, SocketFlags.None);

//        if (byteCounter.Equals(1))
//        {
//            buffer.Add(currByte[0]);
//        }
//    }

//    return buffer.ToArray();
//}

//void SendRequest(string request, Socket currentSocket)
//{
//    Console.WriteLine($"New {request} request from client {currentSocket.RemoteEndPoint}");
//    using (Context context = GetContext())
//    {
//        if (request == Requests.SendStatuses.ToString())
//        {
//            currentSocket.Send(Encoding.Unicode.GetBytes(JsonSerializer.Serialize(context.Statuses)));
//            Console.WriteLine($"New reply to the request {request} has been sent to {currentSocket.RemoteEndPoint}");
//        }
//        if (request == Requests.SendRoles.ToString())
//        {
//            currentSocket.Send(Encoding.Unicode.GetBytes(JsonSerializer.Serialize(context.Roles)));
//            Console.WriteLine($"New reply to the request {request} has been sent to {currentSocket.RemoteEndPoint}");
//        }
//        if (request == Requests.SendCards.ToString())
//        {
//            currentSocket.Send(Encoding.Unicode.GetBytes(JsonSerializer.Serialize(context.Cards.
//                Include(c => c.Assignee).
//                Include(c => c.Status))));
//            Console.WriteLine($"New reply to the request {request} has been sent to {currentSocket.RemoteEndPoint}");
//        }
//        if (request == Requests.SendUsers.ToString())
//        {
//            currentSocket.Send(Encoding.Unicode.GetBytes(JsonSerializer.Serialize(context.Users.Include(c => c.Role))));
//            Console.WriteLine($"New reply to the request {request} has been sent to {currentSocket.RemoteEndPoint}");
//        }
//    }
//}

//void ReceiveRequest(string request, string inboundData, Socket currentSocket)
//{
//    Console.WriteLine($"New {request} request from client {currentSocket.RemoteEndPoint}");
//    using (Context context = GetContext())
//    {
//        inboundData = $"{inboundData.Substring(inboundData.IndexOf("&") + 1)}";
//        if (request == Requests.ReceiveCard.ToString())
//        {
//            Card temp = JsonSerializer.Deserialize<Card>(inboundData);
//            if (context.Cards.Contains(temp))
//                context.Cards.Attach(temp).State = EntityState.Modified;
//            else
//                context.Cards.Attach(temp).State = EntityState.Added;
//            context.SaveChanges();
//            Console.WriteLine($"{request} has been processed by the server");
//        }
//    }
//}

//void RemoveRequest(string request, string inboundData, Socket currentSocket)
//{
//    Console.WriteLine($"New {request} request from client {currentSocket.RemoteEndPoint}");
//    using (Context context = GetContext())
//    {
//        inboundData = $"{inboundData.Substring(inboundData.IndexOf("&") + 1)}";
//        if (request == Requests.RemoveCard.ToString())
//        {
//            Card temp = JsonSerializer.Deserialize<Card>(inboundData);
//            if (context.Cards.Contains(temp))
//                context.Cards.Attach(temp).State = EntityState.Deleted;
//            context.SaveChanges();
//            Console.WriteLine($"{request} has been processed by the server");
//        }
//    }
//}
#endregion
async Task<string> ReceiveAll(Socket socket)
{
    List<byte> buffer = new List<byte>();
    StringBuilder sb = new StringBuilder();
    while (socket.Available > 0)
    {
        var currByte = new Byte[1];
        var byteCounter = socket.Receive(currByte, currByte.Length, SocketFlags.None);

        if (byteCounter.Equals(1))
        {
            buffer.Add(currByte[0]);
        }
    }
    sb.Append(Encoding.Unicode.GetString(buffer.ToArray()));
    return sb.ToString();
}

void SendRequest(string request, RequestWrapper wrappedData, Socket currentSocket)
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
        if (request == Requests.SendUser.ToString())
        {
            string username = wrappedData.Username;
            string password = wrappedData.Password;
            User temp = context.Users.Include(c => c.Role).FirstOrDefault(u => u.Email.Equals(username) && u.Password.Equals(password));
            if (temp != null)
            {
                currentSocket.Send(Encoding.Unicode.GetBytes(JsonSerializer.Serialize(temp)));
                Console.WriteLine($"User {username} authorized from {currentSocket.RemoteEndPoint}");
            }
            else
            {
                currentSocket.Send(Encoding.Unicode.GetBytes(JsonSerializer.Serialize(new User())));
                Console.WriteLine($"User {username} failed to autorize from {currentSocket.RemoteEndPoint}");
            }
        }
    }
}

void ReceiveRequest(string request, RequestWrapper wrappedData, Socket currentSocket)
{
    Console.WriteLine($"New {request} request from client {currentSocket.RemoteEndPoint}");
    using (Context context = GetContext())
    {
        if (request == Requests.ReceiveCard.ToString())
        {
            Card temp = JsonSerializer.Deserialize<Card>(wrappedData.RequestBody);
            if (context.Cards.Contains(temp))
                context.Cards.Attach(temp).State = EntityState.Modified;
            else
                context.Cards.Attach(temp).State = EntityState.Added;
            context.SaveChanges();
            Console.WriteLine($"{request} has been processed by the server");
        }
    }
}

void RemoveRequest(string request, RequestWrapper wrappedData, Socket currentSocket)
{
    Console.WriteLine($"New {request} request from client {currentSocket.RemoteEndPoint}");
    using (Context context = GetContext())
    {
        if (request == Requests.RemoveCard.ToString())
        {
            Card temp = JsonSerializer.Deserialize<Card>(wrappedData.RequestBody);
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