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
    /// Interaction logic for SettingsDropdownControl.xaml
    /// </summary>
    public partial class SettingsDropdownControl : UserControl
    {
        public SettingsDropdownControl()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Executes when the preferences setting is clicked
        /// </summary>
        /// <param name="sender"> Event sender </param>
        /// <param name="e">Event args</param>
        private void Preferences_MouseDown(object sender, MouseButtonEventArgs e)
        {
            // In case preference window is not running
            if (!PreferencesWindow.IsRunning)
            {                
                var window = new PreferencesWindow();
                window.Show();
                System.Windows.Threading.Dispatcher.Run();
            }
            // In case preference window is already running
            else
            {
                PreferencesWindow.CurrentRunningWindow.Focus();   
            }
        }

        /// <summary>
        /// Executes when the exit setting is clicked. CLOSES THE APPLICATION!
        /// </summary>
        /// <param name="sender">Event sender</param>
        /// <param name="e">Event args</param>
        private void Exit_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Environment.Exit(0);    // Exit the application, close all open windows
        }

        /// <summary>
        /// Executes when the history settings is clicked. Opens a new tab with the history
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void History_MouseDown(object sender, MouseButtonEventArgs e)
        {
            var webBrowser = new CefSharp.Wpf.ChromiumWebBrowser();
            webBrowser.BrowserSettings.FileAccessFromFileUrls = CefSharp.CefState.Enabled;  // Enable loading local files through the browser
            var mainWindow = Application.Current.MainWindow as MainWindow;

            
            NavigationBarControl navigationBar = new NavigationBarControl();
            var browserTab = new Utils.BrowserTab(navigationBar, webBrowser);
            webBrowser.Address = Utils.OrganicUtility.GetLocalPageActualUrl("organic://history");
            mainWindow.browserTabControl.AddTab(webBrowser, "History", MainWindow.CreateGrid(navigationBar, webBrowser));
        }
    }
}
