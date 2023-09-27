using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using TaskTrackerClient.ViewModel;

namespace TaskTrackerClient.Commands
{
    internal class CardTileRemovedCommand : BaseCommand
    {
        private readonly ColumnVM _column;

        public CardTileRemovedCommand(ColumnVM column)
        {
            _column = column;
        }

        public override void Execute(object? parameter)
        {
            _column.Cards.Remove(_column.RemovedCard);
        }
    }
}
