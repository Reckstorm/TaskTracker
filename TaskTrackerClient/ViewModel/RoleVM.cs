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
    class RoleVM : BaseVM
    {
        private ObservableCollection<Role> _roles = new ObservableCollection<Role>();
        public readonly ReadOnlyObservableCollection<Role> PublicRoles;
        private Status selectedRole;
        public Status SelectedRole
        {
            get { return selectedRole; }
            set
            {
                selectedRole = value;
                RaisePropertyChanged(nameof(SelectedRole));
            }
        }

        public RoleVM()
        {
            RequestRoles();
            PublicRoles = new ReadOnlyObservableCollection<Role>(_roles);
        }

        public async Task RequestRoles()
        {
            if (!socket.Connected) await Connect();
            if (_roles.Count > 0) _roles.Clear();
            string req = JsonSerializer.Serialize(new RequestWrapper(Requests.SendRoles, "", true));
            await socket.SendAsync(Encoding.Unicode.GetBytes(req), SocketFlags.None);
            List<Role> list = new List<Role>();
            byte[] data = new byte[] { };
            while (data.Length == 0)
            {
                data = ReceiveAll(socket);
            }
            list = JsonSerializer.Deserialize<List<Role>>(Encoding.Unicode.GetString(data));
            if (list.Count > 0)
            {
                foreach (Role item in list)
                {
                    _roles.Add(item);
                }
            }
        }

        public async void SendNewOrUpdatedRole()
        {
            if (!socket.Connected) await Connect();
            string req = JsonSerializer.Serialize(new RequestWrapper(Requests.ReceiveStatus, JsonSerializer.Serialize(SelectedRole), true));
            socket.SendAsync(Encoding.Unicode.GetBytes(req), SocketFlags.None);
        }
    }
}
