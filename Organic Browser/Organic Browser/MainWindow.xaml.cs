using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;
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
        }
    }
}
