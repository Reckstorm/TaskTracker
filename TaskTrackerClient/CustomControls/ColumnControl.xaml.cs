using Models;
using Prism.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace TaskTrackerClient.CustomControls
{
    /// <summary>
    /// Interaction logic for ColumnControl.xaml
    /// </summary>
    public partial class ColumnControl : UserControl
    {
        public bool IsCardHitTestVisible
        {
            get { return (bool)GetValue(IsCardHitTestVisibleProperty); }
            set { SetValue(IsCardHitTestVisibleProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IsCardHitTestVisible.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsCardHitTestVisibleProperty =
            DependencyProperty.Register("IsCardHitTestVisible", typeof(bool), typeof(ColumnControl), new PropertyMetadata(true));


        public object IncomingCard
        {
            get { return (object)GetValue(IncomingCardProperty); }
            set { SetValue(IncomingCardProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IncomingCardTile.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IncomingCardProperty =
            DependencyProperty.Register("IncomingCard", typeof(object), typeof(ColumnControl), 
                new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));


        public ICommand CardTileDropCommand
        {
            get { return (ICommand)GetValue(CardTileDropCommandProperty); }
            set { SetValue(CardTileDropCommandProperty, value); }
        }

        // Using a DependencyProperty as the backing store for CardTileDropCommand.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CardTileDropCommandProperty =
            DependencyProperty.Register("CardTileDropCommand", typeof(ICommand), typeof(ColumnControl), new PropertyMetadata(null));


        public object RemovedCard
        {
            get { return (object)GetValue(RemovedCardProperty); }
            set { SetValue(RemovedCardProperty, value); }
        }

        // Using a DependencyProperty as the backing store for RemovedCardTile.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty RemovedCardProperty =
            DependencyProperty.Register("RemovedCard", typeof(object), typeof(ColumnControl), 
                new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));


        public ICommand CardTileRemovedCommand
        {
            get { return (ICommand)GetValue(CardTileRemovedCommandProperty); }
            set { SetValue(CardTileRemovedCommandProperty, value); }
        }

        // Using a DependencyProperty as the backing store for CardTileRemovedCommand.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CardTileRemovedCommandProperty =
            DependencyProperty.Register("CardTileRemovedCommand", typeof(ICommand), typeof(ColumnControl), new PropertyMetadata(null));


        public ColumnControl()
        {
            InitializeComponent();
        }

        private void CardTile_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed && sender is FrameworkElement frameworkElement)
            {
                DragDrop.DoDragDrop(frameworkElement, new DataObject(DataFormats.Serializable, frameworkElement.DataContext), DragDropEffects.Move);
            }
        }

        private void ListBox_Drop(object sender, DragEventArgs e)
        {
            if (CardTileDropCommand?.CanExecute(null) ?? false)
            {
                IsCardHitTestVisible = false;
                IncomingCard = e.Data.GetData(DataFormats.Serializable);
                CardTileDropCommand?.Execute(null);
                IsCardHitTestVisible = true;
            }
        }

        private void ListBox_DragLeave(object sender, DragEventArgs e)
        {
            if (CardTileRemovedCommand?.CanExecute(null) ?? false)
            {
                RemovedCard = e.Data.GetData(DataFormats.Serializable);
                CardTileRemovedCommand?.Execute(null);
            }
        }
    }
}
