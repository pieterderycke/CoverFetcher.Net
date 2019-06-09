using CoverFetcher.Messages;
using GalaSoft.MvvmLight.Messaging;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
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
        public IConfigurationRoot Configuration { get; private set; }

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

        private void OnStartup(object sender, StartupEventArgs e)
        {
            string path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "CoverFetcher", "settings.json");

            this.Configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(new Dictionary<string, string>
                    {
                        {"ExportSettings:DefaultFileNamePattern", "{artist} - {album}.jpg"}
                    })
                .AddJsonFile(path, optional: true)
                .Build();
        }
    }
}
