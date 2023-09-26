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
            //if (!socket.Connected) Connect();
            //socket.Send(Encoding.Unicode.GetBytes(Requests.SendCards.ToString()));
            //List<Card> list = new List<Card>();
            //byte[] data = new byte[] { };
            //while (data.Length == 0)
            //{
            //    data = ReceiveAll(socket);
            //}
            //list = JsonSerializer.Deserialize<List<Card>>(Encoding.Unicode.GetString(data));
            //if (list.Count > 0)
            //{
            //    foreach (Card s in list)
            //    {
            //        _cards.Add(s);
            //    }
            //}
            RefetchCards();
            PublicCards = new ReadOnlyObservableCollection<Card>(_cards);
        }

        public void RefetchCards()
        {
            _cards.Clear();
            if (!socket.Connected) Connect();
            socket.Send(Encoding.Unicode.GetBytes(Requests.SendCards.ToString()));
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

        public void AddCommand(Card card)
        {
            _cards.Add(card);
            //SelectedCard = card;
        }

        public void RemoveCommand(Card card)
        {
            _cards.Remove(card);
            //SendUpdates();
        }

        public void SendUpdates()
        {
            if (!socket.Connected) Connect();
            string requestType = JsonSerializer.Serialize(Requests.ReceiveCard.ToString());
            string requestContent = JsonSerializer.Serialize(SelectedCard);
            socket.Send(Encoding.Unicode.GetBytes($"{requestType}&{requestContent}"));
        }
    }
}
