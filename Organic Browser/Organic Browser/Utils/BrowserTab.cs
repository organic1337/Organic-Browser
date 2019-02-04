using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Input;
using CefSharp;
using CefSharp.Wpf;
using Organic_Browser.Controls;
using System.Windows.Data;
using System.Web;
using System.Windows;

namespace Organic_Browser.Utils
{
    /// <summary>
    /// Represents a browser tab
    /// </summary>
    class BrowserTab
    {
        // public properties 
        public NavigationBarControl NavigationBar { get; set; }     // Navigation bar control
        public ChromiumWebBrowser WebBrowser { get; set; }          // Web Browser control

        // Private -  Read only data
        private readonly Regex UrlRegex = new Regex(@"^(http:\/\/www\.|https:\/\/www\.|http:\/\/|https:\/\/)?[a-z0-9]+([\-\.]{1}[a-z0-9]+)*\.[a-z]{2,5}(:[0-9]{1,5})?(\/.*)?$", RegexOptions.Compiled | RegexOptions.Compiled);

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
            this.NavigationBar.urlTexBox.KeyUp += UrlTextBox_KeyUp;                         // Handle key pressed in the url textbox
            this.NavigationBar.backBtn.MouseLeftButtonUp += NavBar_BackBtnPress;            // Handle mouse left button up on back button
            this.NavigationBar.forwardBtn.MouseLeftButtonUp += NavBar_ForwardBtnPress;      // Handle mouse left button up on forward button
            this.NavigationBar.refreshBtn.MouseLeftButtonUp += NavBar_RefreshBtnPress;      // Handle mouse left button up on refresh button
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
                if (UrlRegex.IsMatch(this.NavigationBar.Url))
                    this.WebBrowser.Address = this.NavigationBar.Url;
                // In case the given url is NOT actually a url
                else
                    this.WebBrowser.Address = "https://www.google.com/search?q=" + HttpUtility.UrlEncode(this.NavigationBar.Url);
            }
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
        #endregion
    }
}
