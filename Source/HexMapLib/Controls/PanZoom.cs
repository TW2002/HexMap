using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;

namespace TwvmLib.Controls;

public class PanZoom : ContentControl
{
    public double Scale { get; private set; }

    private ScrollViewer? sve;
    private ContentPresenter? cpe;

    private Point? dragStart = null;
    private Point? offset = null;

    public PanZoom()
    {
        this.DefaultStyleKey = typeof(PanZoom);

        Scale = 1.0;

        LostFocus += OnLostFocus;
    }

    private void OnLostFocus(object sender, RoutedEventArgs e)
    {

    }
    public override void OnApplyTemplate()
    {
        sve = GetTemplateChild("sv") as ScrollViewer;
        cpe = GetTemplateChild("cp") as ContentPresenter;

        if (sve == null || cpe == null ) return;

        cpe.PreviewMouseWheel += OnMouseWheel;

        sve.PreviewMouseDown += OnMouseDown;
        sve.PreviewMouseUp += OnMouseUp;
        sve.PreviewMouseMove += OnMouseMove;

    }


    private void OnMouseWheel(object sender, MouseWheelEventArgs e)
    {
        var cp = sender as ContentPresenter;
        if (cp == null || sve == null) return;

        ////var transform = element.RenderTransform as MatrixTransform;
        //var transform = element.LayoutTransform as MatrixTransform;

        //var matrix = transform.Matrix;
        //var scale = e.Delta >= 0 ? 1.1 : (1.0 / 1.1); // choose appropriate scaling factor

        var cpPos = e.GetPosition(cp);
        var svPos = e.GetPosition(sve);

        Scale += e.Delta >= 0 ? 0.1 : -0.1;
        if (Scale < 0.5) Scale = 0.5;
        if (Scale > 2.0) Scale = 2.0;

        cp.LayoutTransform = new ScaleTransform(Scale, Scale);

        //matrix.ScaleAtPrepend(scale, scale, position.X, position.Y);
        ////element.RenderTransform = new MatrixTransform(matrix);
        //element.LayoutTransform  = new MatrixTransform(matrix);

        //sve.ScrollToHorizontalOffset(sve.HorizontalOffset * scale);
        // [this step is critical for offset]
        sve.ScrollToHorizontalOffset(0);
        sve.ScrollToVerticalOffset(0);
        UpdateLayout();


        Vector offset = cp.TranslatePoint(cpPos, sve) - svPos;
        sve.ScrollToHorizontalOffset(offset.X);
        sve.ScrollToVerticalOffset(offset.Y);
        UpdateLayout();

        e.Handled = true;
    }

    private void OnMouseDown(object sender, MouseButtonEventArgs e)
    {
        FrameworkElement? source = e.OriginalSource as FrameworkElement;
        if (source == null || source.Name != "canvas") return;

        var element = (UIElement)sender;
        dragStart = e.GetPosition(element);
        if (sve == null) return;
        offset = new(sve.HorizontalOffset, sve.VerticalOffset);

        //element.CaptureMouse();

        Mouse.OverrideCursor = Cursors.ScrollAll;
        //e.Handled = true;
    }

    private void OnMouseUp(object sender, MouseButtonEventArgs e)
    {
        var element = (UIElement)sender;
        dragStart = null;
        offset = null;
        element.ReleaseMouseCapture();

        // Restore the cursor to an arrow.
        Mouse.OverrideCursor = null;

        //e.Handled = true;
    }

    private void OnMouseMove(object sender, MouseEventArgs e)
    {
        if (dragStart == null || offset == null || sve == null ||
            e.LeftButton != MouseButtonState.Pressed) return;

        Point position = e.GetPosition((UIElement)sender);

        sve.ScrollToHorizontalOffset(offset.Value.X + (dragStart.Value.X - position.X));
        sve.ScrollToVerticalOffset(offset.Value.Y + (dragStart.Value.Y - position.Y));

    }
}
