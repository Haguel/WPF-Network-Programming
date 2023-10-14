using System;
using System.Collections.Generic;
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

namespace network_wpf
{
    public partial class ServerWindow : Window
    {
        private Socket? listenSocket;
        private IPEndPoint? endPoint;
        private LinkedList<ChatMessage> messages;

        public ServerWindow()
        {
            InitializeComponent();
            messages = new();
        }

        private void SwitchServer_Click(object sender, RoutedEventArgs e)
        {
            if (listenSocket == null)
            {
                try
                {
                    IPAddress ip = IPAddress.Parse(HostTextBox.Text);
                    int port = Convert.ToInt32(PortTextBox.Text);

                    endPoint = new(ip, port);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error switch " + ex.Message);

                    return;
                }

                listenSocket = new(
                    AddressFamily.InterNetwork,
                    SocketType.Stream,
                    ProtocolType.Tcp
                    );

                new Thread(StartServer).Start();
            }
            else
            {
                listenSocket.Close();   
            }
        }

        private void StartServer()
        {
            if (listenSocket == null || endPoint == null)
            {
                MessageBox.Show("Invalid Start");
            }

            try
            {
                listenSocket.Bind(endPoint);
                listenSocket.Listen(10);

                // 05.10
                Dispatcher.Invoke(() => StatusLabel.Background = new SolidColorBrush(Colors.Green));
                Dispatcher.Invoke(() => StatusLabel.Content = "Turned on");
                Dispatcher.Invoke(() => ServerLog.Text += "Server started!\n");

                byte[] buffer = new byte[1024];
                while(true)
                {
                    Socket socket = listenSocket.Accept();

                    StringBuilder stringBuilder = new();
                    do
                    {
                        int n = socket.Receive(buffer);

                        stringBuilder.Append(Encoding.UTF8.GetString(buffer, 0, n));
                    } while (socket.Available > 0);
                    String str = stringBuilder.ToString();

                    ServerResponse serverResponse = new();
                    var clientRequest = JsonSerializer.Deserialize<ClientRequest>(str);

                    bool needLog = true;
                    if (clientRequest == null)
                    {
                        str = "Erorr json " + str;
                        serverResponse.Status = "400 Bad Request";
                        //serverResponse.Data = "Error decoding JSON";
                    }
                    else
                    {
                        if (clientRequest.Command.Equals("Message"))
                        {
                            clientRequest.Message.Moment = DateTime.Now;
                            messages.AddLast(clientRequest.Message);

                            str = clientRequest.Message.ToString();
                            serverResponse.Status = "200 OK";
                        } 
                        else if (clientRequest.Command.Equals("Check"))
                        {
                            serverResponse.Status = "200 OK";
                            serverResponse.Messages = messages.Where(m => m.Moment > clientRequest.Message.Moment);
                            needLog = false;
                        }
                    }

                    if (needLog)
                    {
                        Dispatcher.Invoke(() => ServerLog.Text += $"{DateTime.Now} {str}!\n");
                    }

                    String response = "Received " + DateTime.Now;

                    socket.Send(Encoding.UTF8.GetBytes(
                        JsonSerializer.Serialize(serverResponse)));
                    socket.Close();
                }
            }
            catch (Exception ex)
            {
                listenSocket = null;

                // 05.10
                Dispatcher.Invoke(() => StatusLabel.Background = new SolidColorBrush(Colors.Pink));
                Dispatcher.Invoke(() => StatusLabel.Content = "Turned off");
                Dispatcher.Invoke(() => ServerLog.Text += "Server stopped!\n");
            }

            
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            listenSocket?.Close();
        }
    }
}
