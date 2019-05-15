using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using CefSharp.Wpf;
using Organic_Browser.Utils;
using Organic_Browser.Controls;
using System;
using System.IO;

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

            // Prevent the maximized window from hiding windows taskbar
            this.MaxHeight = SystemParameters.MaximizedPrimaryScreenHeight;

            BindWindowButtons();
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
            webBrowser.Address = GetInitialUrl();
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

        /// <summary>
        /// Returns the initial url to start from.
        /// The url may be a file opened with the browser or the home page
        /// </summary>
        /// <returns></returns>
        private static string GetInitialUrl()
        {
            string[] args = Environment.GetCommandLineArgs();

            if (args.Length == 2 && File.Exists(args[1]))
            {
                // In case a file was opened with the browser, show the file
                return OrganicUtility.GetUrlFromPath(args[1]);
            }
            else
                return UserSettings.Load().HomePage;
            
            
        }

        /// <summary>
        /// DragMove the window when the top WrapPanel is pressed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void WrapPanel_MouseDown(object sender, MouseButtonEventArgs e)
        {
            // Begin dragging the window
            if (this.WindowState == WindowState.Maximized)
            {
                this.WindowState = WindowState.Normal;
                this.Top = e.GetPosition(this).Y;
                this.Left = e.GetPosition(this).X/2;
            }
            this.DragMove();
        }

        /// <summary>
        /// Binds the Minimize, Close, MakeBigger functions 
        /// to the buttons in the top right corner.
        /// </summary>
        private void BindWindowButtons()
        {
            // Close
            this.windowManagementControl.closeImageWrapper.MouseLeftButtonDown += (object sender, MouseButtonEventArgs e) =>
            {
                this.Close();
            };

            // Make bigger/smaller
            this.windowManagementControl.makeBiggerImageWrapper.MouseLeftButtonDown += (object sender, MouseButtonEventArgs e) =>
            {
                if (this.WindowState == WindowState.Maximized)
                    this.WindowState = WindowState.Normal;
                else
                    this.WindowState = WindowState.Maximized;
            };

            // minimize
            this.windowManagementControl.minimizeImageWrapper.MouseLeftButtonDown += (object sender, MouseButtonEventArgs e) =>
            {
                this.WindowState = WindowState.Minimized;
            };
        }
    }
}
