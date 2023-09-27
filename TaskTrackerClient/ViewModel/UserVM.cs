using Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace TaskTrackerClient.ViewModel
{
    class UserVM : BaseVM
    {
        private ObservableCollection<User> _users = new ObservableCollection<User>();
        public readonly ReadOnlyObservableCollection<User> PublicUsers;
        private User? selectedUser;
        public User? SelectedUser
        {
            get { return selectedUser; }
            set
            {
                selectedUser = value;
                RaisePropertyChanged(nameof(SelectedUser));
            }
        }

        public UserVM()
        {
            if (!socket.Connected) Connect();
            RequestUsers();
            List<User> list = new List<User>();
            byte[] data = new byte[] { };
            while (data.Length == 0)
            {
                data = ReceiveAll(socket);
            }
            list = JsonSerializer.Deserialize<List<User>>(Encoding.Unicode.GetString(data));
            if (list.Count > 0)
            {
                foreach (User u in list)
                {
                    _users.Add(u);
                }
            }
            PublicUsers = new ReadOnlyObservableCollection<User>(_users);
            //SelectedUser = _users[0];
        }

        private async Task RequestUsers()
        {
            string req = JsonSerializer.Serialize(new RequestWrapper(Requests.SendUsers, ""));
            await socket.SendAsync(Encoding.Unicode.GetBytes(req), SocketFlags.None);
        }

        public void AddCommand(User user)
        {
            _users.Add(user);
            SelectedUser = user;
            SendUpdates();
        }

        public void RemoveCommand(User user)
        {
            _users.Remove(user);
            SendUpdates();
        }

        public void SendUpdates()
        {
            if (!socket.Connected) Connect();
            socket.Send(Encoding.Unicode.GetBytes(JsonSerializer.Serialize(_users)));
        }
    }
}
