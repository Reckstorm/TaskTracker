using MaterialDesignThemes.Wpf;
using Models;
using Prism.Commands;
using Prism.Mvvm;
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
using TaskTrackerClient.View;
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
            set 
            { 
                _userModel.SelectedUser = value;
                RaisePropertyChanged(nameof(SelectedUser));
            }
        }

        private User? currentUser;
        public User? CurrentUser
        {
            get { return currentUser; }
            set 
            { 
                currentUser = value;
                RaisePropertyChanged(nameof(CurrentUser));
            }
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

        private void LoadCurrentUser()
        {
            if (Thread.CurrentPrincipal == null) return;
            CurrentUser = Users.First(x => x.Email.Equals(Thread.CurrentPrincipal.Identity.Name));
        }

        public DelegateCommand DarkThemeCommand { get; }
        public DelegateCommand LightThemeCommand { get; }
        public DelegateCommand ViewDetailsCommand { get; }
        public DelegateCommand SaveCardCommand { get; }
        public DelegateCommand AddCardCommand { get; }
        public DelegateCommand RemoveCardCommand { get; }
        public DelegateCommand LogoutCommand { get; }
        public ObservableCollection<ColumnVM> Columns { get; set; }

        public MainVM()
        {
            LoadCurrentUser();
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
                _cardModel.SendNewOrUpdatedCard();
                ToggleModalAndRerenderColumns();
            });
            AddCardCommand = new DelegateCommand(() =>
            {
                Card temp = new Card();
                temp.DateTimeCreated = DateTime.Now;
                temp.DateTimeModified = DateTime.Now;
                SelectedCard = temp;
                IsOpen = !IsOpen;
            });
            RemoveCardCommand = new DelegateCommand(() =>
            {
                _cardModel.SendRemoveCard();
                ToggleModalAndRerenderColumns();
            });
            LogoutCommand = new DelegateCommand(() =>
            {
                socket.Shutdown(SocketShutdown.Both);
                socket.Disconnect(true);
                Thread.CurrentPrincipal = null;
                new Login().Show();
                Application.Current.MainWindow.Close();
            });
        }

        public void ToggleModalAndRerenderColumns()
        {
            IsOpen = !IsOpen;
            RerenderColumns();
        }

        public void RerenderColumns()
        {
            SelectedCard = null;
            _cardModel.RefetchCards();
            CreateColumns();
        }

        public void CreateColumns()
        {
            if (Columns == null) Columns = new ObservableCollection<ColumnVM>();
            if (Columns.Count > 0) Columns.Clear();
            foreach (Status item in Statuses)
            {
                Columns.Add(new ColumnVM(item, new ObservableCollection<Card>()));
            }

            foreach (Card c in Cards)
            {
                foreach (ColumnVM col in Columns)
                {
                    if (c.Status.Equals(col.Status)) col.Cards.Add(c);
                }
            }
        }
    }
}
