using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Linq;
using System.Text.RegularExpressions;

namespace Organic_Browser.Utils
{
    class OrganicWebUtility
    {
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
        public static readonly string LocalPagesPath = Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "pages");

        static OrganicWebUtility()
        {
            // Regular Pages
            LocalPages.Add("organic://history", GetUrlFromPath(Path.Combine(LocalPagesPath, "history", "index.html")));

            // Error pages
            LocalPages.Add("organic://error/general", GetUrlFromPath(Path.Combine(LocalPagesPath, "general", "index.html")));
            LocalPages.Add("organic://error/no_internet", GetUrlFromPath(Path.Combine(LocalPagesPath, "no_internet", "index.html")));
            LocalPages.Add("organic://error/could_not_resolve", GetUrlFromPath(Path.Combine(LocalPagesPath, "could_not_resolve", "index.html")));
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
    }
}
