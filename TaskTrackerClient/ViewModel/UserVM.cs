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
            RequestUsers();
            PublicUsers = new ReadOnlyObservableCollection<User>(_users);
        }

        public async Task RequestUsers()
        {
            if (!socket.Connected) await Connect();
            if(_users.Count > 0) _users.Clear();
            string req = JsonSerializer.Serialize(new RequestWrapper(Requests.SendUsers, "", true));
            await socket.SendAsync(Encoding.Unicode.GetBytes(req), SocketFlags.None);
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
        }

        public void AddUser(User user)
        {
            _users.Add(user);
            SelectedUser = user;
        }

        public void RemoveUser()
        {
            SendRemoveUser();
            RequestUsers();
        }

        public async void SendRemoveUser()
        {
            if (!socket.Connected) await Connect();
            string req = JsonSerializer.Serialize(new RequestWrapper(Requests.RemoveUser, JsonSerializer.Serialize(SelectedUser), true));
            socket.SendAsync(Encoding.Unicode.GetBytes(req), SocketFlags.None);
        }

        public async void SendNewOrUpdatedUser()
        {
            if (!socket.Connected) await Connect();
            string req = JsonSerializer.Serialize(new RequestWrapper(Requests.ReceiveUser, JsonSerializer.Serialize(SelectedUser), true));
            socket.SendAsync(Encoding.Unicode.GetBytes(req), SocketFlags.None);
        }
    }
}
