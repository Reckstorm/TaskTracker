using Azure.Core;
using Microsoft.EntityFrameworkCore;
using Models;
using System.Linq;
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
            string temp = await ReceiveAll(clientSocket);
            if (temp.Equals(string.Empty)) continue;
            RequestWrapper request = JsonSerializer.Deserialize<RequestWrapper>(temp);
            Console.ForegroundColor = ConsoleColor.Blue;

            if (request.RequestType.Equals(Requests.SendStatuses)) SendRequest(Requests.SendStatuses.ToString(), request, clientSocket);
            if (request.RequestType.Equals(Requests.SendRoles)) SendRequest(Requests.SendRoles.ToString(), request, clientSocket);
            if (request.RequestType.Equals(Requests.SendCards)) SendRequest(Requests.SendCards.ToString(), request, clientSocket);
            if (request.RequestType.Equals(Requests.SendUsers)) SendRequest(Requests.SendUsers.ToString(), request, clientSocket);
            if (request.RequestType.Equals(Requests.SendUser)) SendRequest(Requests.SendUser.ToString(), request, clientSocket);
            if (request.RequestType.Equals(Requests.SearchCard)) SendRequest(Requests.SearchCard.ToString(), request, clientSocket);

            if (request.RequestType.Equals(Requests.ReceiveCard)) ReceiveRequest(Requests.ReceiveCard.ToString(), request, clientSocket);
            if (request.RequestType.Equals(Requests.ReceiveUser)) ReceiveRequest(Requests.ReceiveUser.ToString(), request, clientSocket);
            if (request.RequestType.Equals(Requests.ReceiveStatus)) ReceiveRequest(Requests.ReceiveStatus.ToString(), request, clientSocket);
            if (request.RequestType.Equals(Requests.RemoveCard)) RemoveRequest(Requests.RemoveCard.ToString(), request, clientSocket);
            if (request.RequestType.Equals(Requests.RemoveUser)) RemoveRequest(Requests.RemoveUser.ToString(), request, clientSocket);
            if (request.RequestType.Equals(Requests.RemoveStatus)) RemoveRequest(Requests.RemoveStatus.ToString(), request, clientSocket);
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
        if (request == Requests.SendUser.ToString() && !wrappedData.IsAuthorized)
        {
            User temp = context.Users.Include(c => c.Role).FirstOrDefault(u => u.Email.Equals(wrappedData.User.Email) && u.Password.Equals(wrappedData.User.Password));
            if (temp != null)
            {
                currentSocket.Send(Encoding.Unicode.GetBytes(JsonSerializer.Serialize(temp)));
                Console.WriteLine($"User {wrappedData.User.Email} authorized from {currentSocket.RemoteEndPoint}");
            }
            else
            {
                currentSocket.Send(Encoding.Unicode.GetBytes(JsonSerializer.Serialize(new User())));
                Console.WriteLine($"User {wrappedData.User.Email} failed to autorize from {currentSocket.RemoteEndPoint}");
            }
        }
        if (request == Requests.SearchCard.ToString() && wrappedData.IsAuthorized)
        {
            List<Card> cards = new List<Card>();
            foreach (Card card in context.Cards.
                Include(c => c.Assignee).
                Include(c => c.Status))
            {
                if (!(card.Id.ToString().ToLower().Contains(wrappedData.RequestBody) ||
                              card.Title.ToLower().Contains(wrappedData.RequestBody) ||
                              card.Description.ToLower().Contains(wrappedData.RequestBody)))
                {
                    cards.Add(card);
                }
            }
            currentSocket.Send(Encoding.Unicode.GetBytes(JsonSerializer.Serialize(cards)));
            Console.WriteLine($"New reply to the request {request} has been sent to {currentSocket.RemoteEndPoint}");
        }
        if (request == Requests.SendStatuses.ToString() && wrappedData.IsAuthorized)
        {
            currentSocket.Send(Encoding.Unicode.GetBytes(JsonSerializer.Serialize(context.Statuses)));
            Console.WriteLine($"New reply to the request {request} has been sent to {currentSocket.RemoteEndPoint}");
        }
        if (request == Requests.SendRoles.ToString() && wrappedData.IsAuthorized)
        {
            currentSocket.Send(Encoding.Unicode.GetBytes(JsonSerializer.Serialize(context.Roles)));
            Console.WriteLine($"New reply to the request {request} has been sent to {currentSocket.RemoteEndPoint}");
        }
        if (request == Requests.SendCards.ToString() && wrappedData.IsAuthorized)
        {
            currentSocket.Send(Encoding.Unicode.GetBytes(JsonSerializer.Serialize(context.Cards.
                Include(c => c.Assignee).
                Include(c => c.Status))));
            Console.WriteLine($"New reply to the request {request} has been sent to {currentSocket.RemoteEndPoint}");
        }
        if (request == Requests.SendUsers.ToString() && wrappedData.IsAuthorized)
        {
            currentSocket.Send(Encoding.Unicode.GetBytes(JsonSerializer.Serialize(context.Users.Include(c => c.Role))));
            Console.WriteLine($"New reply to the request {request} has been sent to {currentSocket.RemoteEndPoint}");
        }
    }
}

void ReceiveRequest(string request, RequestWrapper wrappedData, Socket currentSocket)
{
    Console.WriteLine($"New {request} request from client {currentSocket.RemoteEndPoint}");
    using (Context context = GetContext())
    {
        if (request == Requests.ReceiveCard.ToString() && wrappedData.IsAuthorized)
        {
            Card temp = JsonSerializer.Deserialize<Card>(wrappedData.RequestBody);
            temp.Status = temp.Status == null ? context.Statuses.FirstOrDefault() : temp.Status;
            if (context.Cards.Contains(temp))
                context.Cards.Attach(temp).State = EntityState.Modified;
            else
                context.Cards.Attach(temp).State = EntityState.Added;
            context.SaveChanges();
            Console.WriteLine($"{request} has been processed by the server");
        }
        if (request == Requests.ReceiveUser.ToString() && wrappedData.IsAuthorized)
        {
            User temp = JsonSerializer.Deserialize<User>(wrappedData.RequestBody);
            if (context.Users.Contains(temp))
                context.Users.Attach(temp).State = EntityState.Modified;
            else
                context.Users.Attach(temp).State = EntityState.Added;
            context.SaveChanges();
            Console.WriteLine($"{request} has been processed by the server");
        }
        if (request == Requests.ReceiveStatus.ToString() && wrappedData.IsAuthorized)
        {
            Status temp = JsonSerializer.Deserialize<Status>(wrappedData.RequestBody);
            if (context.Statuses.Contains(temp))
                context.Statuses.Attach(temp).State = EntityState.Modified;
            else
                context.Statuses.Attach(temp).State = EntityState.Added;
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
        if (request == Requests.RemoveCard.ToString() && wrappedData.IsAuthorized)
        {
            Card temp = JsonSerializer.Deserialize<Card>(wrappedData.RequestBody);
            if (context.Cards.Contains(temp))
                context.Cards.Attach(temp).State = EntityState.Deleted;
            context.SaveChanges();
            Console.WriteLine($"{request} has been processed by the server");
        }
        if (request == Requests.RemoveUser.ToString() && wrappedData.IsAuthorized)
        {
            User temp = JsonSerializer.Deserialize<User>(wrappedData.RequestBody);
            if (context.Users.Contains(temp))
                context.Users.Attach(temp).State = EntityState.Deleted;
            context.SaveChanges();
            Console.WriteLine($"{request} has been processed by the server");
        }
        if (request == Requests.RemoveStatus.ToString() && wrappedData.IsAuthorized)
        {
            Status temp = JsonSerializer.Deserialize<Status>(wrappedData.RequestBody);
            if (context.Statuses.Contains(temp))
                context.Statuses.Attach(temp).State = EntityState.Deleted;
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