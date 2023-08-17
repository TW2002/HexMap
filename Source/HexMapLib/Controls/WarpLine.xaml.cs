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
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using TradeWarsData;

namespace TwvmLib.Controls;

/// <summary>
/// Interaction logic for WarpLine.xaml
/// </summary>
public partial class WarpLine : UserControl
{
    public HexSector SN { get; set; }
    public HexSector TN { get; set; }
    public WarpLine(HexSector sn, HexSector tn)
    {
        InitializeComponent();

        DataContext = this;
        SN = sn;
        TN = tn;


        Loaded += OnLoaded;

    }

    private void OnLoaded(object sender, RoutedEventArgs e)
    {

    }
}
