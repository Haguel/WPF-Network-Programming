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

namespace network_wpf
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void ServerButton_Click(object sender, RoutedEventArgs e)
        {
            new ServerWindow().Show();
        }

        private void ClientButton_Click(object sender, RoutedEventArgs e)
        {
            new ClientWindow().Show();
        }

        private void EmailButton_Click(object sender, RoutedEventArgs e)
        {
            new EmailWindow().ShowDialog();
        }

        private void ConfirmButton_Click(object sender, RoutedEventArgs e)
        {
            new AuthWindow().ShowDialog();
        }

        private void Http_Click(object sender, RoutedEventArgs e)
        {
            new HttpWindow().ShowDialog();
        }

        private void Crypto_Click(object sender, RoutedEventArgs e)
        {
            new CryptoWindow().ShowDialog();
        }
    }
}
