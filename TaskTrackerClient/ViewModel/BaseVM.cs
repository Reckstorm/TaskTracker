﻿using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace TaskTrackerClient.ViewModel
{
    public abstract class BaseVM : BindableBase
    {
        protected const int PORT = 4444;
        protected const string IP = "127.0.0.1";
        static public Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

        protected async Task Connect()
        {
            socket.SendTimeout = 200;
            socket.ReceiveTimeout = 200;
            try
            {
                await socket.ConnectAsync(IP, PORT);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                Environment.Exit(0);
            }
        }

        protected byte[] ReceiveAll(Socket socket)
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
    }
}
