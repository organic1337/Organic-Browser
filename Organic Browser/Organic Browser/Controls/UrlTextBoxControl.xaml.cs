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

namespace Organic_Browser.Controls
{
    /// <summary>
    /// Interaction logic for UrlTextBoxControl.xaml
    /// </summary>
    public partial class UrlTextBoxControl : UserControl
    {
        // Text property
        public string Text {    
            get { return this.UrlTB.Text; }
            set { Console.WriteLine("UrlChanges: {0}", value); this.UrlTB.Text = value;}
        }

        public UrlTextBoxControl()
        {
            InitializeComponent();

            // Select all on double click
            this.UrlTB.MouseDoubleClick += (object obj, MouseButtonEventArgs e) => UrlTB.SelectAll();
        }
    }
}
