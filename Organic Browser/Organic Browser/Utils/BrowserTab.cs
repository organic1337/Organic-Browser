using System.Net;
using System.Web;
using CefSharp.Wpf;
using System.Windows.Data;
using System.Windows.Input;
using Organic_Browser.Controls;
using System.Text.RegularExpressions;

namespace Organic_Browser.Utils
{
    /// <summary>
    /// Represents a browser tab, Links the ChromiumWebBrowser object 
    /// with the NavigationBarControl object
    /// </summary>
    class BrowserTab
    {
        // public properties 
        public NavigationBarControl NavigationBar { get; set; }     // Navigation bar control
        public ChromiumWebBrowser WebBrowser { get; set; }          // Web Browser control

    
        /// <summary>
        /// Browser tab constructor
        /// </summary>
        /// <param name="navigationBar">Navigation bar control</param>
        /// <param name="webBrowser">Chromium web browser object</param>
        public BrowserTab(NavigationBarControl navigationBar, ChromiumWebBrowser webBrowser)
        {
            // Assign the properties
            this.NavigationBar = navigationBar;
            this.WebBrowser = webBrowser;


            this.Bind();            // Bind the Navigation bar and the web browser
            this.HandleEvents();    // Handle events of both NavigationBar and WebBrowser objects
        }

        /// <summary>
        /// Binds the browser to the navigation bar
        /// </summary>
        private void Bind()
        {
            // Bind the back button to the CanGoBack property
            var backBinding = new Binding("BackEnabled")
            {
                Source = this.NavigationBar,
                Mode = BindingMode.OneWayToSource
            };
            this.WebBrowser.SetBinding(ChromiumWebBrowser.CanGoBackProperty, backBinding);

            // Bind the forward bottun to the CanGoForward property
            var forwardBinding = new Binding("ForwardEnabled")
            {
                Source = this.NavigationBar,
                Mode = BindingMode.OneWayToSource
            };
            this.WebBrowser.SetBinding(ChromiumWebBrowser.CanGoForwardProperty, forwardBinding);

            // Bind the url data to the Address property
            var urlBinding = new Binding("Url")
            {
                Source = this.NavigationBar,
                Mode = BindingMode.OneWayToSource
            };
            this.WebBrowser.SetBinding(ChromiumWebBrowser.AddressProperty, urlBinding);
            // TODO: Bind the home button, settings button and eventually download button
        }

        /// <summary>
        /// Assigns the event needed for the tab to work properly
        /// </summary>
        private void HandleEvents()
        {
            // Navigation bar events
            this.NavigationBar.urlTextBox.KeyUp += UrlTextBox_KeyUp;                                 // Handle key pressed in the url textbox
            this.NavigationBar.backBtn.MouseLeftButtonUp += NavBar_BackBtnPress;                    // Handle mouse click on Back button
            this.NavigationBar.forwardBtn.MouseLeftButtonUp += NavBar_ForwardBtnPress;              // Handle mouse click on Forward button
            this.NavigationBar.refreshBtn.MouseLeftButtonUp += NavBar_RefreshBtnPress;              // Handle mouse click on Refresh button
            this.NavigationBar.homeBtn.MouseLeftButtonUp += NavBar_HomeBtnPress;                    // Handle mouse click on Home button
            this.NavigationBar.settingsMenu.setHomePageLabel.MouseDown += NavBar_SetAsHomePress;    // Handle mouse click on Set as home setting

            // Web browser events
            this.WebBrowser.PreviewMouseLeftButtonUp += WebBrowser_MouseLeftButtonUp;               // Handle mouse click on the web browser
            this.WebBrowser.LoadError += WebBrowser_LoadError;
            this.WebBrowser.FrameLoadEnd += WebBrowser_FrameLoadEnd;
        }

        #region Event handlers
        /// <summary>
        /// Executes when a key in the keyboard is up in the URL text box
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="eventArgs"></param>
        private void UrlTextBox_KeyUp(object sender, KeyEventArgs eventArgs)
        {
            if (eventArgs.Key == Key.Enter)
            {
                // In case the given url is a valid address
                if (OrganicWebUtility.IsValidUrl(this.NavigationBar.Url))
                    this.WebBrowser.Address = this.NavigationBar.Url;
                // In case the given url is an organic url (such as organic://history)
                else if (OrganicWebUtility.IsOrganicUrl(this.NavigationBar.Url))
                    this.WebBrowser.Address = OrganicWebUtility.GetLocalPageActualUrl(this.NavigationBar.Url);
                // In case the given url is NOT actually a url
                else
                    this.WebBrowser.Address = "https://www.google.com/search?q=" + HttpUtility.UrlEncode(this.NavigationBar.Url);
            }
        }

        /// <summary>
        /// Executes when the mouse left button is up on the web browser
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="e"></param>
        private void WebBrowser_MouseLeftButtonUp(object obj, MouseButtonEventArgs e)
        {
            this.NavigationBar.MakeSettingsMenuInvisible();
        }

        /// <summary>
        /// Executes when the home button is pressed
        /// </summary>
        /// <param name="obj">sender</param>
        /// <param name="e">event args</param>
        private void NavBar_HomeBtnPress(object obj, MouseButtonEventArgs e)
        {
            UserSettings settings = UserSettings.Load();
            this.WebBrowser.Address = settings.HomePage;    // Navigate to home page
        }

        /// <summary>
        /// Execute
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="e"></param>
        private void NavBar_SetAsHomePress(object obj, MouseButtonEventArgs e)
        {
            UserSettings settings = UserSettings.Load();
            settings.HomePage = this.WebBrowser.Address;
            settings.Save();
            System.Console.WriteLine("DEBUG: {0} Is set as home page", settings.HomePage);
        }

        /// <summary>
        /// Executes when forward button is pressed
        /// </summary>
        /// <param name="obj">sender</param>
        /// <param name="e">event args</param>
        private void NavBar_ForwardBtnPress(object obj, MouseButtonEventArgs e)
        {
            this.WebBrowser.ForwardCommand.Execute(null);
        }

        /// <summary>
        /// Executes when forward button is pressed
        /// </summary>
        /// <param name="obj">sender</param>
        /// <param name="e">event args</param>
        private void NavBar_BackBtnPress(object obj, MouseButtonEventArgs e)
        {
            this.WebBrowser.BackCommand.Execute(null);
        }

        /// <summary>
        /// Executes when the refresh button is pressed
        /// </summary>
        /// <param name="obj">sender object</param>
        /// <param name="e">event args</param>
        private void NavBar_RefreshBtnPress(object obj, MouseButtonEventArgs e)
        {
            this.WebBrowser.ReloadCommand.Execute(null);
        }

        /// <summary>
        /// Executes when loading error is occured.
        /// 
        /// Basically, show an html page for each error.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void WebBrowser_LoadError(object sender, CefSharp.LoadErrorEventArgs e)
        {
            // Save the original url
            string originalUrl = null;
            this.NavigationBar.Dispatcher.Invoke(() => originalUrl = this.NavigationBar.urlTextBox.Text);

            // In case error aborted by CEF, do nothing
            if (e.ErrorCode == CefSharp.CefErrorCode.Aborted)
                return;

            // Handle loading errors
            string errorPagePath = string.Empty;
            if (OrganicWebUtility.IsConnected() == false)
            {
                // In case of no internet
                errorPagePath = "organic://error/no_internet";
            }
            else if (e.ErrorCode == CefSharp.CefErrorCode.NameNotResolved)
            {
                // In case of domain could not be resolved
                errorPagePath = "organic://error/could_not_resolve";
            }
            else
            {
                // Default loading error page
                System.Console.WriteLine(e.ErrorText);
                errorPagePath = "organic://error/general";
            }

            // Handle more loading errors here

            // Load the error page
            this.WebBrowser.Dispatcher.Invoke(() => this.WebBrowser.Address = OrganicWebUtility.GetLocalPageActualUrl(errorPagePath));

            // Restore the url
            this.NavigationBar.Dispatcher.Invoke(() => this.NavigationBar.urlTextBox.Text = originalUrl);
        }

        /// <summary>
        /// Executes when the Embedded chromium web browser finished to load a frame.
        /// </summary>
        /// <param name="sender">event sender</param>
        /// <param name="e">event args</param>
        private void WebBrowser_FrameLoadEnd(object sender, CefSharp.FrameLoadEndEventArgs e)
        {
            if (e.Frame.IsMain && !e.Url.StartsWith("file"))
            {
                History history = History.Load();
                history.AddUrlVisit(e.Url);
            }
        }
        #endregion

        #region Private functions

        #endregion
    }
}
