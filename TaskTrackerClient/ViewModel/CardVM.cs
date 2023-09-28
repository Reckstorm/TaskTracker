using MaterialDesignThemes.Wpf;
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
using System.Windows;
using System.Windows.Controls;
using Card = Models.Card;

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
            if (!socket.Connected) await Connect();
            if (_cards.Count > 0) _cards.Clear();
            string req = JsonSerializer.Serialize(new RequestWrapper(Requests.SendCards, "", true));
            await socket.SendAsync(Encoding.Unicode.GetBytes(req), SocketFlags.None);
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

        public async void SearchCommad(TextBox tb)
        {
            _cards.Clear();
            if (string.IsNullOrWhiteSpace(tb.Text)) RefetchCards();
            string temp = tb.Text.ToLower();
            if (!socket.Connected) await Connect();
            string req = JsonSerializer.Serialize(new RequestWrapper(Requests.SearchCard, temp, true));
            socket.SendAsync(Encoding.Unicode.GetBytes(req), SocketFlags.None);
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

        public async void SendRemoveCard()
        {
            if (!socket.Connected) await Connect();
            string req = JsonSerializer.Serialize(new RequestWrapper(Requests.RemoveCard, JsonSerializer.Serialize(SelectedCard), true));
            socket.SendAsync(Encoding.Unicode.GetBytes(req), SocketFlags.None);
        }

        public async void SendNewOrUpdatedCard()
        {
            if (!socket.Connected) await Connect();
            string req = JsonSerializer.Serialize(new RequestWrapper(Requests.ReceiveCard, JsonSerializer.Serialize(SelectedCard), true));
            socket.SendAsync(Encoding.Unicode.GetBytes(req), SocketFlags.None);
        }
    }
}
