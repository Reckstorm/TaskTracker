using Models;
using Prism.Commands;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskTrackerClient.ViewModel
{
    public class Column : BaseVM
    { 
        public Column(Status status, ObservableCollection<Card> cards)
        {
            Status = status;
            Cards = cards; 
        }

        public Column(Status status)
        {
            Status = status;
        }

        public Column() { }

        public Status Status { get; set; }
        public ObservableCollection<Card> Cards { get; set; }
    }
}
