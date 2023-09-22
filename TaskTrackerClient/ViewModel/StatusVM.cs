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
            if(!socket.Connected) Connect();
            socket.Send(Encoding.Unicode.GetBytes(Requests.SendStatuses.ToString()));
            List<Status> list = new List<Status>();
            byte[] data = new byte[] { };
            while (data.Length == 0)
            {
                data = ReceiveAll(socket);
            }
            list = JsonSerializer.Deserialize<List<Status>>(Encoding.Unicode.GetString(data));
            if (list.Count > 0)
            {
                foreach (Status s in list)
                {
                    _statuses.Add(s);
                }
            }
            PublicStatuses = new ReadOnlyObservableCollection<Status>(_statuses);
        }

        public void AddCommand(Status status)
        {
            _statuses.Add(status);
            SelectedStatus = status;
            SendUpdates();
        }

        public void RemoveCommand(Status status)
        {
            _statuses.Remove(status);
            SendUpdates();
        }

        public void SendUpdates()
        {
            if (!socket.Connected) Connect();
            socket.Send(Encoding.Unicode.GetBytes(JsonSerializer.Serialize(_statuses)));
        }
    }
}
