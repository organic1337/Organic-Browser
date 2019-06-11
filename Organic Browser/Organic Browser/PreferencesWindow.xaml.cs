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
    /// 
    /// </summary>
    public partial class PreferencesWindow : Window
    {
        public static bool IsRunning = false;                               // Whether the window is running or not
        public static PreferencesWindow CurrentRunningWindow { get; private set; }  // The current running window

        // Read only data
        private static readonly Color RedColor = Color.FromRgb(0xa8, 0x03, 0x13);
        private static readonly Color GrayColor = Color.FromRgb(0x56, 0x56, 0x56);

        public PreferencesWindow()
        {
            InitializeComponent();

            // Mention that the window is running
            IsRunning = true;
            PreferencesWindow.CurrentRunningWindow = this;

            this.WindowStartupLocation = WindowStartupLocation.CenterScreen;

            // Handle theme changing
            (Application.Current as App).ThemeChanged += (object obj, ThemeChangedEventArgs e) =>
            {
                OrganicUtility.UpdateImages(this.mainLayout);
            };
            Application.Current.Activated += (object obj, EventArgs e) => OrganicUtility.UpdateImages(this.mainLayout);

            // Fill the preferences from the usersettings file
            UserSettings settings = UserSettings.Load();
            this.homePage.Value = settings.HomePage;
            this.newTabPage.Value = settings.NewTabPage;
            this.darkRadioButton.IsChecked = settings.Theme == Theme.Dark;
            this.lightRadioButton.IsChecked = settings.Theme == Theme.Light;
            this.autoRadioButton.IsChecked = settings.Theme == Theme.Auto;
        }

        #region Event Handlers
        /// <summary>
        /// Executes when the window is closed
        /// </summary>
        /// <param name="e">Event args</param>
        protected override void OnClosed(EventArgs e)
        {
            this.ApplySavedTheme();
            base.OnClosed(e);
        }

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
            settings.HomePage = this.homePage.Value;
            settings.NewTabPage = this.newTabPage.Value;
            if ((bool)this.darkRadioButton.IsChecked) settings.Theme = Theme.Dark;
            if ((bool)this.lightRadioButton.IsChecked) settings.Theme = Theme.Light;
            if ((bool)this.autoRadioButton.IsChecked) settings.Theme = Theme.Auto;
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
            this.ApplySavedTheme();                         // Apply the saved theme
            UserSettings settings = UserSettings.Load();    // Load the user settings

            // Reset each field to the original one from the user settings
            this.homePage.Value = settings.HomePage;
            this.newTabPage.Value = settings.NewTabPage;
            this.darkRadioButton.IsChecked = settings.Theme == Theme.Dark;
            this.lightRadioButton.IsChecked = settings.Theme == Theme.Light;
            this.autoRadioButton.IsChecked = settings.Theme == Theme.Auto;
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

            // Validate the home page
            if (!OrganicUtility.IsValidUrl(this.homePage.Value))
            {
                errorMessage = "The given URL for the home page is invalid";
            }
            // Validate the new tab page
            else if (!OrganicUtility.IsValidUrl(this.newTabPage.Value))
            {
                errorMessage = "The given URL for the new tab page is invalid";
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

        /// <summary>
        /// Applies the theme saved in the user settings file
        /// </summary>
        private void ApplySavedTheme()
        {
            var settings = UserSettings.Load();
            Theme theme = settings.Theme;
            if (theme == Theme.Auto)
                theme = OrganicUtility.GetWindowsTheme();
            ((App)Application.Current).ApplyTheme(theme);
        }
        #endregion

        /// <summary>
        /// Executes when the light theme radio button is checked
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void LightRadioButton_Checked(object sender, RoutedEventArgs e)
        {
            ((App)Application.Current).ApplyTheme(Theme.Light);
        }


        /// <summary>
        /// Executes when the Dark theme radio button is checked
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DarkRadioButton_Checked(object sender, RoutedEventArgs e)
        {
            ((App)Application.Current).ApplyTheme(Theme.Dark);
        }

        /// <summary>
        /// Executes when the Auto theme radio button is checked
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AutoRadioButton_Checked(object sender, RoutedEventArgs e)
        {
            ((App)Application.Current).ApplyTheme(OrganicUtility.GetWindowsTheme());
        }
    }
}
