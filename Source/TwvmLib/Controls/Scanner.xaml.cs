using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using TradeWarsData;

namespace TwvmLib.Controls;

/// <summary>
/// Interaction logic for Scanner.xaml
/// </summary>
public partial class Scanner : UserControl
{

    Paragraph col1 = new();

    Paragraph col2 = new();

    public Scanner()
    {
        InitializeComponent();


        Loaded += OnLoaded;
        SizeChanged += OnSizeChanged;
    }

    private void OnLoaded(object sender, RoutedEventArgs e)
    {

        MainTextBox.FontSize = ActualHeight / 12;
        PortTextBox.FontSize = ActualHeight / 12;

        MainTextBox.FontFamily = new FontFamily("Comic Sans MS");
        PortTextBox.FontFamily = new FontFamily("Comic Sans MS");
        MainTextBox.Document.Blocks.Clear();
        PortTextBox.Document.Blocks.Clear();

        AppendText(col1, "Greetings programs...", Brushes.Magenta);
        AppendText(col1, "<Not Connected>", Brushes.Red);
       
    }

    public void MoveTo(int sector, Game? game)
    {
        if (game == null) return;
        Sector? cs = game.Sectors.FirstOrDefault(s => s.SectorId == sector);
        if (cs == null) return;

        string warps = "";
        foreach (int waro in cs.WarpsOut)
        {
            if (waro != cs.WarpsOut.Last())
                warps += $"{waro} - ";
            else
                warps += $"{waro}";
            // todo (Unexplored )
        }

        Dispatcher.BeginInvoke(DispatcherPriority.Normal, (Action)(() =>
        {
            MainTextBox.Document.Blocks.Clear();
            col1.Inlines.Clear();
            col2.Inlines.Clear();

            AppendText(col1, $"Sector  : {sector} in {cs.Nebula}.\n", Brushes.White);
            if (!string.IsNullOrEmpty(cs.Beacon))
                AppendText(col1, $"Beacon  : {cs.Beacon}\n", Brushes.White);
            AppendText(col1, $" Warps to Sector(s) :  {warps}\n", Brushes.White);

            AppendText(col2, $"Ports   : {cs.Port.Name}, Class {cs.Port.Class} ({cs.Port.Type})\n\n\n\n\n\n", Brushes.White);
            AppendText(col2, $"Debug   : Nav1:{game.Nav1} Nav2:{game.Nav2} \n", Brushes.White);
            AppendText(col2, $"Class Zero : T:1() S:{game.Stardock}() A:{game.Alpha}() R:{game.Rylos}()", Brushes.White);




            MainNav.Scan();
        }));
    }
    private void AppendText(Paragraph p, string text, Brush brush)
    {
        Run run = new(text);
        run.Foreground = brush;
        p.Inlines.Add(run);
        MainTextBox.Document.Blocks.Add(col1);
        PortTextBox.Document.Blocks.Add(col2);
    }

    private void OnSizeChanged(object sender, SizeChangedEventArgs e)
    {
        // Resize hex nav.
        MainNav.Width = ActualHeight;
        MainNav.Height = ActualHeight;

        // Reposition the main textbox when the control's height changes. 
        Thickness margin = MainTextBox.Margin;
        margin.Left = ActualHeight;
        MainTextBox.Margin = margin;
        MainTextBox.Width = (ActualWidth - ActualHeight) * .55;

        // Reposition the port textbox when the control's height changes. 
        margin = PortTextBox.Margin;
        margin.Left = ActualHeight + MainTextBox.Width;
        PortTextBox.Margin = margin;
        PortTextBox.Width = (ActualWidth - ActualHeight) * .45;

        // Adjust the text box font size.

        Double fontSize = ActualHeight/12;
        MainTextBox.Document.FontSize = fontSize;
        PortTextBox.Document.FontSize = fontSize;
        //MainTextBox.SelectAll();
        //TextRange tr = new(MainTextBox.Selection.Start,MainTextBox.Selection.End);
        //tr.ApplyPropertyValue(TextElement.FontSizeProperty, fontSize);
        //tr.Text = "";
    }
}