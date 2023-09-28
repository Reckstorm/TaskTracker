using Models;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace TaskTrackerClient.ViewModel
{
    class StatusVM : BaseVM
    {
        private ObservableCollection<Status> _statuses = new ObservableCollection<Status>();
        public readonly ReadOnlyObservableCollection<Status> PublicStatuses;
        private Status selectedStatus;
        public Status SelectedStatus
        {
            get { return selectedStatus; }
            set
            {
                selectedStatus = value;
                RaisePropertyChanged(nameof(SelectedStatus));
            }
        }

        public StatusVM()
        {
            RequestStatuses();
            PublicStatuses = new ReadOnlyObservableCollection<Status>(_statuses);
        }

        public async Task RequestStatuses()
        {
            if (!socket.Connected) await Connect();
            if (_statuses.Count > 0) _statuses.Clear();
            string req = JsonSerializer.Serialize(new RequestWrapper(Requests.SendStatuses, "", true));
            await socket.SendAsync(Encoding.Unicode.GetBytes(req), SocketFlags.None);
            List<Status> list = new List<Status>();
            byte[] data = new byte[] { };
            while (data.Length == 0)
            {
                data = ReceiveAll(socket);
            }
            list = JsonSerializer.Deserialize<List<Status>>(Encoding.Unicode.GetString(data));
            if (list.Count > 0)
            {
                foreach (Status item in list)
                {
                    _statuses.Add(item);
                }
            }
        }

        public void AddStatus(Status status)
        {
            _statuses.Add(status);
            SelectedStatus = status;
        }

        public void RemoveStatus()
        {
            SendRemoveStatus();
            RequestStatuses();
        }

        public async void SendRemoveStatus()
        {
            if (!socket.Connected) await Connect();
            string req = JsonSerializer.Serialize(new RequestWrapper(Requests.RemoveStatus, JsonSerializer.Serialize(SelectedStatus), true));
            socket.SendAsync(Encoding.Unicode.GetBytes(req), SocketFlags.None);
        }

        public async void SendNewOrUpdatedStatus()
        {
            if (!socket.Connected) await Connect();
            string req = JsonSerializer.Serialize(new RequestWrapper(Requests.ReceiveStatus, JsonSerializer.Serialize(SelectedStatus), true));
            socket.SendAsync(Encoding.Unicode.GetBytes(req), SocketFlags.None);
        }
    }
}
