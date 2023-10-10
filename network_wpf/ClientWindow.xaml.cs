﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
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
using System.Xml.Schema;

namespace network_wpf
{
    /// <summary>
    /// Логика взаимодействия для ClientWindow.xaml
    /// </summary>
    public partial class ClientWindow : Window
    {
        private Random random = new();
        private IPEndPoint? endPoint;

        public ClientWindow()
        {
            InitializeComponent();
        }

        private void SendButton_Click(object sender, RoutedEventArgs e)
        {
            String[] address = HostTextBox.Text.Split(":");

            try
            {
                endPoint = new
                    (
                        IPAddress.Parse(address[0]),
                        Convert.ToInt32(address[1])
                    );

                new Thread(SendMessage).Start(
                    new ClientRequest
                    {
                        Command = "Message",
                        Data = LoginTextBox.Text + ": " + MessageTextBox.Text
                    }
                    );
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            LoginTextBox.Text = "User" + random.Next(100);
            MessageTextBox.Text = "Hello World!";    
        }

        private void SendMessage(Object? arg)
        {
            var clientRequest = arg as ClientRequest;

            if (endPoint == null || clientRequest == null)
            {
                return;
            }

            Socket socket = new(
                AddressFamily.InterNetwork,
                SocketType.Stream,
                ProtocolType.Tcp
                );

            try
            {
                socket.Connect(endPoint);
                socket.Send(
                    Encoding.UTF8.GetBytes(JsonSerializer.Serialize(clientRequest))
                    );

                MemoryStream memoryStream = new();
                byte[] buffer = new byte[1024];

                do
                {
                    int n = socket.Receive(buffer);
                    memoryStream.Write(buffer, 0, n);
                } while (socket.Available > 0);

                String str = Encoding.UTF8.GetString(memoryStream.ToArray());

                ServerResponse? response = null;
                try 
                {
                    response = JsonSerializer.Deserialize<ServerResponse>(str);
                }
                catch { }

                // hw 06.10
                if (response == null)
                {
                    str = "JSON Error in " + str;
                    Dispatcher.Invoke(() => ClientLog.Background = new SolidColorBrush(Colors.Pink));
                }
                else
                {
                    str = response.Status;
                    Dispatcher.Invoke(() => ClientLog.Background = new SolidColorBrush(Colors.Green));
                }

                Dispatcher.Invoke(() => ClientLog.Text += str + "\n");

                socket.Shutdown(SocketShutdown.Both);
                socket.Dispose();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}
