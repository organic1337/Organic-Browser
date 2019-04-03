using System;
using System.IO;
using Organic_Browser.Utils;
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


namespace Organic_Browser.Controls
{
    /// <summary>
    /// Interaction logic for NavigationBarControl.xaml
    /// </summary>
    public partial class NavigationBarControl : UserControl
    {
        /// <summary>
        /// Whether back button is enabled
        /// </summary>
        public bool BackEnabled
        {
            get { return this.backBtn.IsEnabled; }
            set { this.backBtn.IsEnabled = value; }
        }

        /// <summary>
        /// Whether forward button is enabled
        /// </summary>
        public bool ForwardEnabled
        {
            get { return this.forwardBtn.IsEnabled; }
            set { this.forwardBtn.IsEnabled = value; }
        }

        /// <summary>
        /// The url text box content
        /// </summary>
        public string Url
        {
            get {
                return this.urlTextBox.Text;
            }
            set
            {
                // Make sure that the value is not null or empty
                if (string.IsNullOrEmpty(value))
                    return;

                // In case the url is not a local url
                if (!OrganicWebUtility.IsLocalPageUrl(value))
                    this.urlTextBox.Text = value;    
                // In case the url IS a local url
                else
                {
                    // Show the url only if the url is not an error
                    string organicUrl = OrganicWebUtility.GetOrganicUrl(value);
                    if (!OrganicWebUtility.IsErrorPage(organicUrl))
                        this.urlTextBox.Text = organicUrl;

                }
                
               
            }
        }

        public NavigationBarControl()
        {
            InitializeComponent();

            this.mainGrid.MouseLeftButtonDown += (object obj, MouseButtonEventArgs e) => this.MakeSettingsMenuInvisible();
            this.settingsBtn.MouseLeftButtonUp += (object obj, MouseButtonEventArgs e) => this.MakeSettingsMenuVisible();            
        }

        /// <summary>
        /// Makes the settings dropdown menu invisible
        /// </summary>
        public void MakeSettingsMenuVisible()
        {
            if (this.settingsMenu.Visibility == Visibility.Hidden)
                this.settingsMenu.Visibility = Visibility.Visible;
        }

        /// <summary>
        /// Makes the settings dropdown menu invisible
        /// </summary>
        public void MakeSettingsMenuInvisible()
        {
            if (this.settingsMenu.Visibility == Visibility.Visible)
                this.settingsMenu.Visibility = Visibility.Hidden;
        }
    }
}
