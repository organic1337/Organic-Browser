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
    /// Interaction logic for SavedPageControl.xaml
    /// 
    /// This control represents a single saved page.
    /// </summary>
    public partial class SavedPageControl : UserControl
    {
        // Public properties
        public string Title { get; set; }               // Webpage title
        public ImageSource IconSource { get; set; }     // Webpage icon source

        public SavedPageControl()
        {
            InitializeComponent();

            // Set the data context to this in order to enable the data binding
            this.DataContext = this;    
        }
    }
}
