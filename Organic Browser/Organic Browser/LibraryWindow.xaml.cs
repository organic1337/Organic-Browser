using System;
using System.IO;
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
using System.Windows.Shapes;
using Organic_Browser.Utils;
using Organic_Browser.Controls;

namespace Organic_Browser
{
    /// <summary>
    /// Interaction logic for LibraryWindow.xaml.
    /// This window shows all the saved pages.
    /// </summary>
    public partial class LibraryWindow : Window
    {
        public static bool IsRunning = false;       // Whether the window is running or not
        public static LibraryWindow CurrentRunningWindow { get; private set; }  // The current running window

        public LibraryWindow()
        {
            InitializeComponent();

            // Mention that the window is running
            IsRunning = true;
            LibraryWindow.CurrentRunningWindow = this;

            // Insert all the saved pages
            this.InsertAllSavedPages();

            // Handle theme changing
            (Application.Current as App).ThemeChanged += (object obj, ThemeChangedEventArgs e) =>
            {
                OrganicUtility.UpdateImages(this.mainGrid);
            };
            Application.Current.Activated += (object obj, EventArgs e) => OrganicUtility.UpdateImages(this.mainGrid);
        }

        /// <summary>
        /// Executes when the window is closed
        /// </summary>
        /// <param name="e"></param>
        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
            IsRunning = false;
            CurrentRunningWindow = null;
        }

        /// <summary>
        /// Executes when the mouse left button is down on the close button
        /// </summary>
        /// <param name="sender">Event sender</param>
        /// <param name="e">Event args</param>
        private void CloseBtn_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.Close();
        }

        /// <summary>
        /// Inserts all the saved pages into the XAML page
        /// </summary>
        private void InsertAllSavedPages()
        {
            Grid pagesGrid = this.savedPagesGrid;
            int currentRow = 0, currentCol = 0;
            foreach (SavedPage savedPage in GetAllSavedPages())
            {
                // In case there aren't enough rows add one
                if (currentRow == pagesGrid.RowDefinitions.Count)
                    pagesGrid.RowDefinitions.Add(new RowDefinition() { Style = FindResource("savedPagesGridRow") as Style });

                // Create the save page control
                SavedPageControl control = new SavedPageControl()
                {
                    Title = savedPage.Title,
                    IconSource = new ImageSourceConverter().ConvertFromString(savedPage.IconSource) as ImageSource,
                    Height=135,
                    Width=145
                };
                // Handle saved page control click
                control.MouseLeftButtonUp += (object sender, MouseButtonEventArgs e) =>
                {
                    (Application.Current.MainWindow as MainWindow).AddNewTab(OrganicUtility.GetUrlFromPath(savedPage.HtmlFilePath));
                    this.Close();
                };
                pagesGrid.Children.Add(control);
                Grid.SetRow(control, currentRow);
                Grid.SetColumn(control, currentCol);

                if (currentCol + 1 == pagesGrid.ColumnDefinitions.Count)
                {
                    currentCol = 0;
                    currentRow++;
                }
                else
                    currentCol++;
            }
        }

        /// <summary>
        /// Loads all the saved pages from the appdata
        /// </summary>
        /// <returns>all the saved pages</returns>
        private static SavedPage[] GetAllSavedPages()
        {
            string[] dirs = Directory.GetDirectories(AppData.DownloadedPagesLocation);
            var pages = new SavedPage[dirs.Length];
            int index = 0;
            foreach (string dir in dirs)
            {
                pages[index] = new SavedPage(dir);
                index++;
            }
            return pages;
        }
    }
}
