using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
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
    /// <summary>
    /// Логика взаимодействия для CryptoWindow.xaml
    /// </summary>

    public class HistoryResponse
    {
        public List<HistoryItem> data { get; set; }
        public long timestamp { get; set; }
    }

    public class HistoryItem
    {
        public String priceUsd { get; set; }
        public long time { get; set; }
        public double price => Double.Parse(priceUsd, CultureInfo.InvariantCulture);    
    }

    public class CoincapResponse
    {
        public List<CoinData> data { get; set; }
        public long timestamp { get; set; }
    }
    public class CoinData
    {
        public string id { get; set; }
        public string rank { get; set; }
        public string symbol { get; set; }
        public string name { get; set; }
        public string supply { get; set; }
        public string maxSupply { get; set; }
        public string marketCapUsd { get; set; }
        public string volumeUsd24Hr { get; set; }
        public string priceUsd { get; set; }
        public string changePercent24Hr { get; set; }
        public string vwap24Hr { get; set; }
        public string explorer { get; set; }
    }

    public partial class CryptoWindow : Window
    {

        private readonly HttpClient _httpClient;
        public ObservableCollection<CoinData> CoinsData { get; set; }

        public CryptoWindow()
        {
            InitializeComponent();
            CoinsData = new();
            this.DataContext = this;
            _httpClient = new() { BaseAddress = new Uri(@"https://api.coincap.io/") };
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            LoadAssetsAsync();
        }

        private async Task LoadAssetsAsync()
        {
            var response = JsonSerializer.Deserialize<CoincapResponse>(
                await _httpClient.GetStringAsync(@"/v2/assets?limit=10")
            );

            if (response == null)
            {
                MessageBox.Show("Deserialization error");

                return;
            }

            CoinsData.Clear();
            foreach (var coinData in response.data)
            {
                CoinsData.Add(coinData);
            }
        }

        private void ListViewItem_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            // hw 20.10
            foreach (CoinData coinData in CoinsData)
            {
                ListViewItem listViewItem = itemsListView.ItemContainerGenerator.ContainerFromItem(coinData) as ListViewItem;

                listViewItem.Background = null;
            }

            if (sender is ListViewItem item)
            {
                ListViewItem currentItem = sender as ListViewItem;
                currentItem.Background = Brushes.Aqua;

                if (item.Content is CoinData coinData)
                {
                    ShowHistory(coinData);
                    MessageBox.Show(coinData.id);
                }
            }    
        }

        private async Task ShowHistory(CoinData coinData)
        {
            String body = await _httpClient.GetStringAsync($"/v2/assets/{coinData.id}/history?interval=d1");
            var response = JsonSerializer.Deserialize<HistoryResponse>(body);

            if (response == null || response.data == null)
            {
                MessageBox.Show("Invalid data loaded");
                return;
            }

            long minTime, maxTime;
            double minPrice, maxPrice;
            minTime = maxTime = response.data[0].time;
            minPrice = maxPrice = response.data[0].price;

            foreach (HistoryItem item in response.data)
            {
                if (item.time < minTime) minTime = item.time;
                if (item.time > maxTime) maxTime = item.time;
                if (item.price < minPrice) minPrice = item.price;
                if (item.price > maxPrice) maxPrice = item.price;
            }

            double yOffset = 10;
            double graphH = Graph.ActualHeight - yOffset;

            double x0 = (response.data[0].time - minTime) * Graph.ActualWidth / (maxTime - minTime);
            double y0 = graphH - (response.data[0].price - minPrice) * graphH / (maxPrice - minPrice);

            Graph.Children.Clear();
            foreach (HistoryItem item in response.data)
            {
                double x = (item.time - minTime) * Graph.ActualWidth / (maxTime - minTime);
                double y = graphH - (item.price - minPrice) * graphH / (maxPrice - minPrice);

                DrawLine(x0, y0, x, y);

                x0 = x;
                y0 = y;
            }

            // hw 24.10
            DrawPriceLabel(0, Graph.ActualHeight - 10, "Min price " + minPrice.ToString());
            DrawPriceLabel(0, Graph.ActualHeight, "Min time " + minTime.ToString());
            DrawPriceLabel(0, 10, "Max price " + maxPrice.ToString());
            DrawPriceLabel(0, 0, "Max time " + maxTime.ToString());
            DrawLine(0, graphH, Graph.ActualWidth, graphH, Brushes.BlueViolet);
        }

        private void DrawLine(double x1, double y1, double x2, double y2, Brush brush = null!)
        {
            brush ??= new SolidColorBrush(Colors.Black);

            Graph.Children.Add(new Line
            {
                X1 = x1,
                Y1 = y1,
                X2 = x2,
                Y2 = y2,
                Stroke = new SolidColorBrush(Colors.Black),
                StrokeThickness = 2
            });
        }

        private void DrawPriceLabel(double x, double y, string text)
        {
            var textBlock = new TextBlock
            {
                Text = text,
                Foreground = Brushes.Black,
                FontSize = 12
            };


            Canvas.SetLeft(textBlock, x);
            Canvas.SetTop(textBlock, y - 20);

            Graph.Children.Add(textBlock);
        }
    }
}
