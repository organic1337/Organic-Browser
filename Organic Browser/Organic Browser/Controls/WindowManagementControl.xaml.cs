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
using Organic_Browser.Utils;

namespace Organic_Browser.Controls
{
    /// <summary>
    /// Interaction logic for WindowManagementControl.xaml
    /// </summary>
    public partial class WindowManagementControl : UserControl
    {
        public WindowManagementControl()
        {
            InitializeComponent();

            // Handle theme changing
            (Application.Current as App).ThemeChanged += (object obj, ThemeChangedEventArgs e) =>
            {
                OrganicUtility.UpdateImages(this.mainLayout);
            };
            Application.Current.Activated += (object obj, EventArgs e) => OrganicUtility.UpdateImages(this.mainLayout);
        }

        /// <summary>
        /// Executes when the mouse enters the wrapper
        /// </summary>
        /// <param name="sender">Event sender</param>
        /// <param name="e">Event args</param>
        private void CloseImageWrapper_MouseEnter(object sender, MouseEventArgs e)
        {
            // Make the close image brighter
            ImageSourceConverter converter = new ImageSourceConverter();
            this.closeImage.Source = (ImageSource)converter.ConvertFromString("pack://application:" +
                ",,,/Resources/Images/Buttons/Dark/close.png");

            // Make the close background red
            this.closeImageWrapper.Background = (SolidColorBrush)(new BrushConverter().ConvertFrom("#e80000"));
        }

        /// <summary>
        /// Executes when the mouse leaves the wrapper
        /// </summary>
        /// <param name="sender">Event sender</param>
        /// <param name="e">Event args</param>
        private void CloseImageWrapper_MouseLeave(object sender, MouseEventArgs e)
        {
            // Return the image to its original state
            ImageSourceConverter converter = new ImageSourceConverter();
            this.closeImage.Source = (ImageSource)converter.ConvertFromString((string)this.FindResource("closeSource"));

            // Make the close background transparent
            this.closeImageWrapper.Background = (SolidColorBrush)(new BrushConverter().ConvertFromString("Transparent"));
        }
    }
}
