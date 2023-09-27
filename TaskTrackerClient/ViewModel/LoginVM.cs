using Models;
using Prism.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Security.Principal;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Documents;

namespace TaskTrackerClient.ViewModel
{
    public class LoginVM : BaseVM
    {
        private string _errorStr;
        private string _username;
        private string _password;
        private bool _isVisible = true;
        private BaseVM mainContext;

        public string ErrorStr
        {
            get { return _errorStr; }
            set
            {
                _errorStr = value;
                RaisePropertyChanged(nameof(ErrorStr));
            }
        }
        public string Username
        {
            get { return _username; }
            set
            {
                _username = value;
                RaisePropertyChanged(nameof(Username));
            }
        }
        public string Password
        {
            get { return _password; }
            set
            {
                _password = value;
                RaisePropertyChanged(nameof(Password));
            }
        }
        public bool IsVisible
        {
            get { return _isVisible; }
            set
            {
                _isVisible = value;
                RaisePropertyChanged(nameof(IsVisible));
            }
        }
        public bool IsOpen { get; set; }

        public BaseVM MainContext 
        { 
            get => mainContext;
            set
            {
                mainContext = value;
                RaisePropertyChanged(nameof(MainContext));
            }
        }

        public DelegateCommand LoginCommand { get; }
        public DelegateCommand ExitCommand { get; }

        public LoginVM()
        {
            MainContext = this;
            ExitCommand = new DelegateCommand(() =>
            {
                if (socket.Connected) socket.Disconnect(true);
                Application.Current.Shutdown();
            });
            LoginCommand = new DelegateCommand(() =>
            {
                if (!socket.Connected) Connect();
                RequestUser();
                byte[] data = new byte[] { };
                while (data.Length == 0)
                {
                    data = ReceiveAll(socket);
                }
                User temp = JsonSerializer.Deserialize<User>(Encoding.Unicode.GetString(data));
                if (temp.Id == 0 || temp.Role == null)
                {
                    ErrorStr = "Invalid Data";
                    return;
                }
                Thread.CurrentPrincipal = new GenericPrincipal(new GenericIdentity(Username), null);
                IsVisible = false;
            });
        }

        private async Task RequestUser()
        {
            string req = JsonSerializer.Serialize(new RequestWrapper(Requests.SendUser, "", Username, Password));
            await socket.SendAsync(Encoding.Unicode.GetBytes(req), SocketFlags.None);
        }
    }
}
