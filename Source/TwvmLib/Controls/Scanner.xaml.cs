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

namespace TwvmLib.Controls;

/// <summary>
/// Interaction logic for Scanner.xaml
/// </summary>
public partial class Scanner : UserControl
{
    public Scanner()
    {
        InitializeComponent();


        Loaded += OnLoaded;
        SizeChanged += OnSizeChanged;
    }

    private void OnLoaded(object sender, RoutedEventArgs e)
    {

        MainTextBox.FontSize = ActualHeight/8;;
        Paragraph paragraph = new Paragraph();
        Run run = new Run("Greetings programs...");
        run.Foreground = Brushes.Magenta;
        paragraph.Inlines.Add(run);
        Run run2 = new Run("\n<Not Connected>");
        run2.Foreground = Brushes.Red;
        paragraph.Inlines.Add(run2);
        MainTextBox.Document.Blocks.Clear();
        MainTextBox.Document.Blocks.Add(paragraph);

    }

    private void OnSizeChanged(object sender, SizeChangedEventArgs e)
    {
        // Resize hex nav.
        MainNav.Width = ActualHeight;
        MainNav.Height = ActualHeight;

        // Reposition the textbox when the control's height changes. 
        Thickness margin = MainTextBox.Margin;
        margin.Left = ActualHeight;
        MainTextBox.Margin = margin;

        // Adjust the text box font size.

        Double fontSize = ActualHeight/8;
        MainTextBox.Document.FontSize = fontSize;
        //MainTextBox.SelectAll();
        //TextRange tr = new(MainTextBox.Selection.Start,MainTextBox.Selection.End);
        //tr.ApplyPropertyValue(TextElement.FontSizeProperty, fontSize);
        //tr.Text = "";
    }
}