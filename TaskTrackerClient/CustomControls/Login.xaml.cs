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
    /// Interaction logic for Login.xaml
    /// </summary>
    public partial class Login : UserControl
    {
        public bool IsLoginOpen
        {
            get { return (bool)GetValue(IsLoginOpenProperty); }
            set { SetValue(IsLoginOpenProperty, value); }
        }

        public static readonly DependencyProperty IsLoginOpenProperty =
            DependencyProperty.Register("IsLoginOpen", typeof(bool), typeof(Login), new PropertyMetadata(true));


        public Login()
        {
            InitializeComponent();
        }
    }
}
