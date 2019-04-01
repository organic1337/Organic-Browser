using System.Net;
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
    }
}
