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
            get { return this.urlTexBox.Text; }
            set { this.urlTexBox.Text = value; }
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
