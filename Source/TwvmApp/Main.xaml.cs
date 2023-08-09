using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Net.Sockets;
using System.Reflection.PortableExecutable;
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
using TerminalLib;
using TradeWarsData;

namespace TwvmApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class Main : Window
    {
        private Listener listener;
        private Reader reader;

        private Games games = new();
        private List<Session> sessions = new(); 


        public Main()
        {
            InitializeComponent();

            Loaded += OnLoaded;
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Maximized;

            if (false)
            {
                games.AddLast(new Game("Local 2300", "localhost", 2300));
                games.AddLast(new Game("Local 2301", "localhost", 2301));
                games.AddLast(new Game("MBN Game Z", "MicroBlaster.Net")
                {
                    Login = "Micro",
                    Password = "Meow1234",
                    Letter = "a"
                });

                games.Save();
            }
            else
            {
                games.Load();
            }

            foreach (Game game in games)
            {
                Database.Items.Add(game.Name);
                sessions.Add(new(game));
                game.Moved += OnMove;
                game.Updated += OnUpdate;

            }
            Database.SelectedIndex = 0;

            Scanner.MoveTo(1, (Game?)games.First());
            SectorGraph.MoveTo(1, (Game?)games.First());

            reader = new();
            // Load speech recognition
            listener = new(reader);
            listener.EventRecognized += OnEvent;
            reader.Read("Greetings program");

       


     ;

//            Database.Items.Add("Localhost:2300");
  //          Database.Items.Add("MBN Game Z");
  //          Database.SelectedIndex = 0;


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

        private void OnUpdate(object sender, StatusEventArgs e)
        {
            Game game = (Game)sender;
            Session session = sessions.Where(s => s.Name == game.Name).Single();
            //if (!session.GameServer.Connected)


            if (e.Sector == 0)
            {
                if ((! string.IsNullOrEmpty(game.TwxVersion)) && game.Stardock == 0) 
                {
                    //session.GameServer.Send("$ssGetMap.ts\r");
                }

            }
        }

        private void OnMove(object sender, StatusEventArgs e)
        {
            Scanner.MoveTo(e.Sector, (Game)sender);
        }

        private void OnEvent(object sender, RecognizedArgs e)
        {
            switch (e.Name)
            {
                case "Connect":
                    ConnectTo(Database.SelectedItem as string);
                    break;
                default:
                    //reader.Read(e.Name + "dot" + e.Text); 
                    break;    
            }
        }

        private void OnConnectClick(object sender, RoutedEventArgs e)
        {
            if (Connect.Content as String == "Connect")
            {
                ConnectTo(Database.SelectedItem as string);
            }
            else
            {
                Connect.Content = "Connect";
                reader.Read("Disconnected from server", "Qubot");
            }
        }

        private void ConnectTo(string db)
        {
            Session session = sessions.Where(s => s.Name == db).Single();
            //if (!session.GameServer.Connected)
            //{
                Connect.Content = "Disconnect";
                session.Connect();
                reader.Read("Connecting to " + db, "Qubot");
            //}
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
