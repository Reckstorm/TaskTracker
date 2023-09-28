using Models;
using Prism.Commands;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows.Input;
using TaskTrackerClient.Commands;
using TaskTrackerClient.CustomControls;

namespace TaskTrackerClient.ViewModel
{
    public class ColumnVM : BaseVM
    { 
        public ColumnVM(Status status, ObservableCollection<Card> cards)
        {
            Status = status;
            Cards = cards;
            CardTileReceiveCommand = new CardTileReceiveCommand(this);
            CardTileRemovedCommand = new CardTileRemovedCommand(this);
            SaveCardOnDropCommand = new DelegateCommand(this.SaveCardOnDrop);
        }

        public ColumnVM(Status status)
        {
            Status = status;
        }

        public ColumnVM() { }

        private Card? _incomingCard;

        public Card? IncomingCard
        {
            get { return _incomingCard; }
            set 
            {
                _incomingCard = value; 
                _incomingCard.Status = Status;
                RaisePropertyChanged(nameof(IncomingCard));
            }
        }

        private Card? _removedCard;

        public Card? RemovedCard
        {
            get { return _removedCard; }
            set
            {
                _removedCard = value;
                RaisePropertyChanged(nameof(RemovedCard));
            }
        }

        public DelegateCommand SaveCardOnDropCommand { get; }

        public async void SaveCardOnDrop()
        {
            if (!socket.Connected) await Connect();
            string req = JsonSerializer.Serialize(new RequestWrapper(Requests.ReceiveCard, JsonSerializer.Serialize(IncomingCard), true));
            await socket.SendAsync(Encoding.Unicode.GetBytes(req), SocketFlags.None);
        }

        public Status Status { get; set; }
        public ObservableCollection<Card> Cards { get; set; }

        public ICommand CardTileReceiveCommand { get; }
        public ICommand CardTileRemovedCommand { get; }
    }
}
