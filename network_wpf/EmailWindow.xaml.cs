using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Mail;
using System.Net.Mime;
using System.Text;
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
    /// <summary>
    /// Логика взаимодействия для EmailWindow.xaml
    /// </summary>
    public partial class EmailWindow : Window
    {
        public EmailWindow()
        {
            InitializeComponent();
        }

        private SmtpClient GetSmtpClient()
        {
            String? host = App.GetConfiguration("smtp:host");

            if (host == null)
            {
                MessageBox.Show("error getting host");
                return null;
            }

            String? portString = App.GetConfiguration("smtp:port");

            if (portString == null)
            {
                MessageBox.Show("error getting port");
                return null;
            }

            int port = 0;
            try { port = int.Parse(portString); }
            catch
            {
                MessageBox.Show("Error parsing port");
            }

            String? email = App.GetConfiguration("smtp:email");

            if (email == null)
            {
                MessageBox.Show("error getting email");
                return null;
            }

            String? password = App.GetConfiguration("smtp:password");

            if (password == null)
            {
                MessageBox.Show("error getting password");
                return null;
            }

            String? sslString = App.GetConfiguration("smtp:ssl");

            if (sslString == null)
            {
                MessageBox.Show("error getting ssl");
                return null;
            }

            bool ssl = false;
            try { ssl = bool.Parse(sslString); }
            catch
            {
                MessageBox.Show("Error parsing ssl");
            }

            if (!textBoxTo.Text.Contains("@"))
            {
                MessageBox.Show("Enter proper email");

                return null;
            }

            return new(host, port)
            {
                EnableSsl = ssl,
                Credentials = new NetworkCredential(email, password)
            };      
        }

        private void SendButton1_Click(object sender, RoutedEventArgs e)
        {
        }

        private void SendButton2_Click(object sender, RoutedEventArgs e)
        {
            SmtpClient smtpClient = GetSmtpClient();
            if (smtpClient == null) return;

            MailMessage mailMessage = new(
                App.GetConfiguration("smtp:email"),
                textBoxTo.Text, textBoxSubject.Text, textBoxHtml.Text)
            {
                IsBodyHtml = true
            };

            ContentType pngType = new("image/png");
            mailMessage.Attachments.Add(new Attachment("coin25.png", pngType));

            ContentType mp3Type = new("audio/mpeg");
            mailMessage.Attachments.Add(new Attachment("Jump_01.mp3", mp3Type));

            smtpClient.Send(mailMessage);

            MessageBox.Show("Sent");

        }

        private void SendButton3_Click(object sender, RoutedEventArgs e)
        {
            // hw 13.10
            Random random = new Random();
            int number = random.Next(0, 1000000);
            string code = number.ToString("D6");
            string htmlBody = $"<p>Це тестовий лист з HTML-кодом.</p><p>Ваш код: <b style='color:tomato'>{code}</b></p>";

            SmtpClient smtpClient = GetSmtpClient();
            if (smtpClient == null) return;

            MailMessage mailMessage = new(
                App.GetConfiguration("smtp:email"),
                textBoxTo.Text, textBoxSubject.Text, htmlBody)
            {
                IsBodyHtml = true
            };

            ContentType txtType = new("txt/plain");
            mailMessage.Attachments.Add(new Attachment("privacy.txt", txtType));

            smtpClient.Send(mailMessage);

            MessageBox.Show("Sent");
        }
    }
}
