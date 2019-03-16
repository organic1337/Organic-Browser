using Organic_Browser.Utils;
using System;
using System.Collections.Generic;
using System.IO;
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
using System.Windows.Shapes;

namespace Organic_Browser
{
    /// <summary>
    /// Interaction logic for PreferencesWindow.xaml
    /// </summary>
    public partial class PreferencesWindow : Window
    {
        public static bool IsRunning = false;

        public PreferencesWindow()
        {
            InitializeComponent();

            IsRunning = true; // Mention that the window is running

            // Fill the preferences from the usersettings file
            UserSettings settings = UserSettings.Load();
            this.pagesDownloadLocation.Text = settings.DownloadWebpagesLocation;
        }

        // Read only data
        private static readonly Color RedColor = Color.FromRgb(0xa8, 0x03, 0x13);
        private static readonly Color GrayColor = Color.FromRgb(0x56, 0x56, 0x56);

        #region Event Handlers
        /// <summary>
        /// Executes when the preferences X button is clicked 
        /// </summary>
        /// <param name="sender"> Event sender </param>
        /// <param name="e"> event args </param>
        private void XButton_MouseDown(object sender, MouseButtonEventArgs e)
        {
            IsRunning = false; // Mention that the window had stopped running
            this.Close();
        }

        /// <summary>
        /// Executes when the preferences save button is clicked 
        /// </summary>
        /// <param name="sender"> Event sender </param>
        /// <param name="e"> event args </param>
        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            // Load the user settings
            UserSettings settings = UserSettings.Load();

            // Validate fields, if not all valid, return
            bool isValid = this.ValidateFields();
            if (!isValid) return;

            // Save each field in the user settings 
            settings.DownloadWebpagesLocation = this.pagesDownloadLocation.Text;
            settings.Save();

            // Prompt success 
            this.Prompt("Saved Successfully", GrayColor);
        }

        /// <summary>
        /// Executes when the preferences Reset button is clicked
        /// </summary>
        /// <param name="sender"> Event sender</param>
        /// <param name="e">Event args</param>
        private void ResetButton_Click(object sender, RoutedEventArgs e)
        {
            UserSettings settings = UserSettings.Load();    // Load the user settings

            // Reset each field to the original one from the user settings
            this.pagesDownloadLocation.Text = settings.DownloadWebpagesLocation;
        }

        #endregion

        #region Private functions
        /// <summary>
        /// Validates the fields, if not valid prompt an error message
        /// </summary>
        /// <returns> Whether all fields are valid or not </returns>
        private bool ValidateFields()
        {
            string errorMessage = string.Empty;

            // Validate the page download location
            if (!Directory.Exists(this.pagesDownloadLocation.Text))
            {
                errorMessage = "The given path does not exist";
            }

            // Prompt the error message and return whether valid or not
            if (errorMessage != string.Empty)
                this.Prompt(errorMessage, RedColor);
            return string.IsNullOrEmpty(errorMessage);

        }

        /// <summary>
        /// Prompts a message to the message label
        /// </summary>
        /// <param name="message">Message to prompt</param>
        /// <param name="color">Message color</param>
        private void Prompt(string message, Color color)
        {
            this.promptLabel.Foreground = new SolidColorBrush(color);
            this.promptLabel.Text = message;
        }
        #endregion
    }
}
