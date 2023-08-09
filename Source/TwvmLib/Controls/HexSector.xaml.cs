using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using TradeWarsData;

namespace TwvmLib.Controls;

/// <summary>
/// Interaction logic for HexSector.xaml
/// </summary>
public partial class HexSector : Button
{
    public int Sector
    {
        get { return (int)GetValue(SectorProperty); }
        set { SetValue(SectorProperty, value); }
    }

    public static readonly DependencyProperty SectorProperty
        = DependencyProperty.Register("Sector",
            typeof(int), typeof(HexSector),
            new FrameworkPropertyMetadata(new PropertyChangedCallback(SectorChanged)));

    public double X 
    {
        get { return (double)GetValue(XProperty); }
        set { SetValue(XProperty, value); }
    }

    public static readonly DependencyProperty XProperty
        = DependencyProperty.Register("X",
            typeof(double), typeof(HexSector),
            new FrameworkPropertyMetadata(new PropertyChangedCallback(PositionChanged)));

    public double Y
    {
        get { return (double)GetValue(YProperty); }
        set { SetValue(YProperty, value); }
    }

    public static readonly DependencyProperty YProperty
        = DependencyProperty.Register("Y",
            typeof(double), typeof(HexSector),
            new FrameworkPropertyMetadata(new PropertyChangedCallback(PositionChanged)));


    public int SectorFontSize { get; set; }
    public Brush? AlertBrush { get; set; }

    private Point? dragStart = null;

    public HexSector(int sector)
    {
        DataContext = this;
        InitializeComponent();

        Loaded += OnLoaded;
        if (sector > 0) Sector = sector;
    }

    private void OnLoaded(object sender, RoutedEventArgs e)
    {
        PreviewMouseMove += OnPreviewMouseMove;
        PreviewMouseDown += OnMouseDown;
        PreviewMouseUp += OnMouseUp;

        //Path path = new Path();
        //path.Data = Geometry.Parse("M 0 45 L 25 0 L 75, 0 L 100 45 L 75 90 L 25 90 Z") ;
        //path.Stroke = this.Foreground;
        //path.StrokeThickness = 1;
        //this.Content = path;
    }


    private static void SectorChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        HexSector self = (HexSector)d;
        self.SectorFontSize = 36;
        if (self.Sector > 999) self.SectorFontSize = 26;
        if (self.Sector > 9999) self.SectorFontSize = 20;
    }
    private static void PositionChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        HexSector self = (HexSector)d;

        Canvas.SetLeft(self, self.X - (self.ActualWidth / 2));
        Canvas.SetTop(self, self.Y - (self.ActualHeight / 2));


    }

    private void OnPreviewMouseMove(object sender, MouseEventArgs e)
    {
        if (dragStart == null ||
            e.LeftButton != MouseButtonState.Pressed) return;

        Canvas c = (Canvas)Parent;
        Point position = e.GetPosition(c);
        
        //Canvas.SetLeft(this, position.X - dragStart.Value.X);
        //Canvas.SetTop(this, position.Y - dragStart.Value.Y);
        X = position.X - dragStart.Value.X;
        Y = position.Y - dragStart.Value.Y;


        //e.Handled = true;
    }


    private void OnMouseDown(object sender, MouseButtonEventArgs e)
    {
        var element = (UIElement)sender;
        //dragStart = e.GetPosition(element);
        dragStart = new (e.GetPosition(element).X - (ActualWidth / 2),
                         e.GetPosition(element).Y - (ActualHeight / 2));

        element.CaptureMouse();

        Mouse.OverrideCursor = Cursors.Hand;
        //e.Handled = true;
    }
    private void OnMouseUp(object sender, MouseButtonEventArgs e)
    {
        var element = (UIElement)sender;
        dragStart = null;
        element.ReleaseMouseCapture();

        Mouse.OverrideCursor = null;
        //e.Handled = true;
    }





}
