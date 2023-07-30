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



    public Brush? AlertBrush { get; set; }

    private Point? dragStart = null;

    public HexSector()
    {
        InitializeComponent();

        Loaded += OnLoaded;
    }

    private void OnLoaded(object sender, RoutedEventArgs e)
    {
        DataContext = this;

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
        //self.
    }

    private void OnPreviewMouseMove(object sender, MouseEventArgs e)
    {
        if (dragStart == null ||
            e.LeftButton != MouseButtonState.Pressed) return;

        Canvas c = (Canvas)Parent;
        Point position = e.GetPosition(c);

        Canvas.SetLeft(this, position.X - dragStart.Value.X);
        Canvas.SetTop(this, position.Y - dragStart.Value.Y);

        //e.Handled = true;
    }


    private void OnMouseDown(object sender, MouseButtonEventArgs e)
    {
        var element = (UIElement)sender;
        dragStart = e.GetPosition(element); ;
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
