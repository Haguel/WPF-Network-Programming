using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
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
    public partial class HttpWindow : Window
    {
        private List<NbuRate>? rates;

        public HttpWindow()
        {
            InitializeComponent();
        }

        private async void get1Button_Click(object sender, RoutedEventArgs e)
        {
            using HttpClient httpClient = new();
            var response = await httpClient.GetAsync("https://itstep.org/");
            textBlock1.Text = "";
            textBlock1.Text += (int)response.StatusCode + " " + response.ReasonPhrase + "\r\n";

            foreach (var header in response.Headers)
            {
                textBlock1.Text += $"{header.Key, 20}: " + "| " + 
                    String.Join(',', header.Value).Ellipsis(40) + "\r\n";
            }

            String body = await response.Content.ReadAsStringAsync();
            textBlock1.Text += $"\r\n{body}";
        }

        private async void ratesButton_Click(object sender, RoutedEventArgs e)
        {
            if (rates == null)
            {
                await loadRatesAsync();

            }
            if (rates == null)
            {
                MessageBox.Show("Error deserializing");
                return;
            }

            foreach (var rate in rates)
            {
                textBlock1.Text += $"{rate.cc} {rate.txt} {rate.rate}\n";
            }
        }

        private async Task loadRatesAsync()
        {
            using HttpClient httpClient = new();
            String body = await httpClient.GetStringAsync(@"https://bank.gov.ua/NBUStatService/v1/statdirectory/exchange?json");

            rates = JsonSerializer.Deserialize<List<NbuRate>>(body);
        }

        private async void popularButton_Click(object sender, RoutedEventArgs e)
        {
            using HttpClient httpClient = new();
            String body = await httpClient.GetStringAsync(@"https://bank.gov.ua/NBUStatService/v1/statdirectory/exchange?json");

            var rates = JsonSerializer.Deserialize<List<NbuRate>>(body);

            if (rates == null)
            {
                MessageBox.Show("Error deserializing");
            }
            foreach (var rate in rates)
            {
                if (rate.cc == "XAU" || rate.cc == "USD" || rate.cc == "EUR")
                { 
                    textBlock1.Text += $"{rate.cc} {rate.txt} {rate.rate}\n";
                }
            }
        }

        private async void nbuJson_Click(object sender, RoutedEventArgs e)
        {
            using HttpClient httpClient = new();
            String body = await httpClient.GetStringAsync(@"https://bank.gov.ua/NBUStatService/v1/statdirectory/exchange?json");

            textBlock1.Text += body;
        }

        private async void nbuXaml_Click(object sender, RoutedEventArgs e)
        {
            using HttpClient httpClient = new();
            String body = await httpClient.GetStringAsync(@"https://bank.gov.ua/NBUStatService/v1/statdirectory/exchange");

            textBlock1.Text += body;
        }
    }

    public static class EllipsisExtensions
    { 
        public static string Ellipsis(this string str, int maxLength) 
        {
            return str.Length > maxLength ? str[..(maxLength - 3)] + "..." : str;
        }
    }

    class NbuRate
    {
        public int r030 { get; set; }
        public String txt { get; set; }
        public double rate { get; set; }
        public String cc { get; set; }
        public String Exchangedate { get; set; }
    }
}
