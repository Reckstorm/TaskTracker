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
using System.Windows.Threading;
using TaskTrackerClient.View;
using Card = Models.Card;

namespace TaskTrackerClient.ViewModel
{
    public class MainVM : BaseVM
    {
        readonly StatusVM _statusModel = new StatusVM();
        public ReadOnlyObservableCollection<Status> Statuses => _statusModel.PublicStatuses;

        private Status selectedStatus;

        public Status SelectedStatus
        {
            get { return _statusModel.SelectedStatus; }
            set 
            {
                _statusModel.SelectedStatus = value;
                RaisePropertyChanged(nameof(SelectedStatus));
            }
        }


        readonly RoleVM _roleModel = new RoleVM();
        public ReadOnlyObservableCollection<Role> Roles => _roleModel.PublicRoles;

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

        private bool _isUsersListOpen = false;
        public bool IsUsersListOpen
        {
            get { return _isUsersListOpen; }
            set
            {
                _isUsersListOpen = value;
                RaisePropertyChanged(nameof(IsUsersListOpen));
            }
        }

        private bool _isStatusListOpen = false;
        public bool IsStatusListOpen
        {
            get { return _isStatusListOpen; }
            set
            {
                _isStatusListOpen = value;
                RaisePropertyChanged(nameof(IsStatusListOpen));
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

        public ObservableCollection<ColumnVM> ColumnModel { get; set; }

        private void LoadCurrentUser()
        {
            if (Thread.CurrentPrincipal == null) return;
            CurrentUser = Users.First(x => x.Email.Equals(Thread.CurrentPrincipal.Identity.Name));
        }

        public DelegateCommand DarkThemeCommand { get; }
        public DelegateCommand LightThemeCommand { get; }
        public DelegateCommand ViewDetailsCommand { get; }
        public DelegateCommand ToggleUserListVisible { get; }
        public DelegateCommand ToggleStatusListVisible { get; }
        public DelegateCommand SaveCardCommand { get; }
        public DelegateCommand AddCardCommand { get; }
        public DelegateCommand RemoveCardCommand { get; }
        public DelegateCommand SaveUserCommand { get; }
        public DelegateCommand AddUserCommand { get; }
        public DelegateCommand RemoveUserCommand { get; }
        public DelegateCommand SaveStatusCommand { get; }
        public DelegateCommand AddStatusCommand { get; }
        public DelegateCommand RemoveStatusCommand { get; }
        public DelegateCommand RefreshCommand { get; }
        public DelegateCommand<TextBox> SearchCommand { get; }
        public DelegateCommand LogoutCommand { get; }

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
            ToggleUserListVisible = new DelegateCommand(() =>
            {
                IsUsersListOpen = !IsUsersListOpen;
                SelectedUser = null;
            });
            ToggleStatusListVisible = new DelegateCommand(() =>
            {
                IsStatusListOpen = !IsStatusListOpen;
                SelectedStatus = null;
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
            SaveUserCommand = new DelegateCommand(() =>
            {
                _userModel.SendNewOrUpdatedUser();
            });
            AddUserCommand = new DelegateCommand(() =>
            {
                User temp = new User();
                _userModel.AddUser(temp);
                _userModel.SendNewOrUpdatedUser();
                _userModel.RequestUsers();
                SelectedUser = temp;
            });
            RemoveUserCommand = new DelegateCommand(() =>
            {
                _userModel.RemoveUser();
                SelectedUser = Users.LastOrDefault();
            });
            SaveStatusCommand = new DelegateCommand(() =>
            {
                _statusModel.SendNewOrUpdatedStatus();
                CreateColumns();
            });
            AddStatusCommand = new DelegateCommand(() =>
            {
                Status temp = new Status();
                _statusModel.AddStatus(temp);
                _statusModel.SendNewOrUpdatedStatus();
                _statusModel.RequestStatuses();
                SelectedStatus = temp;
            });
            RemoveStatusCommand = new DelegateCommand(() =>
            {
                _statusModel.RemoveStatus();
                SelectedStatus = Statuses.LastOrDefault();
                CreateColumns();
            });
            RefreshCommand = new DelegateCommand(() =>
            {
                _roleModel.RequestRoles();
                _statusModel.RequestStatuses();
                _userModel.RequestUsers();
                RerenderColumns();
            });
            SearchCommand = new DelegateCommand<TextBox>((tb) => 
            {
                if (tb == null) return;
                _cardModel.SearchCommad(tb);
                CreateColumns();
            });
            LogoutCommand = new DelegateCommand(() =>
            {
                if(socket.Connected)
                {
                    socket.Send(Encoding.Unicode.GetBytes(JsonSerializer.Serialize(new RequestWrapper(Requests.ConnectionClose, "", true))));
                    socket.Shutdown(SocketShutdown.Both);
                    socket.Disconnect(true);
                }
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
            if (ColumnModel == null) ColumnModel = new ObservableCollection<ColumnVM>();
            if (ColumnModel.Count > 0) ColumnModel.Clear();
            foreach (Status item in Statuses)
            {
                ColumnModel.Add(new ColumnVM(item, new ObservableCollection<Card>()));
            }

            foreach (Card c in Cards)
            {
                foreach (ColumnVM col in ColumnModel)
                {
                    if (c.Status.Equals(col.Status)) col.Cards.Add(c);
                }
                if(ColumnModel.Any(col => col.Status.Equals(c.Status))) continue;
            }
        }
    }
}
