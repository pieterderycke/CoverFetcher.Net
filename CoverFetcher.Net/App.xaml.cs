using CoverFetcher.Messages;
using GalaSoft.MvvmLight.Messaging;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace CoverFetcher
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public App()
        {
            Messenger.Default.Register<ShowErrorMessage>(this, OnShowErrorMessageReceived);
        }

        private void OnShowErrorMessageReceived(ShowErrorMessage message)
        {
            Application.Current.Dispatcher.Invoke((Action)(() =>
            {
                MessageBox.Show(this.MainWindow, message.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }));
        }
    }
}
