using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using WooScripter.Objects.WooScript;
using System.Windows.Navigation;
using System.Diagnostics;
using WooScripter.Objects;

namespace WooScripter
{
    /// <summary>
    /// Interaction logic for HelpWindow.xaml
    /// </summary>
    public partial class HelpWindow : Window
    {
        public HelpWindow()
        {
            InitializeComponent();

            string helpText;

            WooScript helpScript = new WooScript();
            helpText = helpScript.GetHelpText();

            string distanceHelpText;

            distanceHelpText = Distance.GetHelpText();
            helpText += System.Environment.NewLine + "Distance Estimation primitives : " + System.Environment.NewLine + distanceHelpText;

            textBox1.Text = helpText;
        }
        
        private void Hyperlink_RequestNavigate(object sender, RequestNavigateEventArgs e)
        {
            Process.Start(new ProcessStartInfo(e.Uri.AbsoluteUri));
            e.Handled = true;
        }

        private void button1_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
