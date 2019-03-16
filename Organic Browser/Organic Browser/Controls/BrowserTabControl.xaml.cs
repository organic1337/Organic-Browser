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
    /// Interaction logic for TabControl.xaml
    /// </summary>
    public partial class TabControl : UserControl
    {
        private int tabCount = 1;                           // Counts the number of tabs
        private const double TabWidth = 150;                // Width of the tab header
        private const double PlusButtonMarginTop = 5;       // The margin of the + button from top
        private const double PlusButtonMarginLeft = 5;      // The margin of the + button from top

        public TabControl()
        {
            InitializeComponent();

            // Margin the add new tab button
            this.addNewTabButton.Margin = this.GetAddNewTabButtonMargin;    
        }

        private void AddTab(string header, UIElement content)
        {
            tabCount++; // Increase the tab count by 1

            // Move the + button right
            this.addNewTabButton.Margin = this.GetAddNewTabButtonMargin;

            // Create a tab item and add it to the tab control;
            TabItem item = this.CreateTabItem(header);
            item.Content = content;                         // Put the given content inside the tab item
            this.tabControl.Items.Add(item);                // Add the complete tab to the UI
        }

        /// <summary>
        /// Executes when the add new tab button is clicked
        /// </summary>
        /// <param name="sender">object sender</param>
        /// <param name="e">event args</param>
        private void AddNewTabButton_Click(object sender, RoutedEventArgs e)
        {
            AddTab("New Tab", null);
        }

        /// <summary>
        /// Creates a new tab item
        /// </summary>
        /// <param name="header"></param>
        /// <returns></returns>
        private TabItem CreateTabItem(string header)
        {
            TabItem item = new TabItem();
            // Create all the UI elements
            var dockPanel = new DockPanel();                        // Header's dockpanel
            var label = new Label()
            {                                                       // Header's label
                Content = header,
                Style = FindResource("headerLabel") as Style
            };
            var imageWrapper = new Border()                         // Header's x image wrapper
            {
                Style = this.FindResource("xButtonImageWrapper") as Style
            };
            var image = new Image()                                 // Header's x Image
            {
                Style = this.FindResource("xImage") as Style
            };

            // Arrange the UI elements together and set the header to the result
            imageWrapper.Child = image;
            dockPanel.Children.Add(label);
            dockPanel.Children.Add(imageWrapper);
            item.Header = dockPanel;

            // Handle the x button press
            imageWrapper.MouseDown += (object sender, MouseButtonEventArgs e) =>
            {
                this.tabCount--;                                                // Decrease the tabCount
                this.tabControl.Items.Remove(item);                             // Remove the tab from the UI
                this.addNewTabButton.Margin = this.GetAddNewTabButtonMargin;    // Margin the add new tab button
            };

            return item;
        }

        /// <summary>
        /// Current margin of the add new tab button
        /// </summary>
        private Thickness GetAddNewTabButtonMargin
        {
            get
            {
                return new Thickness(TabWidth * this.tabCount + PlusButtonMarginLeft, PlusButtonMarginTop, 0, 0);
            }            
        }
    }
}
