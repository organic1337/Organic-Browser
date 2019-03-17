using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

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
    }
}
