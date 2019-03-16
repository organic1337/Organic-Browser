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
    /// Interaction logic for SettingsDropdownControl.xaml
    /// </summary>
    public partial class SettingsDropdownControl : UserControl
    {
        public SettingsDropdownControl()
        {
            InitializeComponent();
        }

        private void Preferences_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (!PreferencesWindow.IsRunning)
            {                
                System.Threading.Thread preferencesThread = new System.Threading.Thread(() =>
                {
                    var window = new PreferencesWindow();
                    window.Show();
                    System.Windows.Threading.Dispatcher.Run();
                });
                preferencesThread.IsBackground = true;
                preferencesThread.SetApartmentState(System.Threading.ApartmentState.STA);
                preferencesThread.Start();
            }
        }
    }
}
