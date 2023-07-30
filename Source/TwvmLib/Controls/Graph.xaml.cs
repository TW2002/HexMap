using System;
using System.Collections.Generic;
using System.IO;
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
using TradeWarsData;


namespace TwvmLib.Controls;

/// <summary>
/// Interaction logic for Graph.xaml
/// </summary>
public partial class Graph : UserControl
{
    public Graph()
    {
        InitializeComponent();

        Loaded += OnLoaded;
    }

    private void OnLoaded(object sender, RoutedEventArgs e)
    {
        //<Path Width="64" Height="64" Stretch="Uniform" Fill="Blue"
        //Data = "M8.660254,0 L17.320508,5 17.320508,15 8.660254,20 0,15 0,5 8.660254,0 Z" />

        //0   , 8.66     0  43.3
        //5   , 17.32    25  86.6
        //15  , 17.32    75  86.6
        //20  , 8.66     100 
        //15  , 0
        //Data="M 80,200 A 100,50 45 1 0 100,50"


        return;
        const int GRID_SIZE = 20;
        const int GRID_SPACING = 50;

        //System.Windows.Shapes.Path path = new();
        //Canvas.SetTop(line, 10);
        //Canvas.SetLeft(line, 10);

        for (int i = 0; i < GRID_SIZE; i++)
        {
            Line line = new();
            line.X1 = 0;
            line.X2 = GRID_SIZE * GRID_SPACING * 2;
            line.Y1 = i * GRID_SPACING;
            line.Y2 = line.Y1;
            line.StrokeThickness = 1;
            line.Stroke = System.Windows.Media.Brushes.Blue;

            line.SnapsToDevicePixels = true;
            line.SetValue(RenderOptions.EdgeModeProperty, EdgeMode.Aliased);
            canvas.Children.Add(line);

            Line line2 = new();
            line2.X1 = i * GRID_SPACING;
            line2.X2 = line2.X1 + GRID_SIZE * GRID_SPACING;
            line2.Y1 = 0;
            line2.Y2 = GRID_SIZE * GRID_SPACING;
            line2.StrokeThickness = 1;
            line2.Stroke = System.Windows.Media.Brushes.Blue;

            line2.SnapsToDevicePixels = true;
            line2.SetValue(RenderOptions.EdgeModeProperty, EdgeMode.Aliased);
            canvas.Children.Add(line2);

            Line line3 = new();
            line3.X1 = i * GRID_SPACING;
            line3.X2 = line2.X1 + GRID_SIZE * GRID_SPACING;
            line3.Y1 = GRID_SIZE * GRID_SPACING;
            line3.Y2 = 0;
            line3.StrokeThickness = 1;
            line3.Stroke = System.Windows.Media.Brushes.Blue;

            line3.SnapsToDevicePixels = true;
            line3.SetValue(RenderOptions.EdgeModeProperty, EdgeMode.Aliased);
            canvas.Children.Add(line3);


        }

    }

    public void MoveTo(int sector, Game? game)
    {
        if (game == null) return;
        Sector? cs = game.Sectors.FirstOrDefault(s => s.SectorId == sector);
        if (cs == null) return;


    }
}


