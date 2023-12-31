﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace TaskTrackerClient.View
{
    /// <summary>
    /// Interaction logic for Login.xaml
    /// </summary>
    public partial class Login : Window
    {
        public Login()
        {
            InitializeComponent();
            this.IsVisibleChanged += (s, e) =>
            {
                if (this.IsVisible == false && this.IsLoaded && Thread.CurrentPrincipal != null)
                {
                    Application.Current.MainWindow = new MainWindow();
                    Application.Current.MainWindow.Show();
                    this.Close();
                }
            };
        }
    }
}
