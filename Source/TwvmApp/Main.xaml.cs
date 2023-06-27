using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
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
using SpeechLib;
namespace TwvmApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class Main : Window
    {
        private Listener listener;
        private Reader reader;

        public Main()
        {
            InitializeComponent();

            Loaded += OnLoaded;
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            // Load speech recognition
            listener  = new();
            reader = new();

            Database.Items.Add("Localhost:2300");
            Database.Items.Add("MBN Game Z");
            Database.SelectedIndex = 0;


            MapMode.Items.Add("Current sector");
            MapMode.Items.Add("Selected sector");
            MapMode.Items.Add("Class zero port");
            MapMode.Items.Add("Navigation point");
            MapMode.Items.Add("Follow trader");
            MapMode.SelectedIndex = 0;

            ClassZero.Items.Add("Terra Sol - 1");
            ClassZero.Items.Add("Stardock");
            ClassZero.Items.Add("Alpha Centauri");
            ClassZero.Items.Add("Rylos");
            ClassZero.SelectedIndex = 0;
        }

        private void OnConnectClick(object sender, RoutedEventArgs e)
        {
            if (Connect.Content as String == "Connect")
            {
                Connect.Content = "Disconnect";
                reader.Read("Connecting to" + Database.SelectedItem, "Qubot");

            }
            else
            {
                Connect.Content = "Connect";
                reader.Read("Disconnected from server", "Qubot");
            }





        }

        private void MapMode_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void MapModeChanged(object sender, SelectionChangedEventArgs e)
        {
            TargetSector.Visibility = Visibility.Collapsed;
            ClassZero.Visibility = Visibility.Collapsed;



            switch (MapMode.SelectedIndex)
            {
                case 0:
                case 1:
                    TargetSector.Visibility = Visibility.Visible;
                    break;
                case 2:
                    ClassZero.Visibility = Visibility.Visible;
                    break;
            }

        }
    }
}
