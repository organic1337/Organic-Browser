using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using CefSharp.Wpf;
using Organic_Browser.Utils;
using Organic_Browser.Controls;
using System;

namespace Organic_Browser
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            // Initialize
            CefInitializer.Initialize();            
            InitializeComponent();

            ManageTabs();                                                           // Manage the browser tabs
            this.Closed += (object sender, EventArgs e) => Environment.Exit(0);     //  When the main window is closed, close the browser
        }

        /// <summary>
        /// Manages the browser tabs
        /// </summary>
        private void ManageTabs()
        {
            // Close the app in case all tabs are closed
            this.browserTabControl.TabClosed += (object sender, EventArgs e) =>
            {
                if (this.browserTabControl.TabCount == 0)
                {
                    Environment.Exit(0);
                }
            };

            // Start with one tab (Homepage)
            var webBrowser = new ChromiumWebBrowser();
            webBrowser.BrowserSettings.FileAccessFromFileUrls = CefSharp.CefState.Enabled;      // Enable loading local files through the browser
            var navigationBar = new NavigationBarControl();
            BrowserTab browserTab = new BrowserTab(navigationBar, webBrowser);
            webBrowser.Address = UserSettings.Load().HomePage;
            Grid tabContent = CreateGrid(navigationBar, webBrowser);
            this.browserTabControl.AddTab(webBrowser, "Home page", tabContent);          

            // Enable tab adding
            this.browserTabControl.NewTabButtonClick += (object sender, EventArgs e) =>
            {
                webBrowser = new ChromiumWebBrowser();
                webBrowser.BrowserSettings.FileAccessFromFileUrls = CefSharp.CefState.Enabled;  // Enable loading local files through the browser
                navigationBar = new NavigationBarControl();
                browserTab = new BrowserTab(navigationBar, webBrowser);
                webBrowser.Address = UserSettings.Load().NewTabPage;         
                tabContent = CreateGrid(navigationBar, webBrowser);
                this.browserTabControl.AddTab(webBrowser, "New Tab", tabContent);
            };
        }

        /// <summary>
        /// Creates a grid of the navigation bar combined 
        /// </summary>
        /// <param name="navigationBar"></param>
        /// <param name="webBrowser"></param>
        /// <returns></returns>
        public static Grid CreateGrid(NavigationBarControl navigationBar, ChromiumWebBrowser webBrowser)
        {
            Grid grid = new Grid();
            grid.RowDefinitions.Add(new RowDefinition() { Height = GridLength.Auto });
            grid.RowDefinitions.Add(new RowDefinition());
            webBrowser.SetValue(Grid.RowProperty, 1);
            navigationBar.Height = 57;
            navigationBar.VerticalAlignment = VerticalAlignment.Top;
            grid.Children.Add(webBrowser);
            grid.Children.Add(navigationBar);

            return grid;
        }
    }
}
