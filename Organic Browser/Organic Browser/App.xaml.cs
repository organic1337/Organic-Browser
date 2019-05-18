using System;
using Organic_Browser.Utils;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Navigation;

namespace Organic_Browser
{
    # region Theme enum and EventArgs
    /// <summary>
    /// Represents the general theme of the browser
    /// </summary>
    public enum Theme { Dark, Light, Auto }

    /// <summary>
    /// Event args for theme changed.
    /// </summary>
    public class ThemeChangedEventArgs
    {
        public Theme NewTheme;  // The new theme of the app

        public ThemeChangedEventArgs(Theme newTheme)
        {
            this.NewTheme = newTheme;
        }
    }
    #endregion
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public Theme CurrentTheme { get; set; }                             // Current app theme
        public event EventHandler<ThemeChangedEventArgs> ThemeChanged;      // Event for theme changed

        /// <summary>
        /// Executes when the theme is changed
        /// </summary>
        protected virtual void OnThemeChanged()
        {
            ThemeChanged?.Invoke(this, new ThemeChangedEventArgs(this.CurrentTheme));
        }

        /// <summary>
        /// Executes on application startup
        /// </summary>
        /// <param name="e"></param>
        protected override void OnStartup(StartupEventArgs e)
        {
            var settings = UserSettings.Load();
            if (settings.Theme == Theme.Auto)
                this.CurrentTheme = OrganicUtility.GetWindowsTheme();
            else
                this.CurrentTheme = settings.Theme;
            base.OnStartup(e);
        }

        protected override void OnActivated(EventArgs e)
        {
            string themePath = string.Empty;
            if (this.CurrentTheme == Theme.Dark)
                themePath = "pack://application:,,,/Resources/Themes/DarkTheme.xaml";
            if (this.CurrentTheme == Theme.Light)
                themePath = "pack://application:,,,/Resources/Themes/LightTheme.xaml";

            this.Resources.MergedDictionaries.Clear();
            this.Resources.MergedDictionaries.Add(new ResourceDictionary()
            {
                Source = new Uri(themePath, UriKind.Absolute)
            });

            base.OnActivated(e);
        }

        protected override void OnLoadCompleted(NavigationEventArgs e)
        {
            string themePath = string.Empty;
            if (this.CurrentTheme == Theme.Dark)
                themePath = "pack://application:,,,/Resources/Themes/DarkTheme.xaml";
            if (this.CurrentTheme == Theme.Light)
                themePath = "pack://application:,,,/Resources/Themes/LightTheme.xaml";
            
            this.Resources.MergedDictionaries.Clear();
            this.Resources.MergedDictionaries.Add(new ResourceDictionary()
            {
                Source = new Uri(themePath, UriKind.Relative)
            });

            base.OnLoadCompleted(e);
        }

        /// <summary>
        /// Applies the given theme
        /// </summary>
        /// <param name="Theme"></param>
        public void ApplyTheme(Theme theme)
        {
            // Do NOT change the theme to the same one
            if (theme == this.CurrentTheme)
                return;

            string themePath = string.Empty;
            if (theme == Theme.Dark)
            {
                this.CurrentTheme = Theme.Dark;
                themePath = "pack://application:,,,/Resources/Themes/DarkTheme.xaml";
            }
            if (theme == Theme.Light)
            {
                this.CurrentTheme = Theme.Light;
                themePath = "pack://application:,,,/Resources/Themes/LightTheme.xaml";
            }
            this.Resources.MergedDictionaries.Clear();
            this.Resources.MergedDictionaries.Add(
                new ResourceDictionary()
                {
                    Source = new Uri(themePath, UriKind.Absolute)
                });
            this.OnThemeChanged();
        }
    }
}
