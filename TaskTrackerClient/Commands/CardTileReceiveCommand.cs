using Models;
using Prism.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using TaskTrackerClient.ViewModel;

namespace TaskTrackerClient.Commands
{
    internal class CardTileReceiveCommand : BaseCommand
    {
        private readonly ColumnVM _column;

        public CardTileReceiveCommand(ColumnVM column)
        {
            _column = column;
        }

        public override void Execute(object? parameter)
        {
            _column.Cards.Add(_column.IncomingCard);
        }
    }
}
