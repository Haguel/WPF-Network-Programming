using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using System.Windows;

namespace network_wpf
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private static String configFilename = "email-settings.json";
        private static JsonElement? settings = null;

        public static String GetConfiguration(String name)
        {
            if (settings == null)
            {
                if (!System.IO.File.Exists(configFilename))
                {
                    MessageBox.Show("Configuration file doesn't exist");

                    return null;
                }

                // hw 12.10
                try
                {
                    settings = JsonSerializer.Deserialize<dynamic>
                        (System.IO.File.ReadAllText(configFilename));
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Invalid json");
                }
            }

            JsonElement? jsonElement = settings;

            try
            {
                foreach (String key in name.Split(':'))
                {
                    jsonElement = jsonElement?.GetProperty(key);
                }
            }
            catch
            {
                return null;
            }

            //String? host = settings.GetProperty("smtp").GetProperty("host").GetString();

            return jsonElement?.GetString();
        }
    }
}
