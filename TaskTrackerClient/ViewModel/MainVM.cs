using MaterialDesignThemes.Wpf;
using Models;
using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using Card = Models.Card;

namespace TaskTrackerClient.ViewModel
{
    public class MainVM : BaseVM
    {
        readonly StatusVM _statusModel = new StatusVM();
        public ReadOnlyObservableCollection<Status> Statuses => _statusModel.PublicStatuses;

        readonly CardVM _cardModel = new CardVM();
        private Card? selectedCard;
        public ReadOnlyObservableCollection<Card> Cards => _cardModel.PublicCards;
        public Card? SelectedCard
        {
            get { return _cardModel.SelectedCard; }
            set 
            { 
                _cardModel.SelectedCard = value; 
                RaisePropertyChanged(nameof(SelectedCard)); 
            }
        }

        readonly UserVM _userModel = new UserVM();
        private User? selectedUser;
        public ReadOnlyObservableCollection<User> Users => _userModel.PublicUsers;
        public User? SelectedUser
        {
            get { return _userModel.SelectedUser; }
            set { _userModel.SelectedUser = value; }
        }
        private bool _isOpen = false;
        public bool IsOpen
        {
            get { return _isOpen; }
            set
            {
                _isOpen = value;
                RaisePropertyChanged(nameof(IsOpen));
            }
        }

        private bool _isDark = true;
        private string _themeName = "SystemTheme";
        public bool IsDark
        {
            get { return _isDark; }
            set
            {
                _isDark = value;
                RaisePropertyChanged(nameof(IsDark));
            }
        }
        public string ThemeName
        {
            get { return _themeName; }
            set
            {
                _themeName = value;
                RaisePropertyChanged(nameof(ThemeName));
            }
        }

        public DelegateCommand DarkThemeCommand { get; }
        public DelegateCommand LightThemeCommand { get; }
        public DelegateCommand ViewDetailsCommand { get; }
        public DelegateCommand SaveCardCommand { get; }
        public DelegateCommand AddCardCommand { get; }
        public ObservableCollection<Column> Columns { get; set; }

        public MainVM()
        {
            CreateColumns();

            DarkThemeCommand = new DelegateCommand(() =>
            {
                PaletteHelper paletteHelper = new PaletteHelper();
                ITheme theme = paletteHelper.GetTheme();
                theme.SetBaseTheme(new MaterialDesignDarkTheme());
                paletteHelper.SetTheme(theme);
                ThemeName = "DarkTheme";
            });
            LightThemeCommand = new DelegateCommand(() =>
            {
                PaletteHelper paletteHelper = new PaletteHelper();
                ITheme theme = paletteHelper.GetTheme();
                theme.SetBaseTheme(new MaterialDesignLightTheme());
                paletteHelper.SetTheme(theme);
                ThemeName = "LightTheme";
            });
            ViewDetailsCommand = new DelegateCommand(() =>
            {
                IsOpen = !IsOpen;
                SelectedCard = null;
            });
            SaveCardCommand = new DelegateCommand(() =>
            {
                _cardModel.SendUpdates();
                IsOpen = !IsOpen;
                SelectedCard = null;
                _cardModel.RefetchCards();
                CreateColumns();
            });
            AddCardCommand = new DelegateCommand(() =>
            {
                Card temp = new Card();
                temp.DateTimeCreated = DateTime.Now;
                temp.DateTimeModified = DateTime.Now;
                SelectedCard = temp;
                _cardModel.AddCommand(temp);
                IsOpen = true;
            });
        }

        public void CreateColumns()
        {
            if(Columns == null) Columns = new ObservableCollection<Column>();
            if(Columns.Count > 0) Columns.Clear();
            foreach (Status item in Statuses)
            {
                Columns.Add(new Column(item, new ObservableCollection<Card>()));
            }

            foreach (Card c in Cards)
            {
                foreach (Column col in Columns)
                {
                    if (c.Status.Equals(col.Status)) col.Cards.Add(c);
                }
            }
        }
    }
}
