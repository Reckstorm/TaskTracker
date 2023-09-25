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
        private Status? selectedStatus;
        public ReadOnlyObservableCollection<Status> Statuses => _statusModel.PublicStatuses;
        public Status SelectedStatus
        {
            get { return _statusModel.SelectedStatus; }
            set { _statusModel.SelectedStatus = value; }
        }

        readonly CardVM _cardModel = new CardVM();
        private Card? selectedCard;
        public ReadOnlyObservableCollection<Card> Cards => _cardModel.PublicCards;
        public Card SelectedCard
        {
            get { return _cardModel.SelectedCard; }
            set { _cardModel.SelectedCard = value; }
        }

        readonly UserVM _userModel = new UserVM();
        private User? selectedUser;
        public ReadOnlyObservableCollection<User> Users => _userModel.PublicUsers;
        public User SelectedUser
        {
            get { return _userModel.SelectedUser; }
            set { _userModel.SelectedUser = value; }
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

        public ObservableCollection<Dictionary<Status, ObservableCollection<Card>>> columns { get; set; }

        public ObservableCollection<Column> Columns { get; set; }

        public MainVM()
        {
            Columns = new ObservableCollection<Column>();
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
        }
    }
}
