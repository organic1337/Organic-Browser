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
using Organic_Browser.Utils;

namespace Organic_Browser
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private List<BrowserTab> tabs = new List<BrowserTab>();

        public MainWindow()
        {
            CefInitializer.Initialize();            
            InitializeComponent();

            BrowserTab tab = new BrowserTab(this.navBar, this.webBrowser);
            webBrowser.Address = UserSettings.Load().HomePage;
            tabs.Add(tab);
        }

        /// <summary>
        /// Executes when the mouse's left button is down
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Grid_LeftButtonDown(object sender, MouseButtonEventArgs e)
        {
        }
    }
}
