using System;
using System.IO;
using System.Net;
using System.Linq;
using System.Windows;
using System.Reflection;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Controls;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Microsoft.Win32;

namespace Organic_Browser.Utils
{
    class OrganicUtility
    {
        #region Web Utilities

        // Private -  Read only data
        private readonly static Regex UrlRegex = new Regex(@"^(http:\/\/www\.|https:\/\/www\.|http:\/\/|https:\/\/)?[a-z0-9]+([\-\.]{1}[a-z0-9]+)*\.[a-z]{2,5}(:[0-9]{1,5})?(\/.*)?$", RegexOptions.Compiled | RegexOptions.IgnoreCase);

        /// <summary>
        /// Returns whether the given URL is valid
        /// </summary>
        /// <param name="url">Url to validate</param>
        /// <returns>URL validation</returns>
        public static bool IsValidUrl(string url)
        {
            return UrlRegex.IsMatch(url);
        }

        /// <summary>
        /// Checks whether the computer is connected to the internet
        /// </summary>
        /// <returns></returns>
        public static bool IsConnected()
        {
            try
            {
                using (var client = new WebClient())
                using (client.OpenRead("http://clients3.google.com/generate_204"))
                {
                    return true;
                }
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Return the url of the given path:
        /// C:\Users\username\index.html -> file:///C:/Users/username/index.html
        /// </summary>
        /// <param name="path">path to the file</param>
        /// <returns>Url to the file</returns>
        public static string GetUrlFromPath(string path)
        {
            string url = string.Format("file:///{0}", path.Replace('\\', '/').ToLower());
            string urlEncoded = Uri.EscapeUriString(url);
            return urlEncoded;
        }


        #region Local Pages
        // Map the organic urls with the actual urls ("organic://...": "file://...")
        public static readonly Dictionary<string, string> LocalPages = new Dictionary<string, string>();       
        // Path to the local pages directory
        public static readonly string LocalPagesPath = System.IO.Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "pages");

        static OrganicUtility()
        {
            // Regular Pages
            LocalPages.Add("organic://history", GetUrlFromPath(System.IO.Path.Combine(LocalPagesPath, "history", "index.html")));

            // Error pages
            LocalPages.Add("organic://error/general", GetUrlFromPath(System.IO.Path.Combine(LocalPagesPath, "general", "index.html")));
            LocalPages.Add("organic://error/no_internet", GetUrlFromPath(System.IO.Path.Combine(LocalPagesPath, "no_internet", "index.html")));
            LocalPages.Add("organic://error/could_not_resolve", GetUrlFromPath(System.IO.Path.Combine(LocalPagesPath, "could_not_resolve", "index.html")));
        }

        /// <summary>
        /// Gets a url that starts with 'organic://' and returns whether it is 
        /// an error page or not
        /// </summary>
        /// <param name="localPageUrl">A local page</param>
        /// <returns>Whether an error page or not</returns>
        public static bool IsErrorPage(string organicUrl)
        {
            return organicUrl.StartsWith("organic://error");
        }

        /// <summary>
        /// Checks whether the given url is an organic url (like organic://history)
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public static bool IsOrganicUrl(string url)
        {
            return LocalPages.Keys.Contains(url);
        }

        /// <summary>
        /// Checks whether the given url is a local page 
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public static bool IsLocalPageUrl(string url)
        {
            return LocalPages.Values.Contains(url.ToLower());
        }

        /// <summary>
        /// Returns the actual path (file://... format) of the local page
        /// </summary>
        /// <param name="url">local page url like organic://history</param>
        /// <returns>Actual url</returns>
        public static string GetLocalPageActualUrl(string url)
        {
            url = url.ToLower();
            if (!IsOrganicUrl(url))
                throw new Exception("The given url is not a valid local url");
            
            return LocalPages[url];
        }

        /// <summary>
        /// Returns organic path (from file://... to organic://...)
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public static string GetOrganicUrl(string url)
        {
            url = url.ToLower();
            if (!IsLocalPageUrl(url))
                throw new Exception("The given url does not exist");
            
            return LocalPages.First((pair) => pair.Value == url.ToLower()).Key;
        }
        #endregion

        #endregion

        #region General Utilities
        /// <summary>
        /// Reads the registry for the windows theme.
        /// </summary>
        /// <returns></returns>
        public static Theme GetWindowsTheme()
        {
            int value;
            using (RegistryKey key = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Themes\Personalize"))
            {
                value = (int)key.GetValue("AppsUseLightTheme");
            }

            if (value == 0)
                return Theme.Dark;
            else
                return Theme.Light;
        }

        /// <summary>
        /// Returns the absolute path of a relative path, relatively to the executable location
        /// </summary>
        /// <returns>Absolute path</returns>
        public static string GetAbsolutePath(string relativePath)
        {
            return System.IO.Path.Combine(System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), relativePath);
        }

        /// <summary>
        /// Recursively updates the sources of all the images within the given element
        /// </summary>
        /// <param name="mainElement"></param>
        public static void UpdateImages(UIElement mainElement)
        {
            Action<UIElement> HandleChild = (UIElement child) =>
            {
                if (child is Image)
                    UpdateImageSource(child as Image);
                else if (child is Decorator || child is Panel || child is HeaderedContentControl || child is TabControl)
                    UpdateImages(child);
            };
            // In case the given element is a panel
            if (mainElement is Panel)
                foreach (UIElement child in (mainElement as Panel).Children)
                    HandleChild(child);
            // In case the given element is a Decorator
            else if (mainElement is Decorator)
                HandleChild((mainElement as Decorator).Child);
            else if (mainElement is TabControl)
                foreach (TabItem item in (mainElement as TabControl).Items)
                    HandleChild(item);
            // In case the given element is a HeaderedContentControl
            else if (mainElement is HeaderedContentControl)
            {
                object header = (mainElement as HeaderedContentControl).Header;
                if (header is UIElement)
                    HandleChild(header as UIElement);
            }            
        }

        /// <summary>
        /// Updates a single image source
        /// </summary>
        /// <param name="image"></param>
        private static void UpdateImageSource(Image image)
        {
            var converter = new ImageSourceConverter();
            switch ((Application.Current as App).CurrentTheme)
            {
                case Theme.Dark:
                    image.Source = (ImageSource)converter.ConvertFromString(
                        image.Source.ToString().Replace("Light", "Dark"));
                    break;

                case Theme.Light:
                    image.Source = (ImageSource)converter.ConvertFromString(
                        image.Source.ToString().Replace("Dark", "Light"));
                    break;

            }
        }
        #endregion
    }
}
