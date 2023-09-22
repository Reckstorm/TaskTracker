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
        private Status? selectedCard;
        public ReadOnlyObservableCollection<Card> Cards => _cardModel.PublicCards;
        public Card SelectedCard
        {
            get { return _cardModel.SelectedCard; }
            set { _cardModel.SelectedCard = value; }
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

        public MainVM()
        {
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
