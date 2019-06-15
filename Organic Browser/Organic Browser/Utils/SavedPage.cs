using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Organic_Browser.Utils
{
    /// <summary>
    /// Represents a single saved page
    /// </summary>
    class SavedPage
    {
        private const string DefaultIconSource = "pack://application:,,,/Resources/Images/Buttons/Light/default-favicon.png";

        // Public properties
        public string IconSource { get; private set; }
        public string Title { get; private set; }
        public string HtmlFilePath { get; private set; }

        /// <summary>
        /// SavedPage constructor
        /// </summary>
        /// <param name="webPagePath">Path to the folder of the saved page</param>
        public SavedPage(string webPagePath)
        {
            if (!Directory.Exists(webPagePath))
                throw new DirectoryNotFoundException();

            // Find the html file path
            this.HtmlFilePath = Path.Combine(webPagePath, "index.html");

            // Find the title
            using (StreamReader reader = new StreamReader(HtmlFilePath))
            {
                Regex regex = new Regex(@"<title>(?<title>[^<>]+)</title>", RegexOptions.IgnoreCase | RegexOptions.Multiline);
                this.Title = regex.Match(reader.ReadToEnd()).Groups["title"].Value;
                if (string.IsNullOrEmpty(Title))
                    this.Title = "Untitled";
            }

            // Find the icon source
            string iconSource = Path.Combine(webPagePath, "favicon.ico");
            if (File.Exists(iconSource))
                this.IconSource = iconSource;
            else
                this.IconSource = DefaultIconSource;

            
        }
    }
}
