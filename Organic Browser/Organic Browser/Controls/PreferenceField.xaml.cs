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
    /// Interaction logic for PreferenceField.xaml
    /// </summary>
    public partial class PreferenceField : UserControl
    {
        private string value;   // The value of the field

        public PreferenceField()
        {
            InitializeComponent();

            this.DataContext = this;
            this.valueTextBox.MouseDoubleClick += (object Sender, MouseButtonEventArgs e) => 
            {
                this.valueTextBox.SelectAll();
            };
        }

        /// <summary>
        /// The data appears in the label
        /// </summary>
        public string Field { get; set; }
        
        /// <summary>
        /// The data inside the textbox
        /// </summary>
        public string Value
        {
            get
            {
                return this.valueTextBox.Text;
            }
            set
            {
                this.value = value;
                this.valueTextBox.Text = value;
            }
        }
    }
}
