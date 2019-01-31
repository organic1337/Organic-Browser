using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using CefSharp;
using CefSharp.Wpf;
using Organic_Browser.Controls;

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

            // Bind the Navigation bar and the web browser
            this.Bind();
        }

        /// <summary>
        /// Binds the browser to the navigation bar
        /// </summary>
        private void Bind()
        {          
            // Bind the buttons in the navigation bar to the commands of the webbrowser
            // Back
            this.NavigationBar.backBtn.MouseUp += (object obj, MouseButtonEventArgs e) => this.WebBrowser.BackCommand.Execute(null);
            // Forward
            this.NavigationBar.forwardBtn.MouseUp += (object obj, MouseButtonEventArgs e) => this.WebBrowser.ForwardCommand.Execute(null);
            // Refresh
            this.NavigationBar.refreshBtn.MouseUp += (object obj, MouseButtonEventArgs e) => this.WebBrowser.ReloadCommand.Execute(null);

            // Update url on navigation
            // TODO: FIX!! working super slow
            this.WebBrowser.FrameLoadEnd += (object obj, FrameLoadEndEventArgs e) => this.UpdateUrlTextBox();

            // Key up (Navigation bar textbox)
            this.NavigationBar.urlTexBox.KeyUp += UrlTextBox_OnKeyUp;            
            
            // TODO: Bind the home button, settings button and eventually download button
        }

        /// <summary>
        /// Executes when a key in the keyboard is up in the URL text box
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="eventArgs"></param>
        private void UrlTextBox_OnKeyUp(object sender, KeyEventArgs eventArgs)
        {
            if (eventArgs.Key == Key.Enter)
                this.WebBrowser.Address = this.NavigationBar.urlTexBox.Text;
        }

        /// <summary>
        /// Updates the url text box with the WebBrowser object url
        /// </summary>
        private void UpdateUrlTextBox()
        {
            // Update the url from the thread that created the navigation bar
            void Update()
            {
                this.NavigationBar.urlTexBox.Text = this.WebBrowser.Address;
            }
            this.NavigationBar.Dispatcher.BeginInvoke((Action) Update);      
        }
    }
}
