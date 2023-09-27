using Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace TaskTrackerClient.ViewModel
{
    class CardVM : BaseVM
    {
        private ObservableCollection<Card> _cards = new ObservableCollection<Card>();
        public readonly ReadOnlyObservableCollection<Card> PublicCards;
        private Card? selectedCard;
        public Card? SelectedCard
        {
            get { return selectedCard; }
            set 
            { 
                selectedCard = value; 
                RaisePropertyChanged(nameof(SelectedCard));
            }
        }

        public CardVM()
        {
            RefetchCards();
            PublicCards = new ReadOnlyObservableCollection<Card>(_cards);
        }

        public async void RefetchCards()
        {
            _cards.Clear();
            if (!socket.Connected) Connect();
            //socket.Send(Encoding.Unicode.GetBytes(Requests.SendCards.ToString()));
            RequestCards();
            List<Card> list = new List<Card>();
            byte[] data = new byte[] { };
            while (data.Length == 0)
            {
                data = ReceiveAll(socket);
            }
            list = JsonSerializer.Deserialize<List<Card>>(Encoding.Unicode.GetString(data));
            if (list.Count > 0)
            {
                foreach (Card s in list)
                {
                    _cards.Add(s);
                }
            }
        }

        private async Task RequestCards()
        {
            string req = JsonSerializer.Serialize(new RequestWrapper(Requests.SendCards, ""));
            await socket.SendAsync(Encoding.Unicode.GetBytes(req), SocketFlags.None);
        }

        public void SendRemoveCard()
        {
            if (!socket.Connected) Connect();
            //string requestType = JsonSerializer.Serialize(Requests.RemoveCard.ToString());
            //string requestContent = JsonSerializer.Serialize(SelectedCard);
            //socket.Send(Encoding.Unicode.GetBytes($"{requestType}&{requestContent}"));
            string req = JsonSerializer.Serialize(new RequestWrapper(Requests.RemoveCard, JsonSerializer.Serialize(SelectedCard)));
            socket.SendAsync(Encoding.Unicode.GetBytes(req), SocketFlags.None);
        }

        public void SendNewOrUpdatedCard()
        {
            if (!socket.Connected) Connect();
            //string requestType = JsonSerializer.Serialize(Requests.ReceiveCard.ToString());
            //string requestContent = JsonSerializer.Serialize(SelectedCard);
            //socket.Send(Encoding.Unicode.GetBytes($"{requestType}&{requestContent}"));
            string req = JsonSerializer.Serialize(new RequestWrapper(Requests.ReceiveCard, JsonSerializer.Serialize(SelectedCard)));
            socket.SendAsync(Encoding.Unicode.GetBytes(req), SocketFlags.None);
        }
    }
}
