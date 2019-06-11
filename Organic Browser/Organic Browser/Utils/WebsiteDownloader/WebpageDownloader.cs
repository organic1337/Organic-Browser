using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Net;
using System.Text.RegularExpressions;
using System.Web;
using System.Text;
using Organic_Browser.Utils;

namespace Organic_Browser.Utils.WebsiteDownloader
{
    /// <summary>
    /// This class wraps the html code making it available
    /// to download all of it's resources so we could view the
    /// html page offline.
    /// </summary>
    class WebpageDownloader
    {
        // Private & Static fields
        private static readonly string CssDirectoryName = "css";                        // The name of the directory that contains the css
        private static readonly string JsDirectoryName = "js";                          // The name of the directory that contains the javascript files
        private static readonly string ImgsDirectoryName = "images";                    // The name of the directory that contains the images
        private static readonly string CssResourcesDirectoryName = "css_resources";     // A directory that contains the files that the css file use
        private static readonly string MainPageName = "index.html";                     // The name of the local page
        private static readonly Encoding DefaultEncoding = Encoding.UTF8;               // The default encoding for the downloaded html

        // Public Properties
        public string HtmlCode { get; private set; }    // Html code
        public string Url { get; private set; }         // Url to download

        // Private fields
        private string basePath;
        private HtmlDocument htmlDoc;                           // Parsed html page
        private WebClient webClient;                            // Http client for downloading the pages
        private int imgId = 0;                                  // Used for naming the images files
        private int jsId = 0;                                   // Used for naming the javascript files
        private int cssId = 0;                                  // Used for naming the css files
        private int cssResourceId = 0;                          // Used for naming the resources inside the css files
        private enum Resource { Img, Css, Js, CssResource };    // Kinds of resources that can exist
        private Dictionary<string, string> resourcesNames;      // mapping between the downloaded url and local resource path

        // Public events
        public event EventHandler StartedDownloading;
        public event EventHandler FinishedDownloading;

        // Private properties
        private string CssResourcesPath { get { return Path.Combine(this.basePath, CssResourcesDirectoryName); } }
        private string CssPath { get { return Path.Combine(this.basePath, CssDirectoryName); } }
        private string JsPath { get { return Path.Combine(this.basePath, JsDirectoryName); } }
        private string ImgsPath { get { return Path.Combine(this.basePath, ImgsDirectoryName); } }
        private string MainPagePath { get { return Path.Combine(this.basePath, MainPageName); } }

        /// <summary>
        /// Url Downloader constructor
        /// </summary>
        /// <param name="url">a url that the client want to download</param>
        /// <param name="path">Destination path</param>
        public WebpageDownloader(string url, string path, string websiteName = "Downloaded Website")
        {
            this.webClient = new WebClient();            
            this.webClient.Encoding = DefaultEncoding;                  // Set the encoding
            string html = this.webClient.DownloadString(url);
            this.Url = url;
            this.HtmlCode = html;
            this.htmlDoc = new HtmlDocument(html);                      // Parse the html code
            this.resourcesNames = new Dictionary<string, string>();

            // Fix the url if invalid
            if (!this.Url.EndsWith("/"))
                this.Url += "/";

            // Create a new main directory
            this.basePath = Path.Combine(path, websiteName);
            
            if (Directory.Exists(this.basePath))
            {
                // Add an id to the directory
                this.basePath += Directory.EnumerateDirectories(path).Count(
                    dir => Path.GetFileName(dir).StartsWith(websiteName)).ToString();  
            }
            Directory.CreateDirectory(basePath);

            // Create sub directories
            Directory.CreateDirectory(this.CssPath);            // Create a directory for css files
            Directory.CreateDirectory(this.JsPath);             // Create a directroy for javascript files
            Directory.CreateDirectory(this.ImgsPath);           // Create a directory for images
            Directory.CreateDirectory(this.CssResourcesPath);   // Create a directory for resources retrieved from the css file

        }

        /// <summary>
        /// Downloads the html file with all of the resources.
        /// Images, Javascripts etc...
        /// </summary>
        public void Download()
        {
            this.OnStartedDownloading();            // downloading is starting

            // Download all the resources
            this.DownloadFavicon();
            this.DownloadImages();
            this.DownloadCss();
            this.DownloadJs();
            this.InsertLinks();                     // Insert the link into the html code
            this.RemoveSrcsetAttribute();           // Remove srcset attribute that make images disappear
            this.SaveMainPage();                    // Save the html code into a file

            this.OnFinishedDownloading();           // downloading is finished
        }

        /// <summary>
        /// Downloads the images from the html page to the given path
        /// </summary>
        /// <param name="path">path to download the images to</param>
        public void DownloadImages()
        {
            foreach (HtmlElement imgTag in htmlDoc.GetElementsByTagName("img"))
            {
                if (imgTag.Attributes.Keys.Contains("src"))
                {
                    string url = imgTag.Attributes["src"];         // Get the attribute's value from the element
                    string absoluteUrl = GetAbsoluteUrl(this.Url, url);    // Get the absolute furl from the given url

                    if (!this.IsDownloaded(absoluteUrl) && url.Length > 0)
                    {
                        Console.WriteLine("DEBUG: {0} Downloading {1}", this.imgId, absoluteUrl);

                        string extension = this.GetLastDownloadedFileExtension();
                        string filePath = Path.Combine(this.ImgsPath, imgId.ToString() + extension);     // The local file path             
                        try
                        {
                            this.webClient.DownloadFile(absoluteUrl, filePath);
                            this.RegisterDownload(url, filePath, Resource.Img);
                        }
                        catch (WebException ex)
                        {
                            Console.WriteLine("DEBUG: ERROR: Failed to download {0}  MESSAGE==>{1}", absoluteUrl, ex.Message);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Downloads the Stylesheets from the given url, Including the resources that
        /// The stylesheet uses like another stylesheets, images etc...
        /// </summary>
        public void DownloadCss()
        {
            // Download the css files and their resources
            bool FilterLinks(HtmlElement e) => e.TagName.ToLower() == "link" && e.Attributes.Keys.Contains("rel") && e.Attributes.Keys.Contains("href") && e.Attributes["rel"].ToLower() == "stylesheet";
            foreach (HtmlElement element in htmlDoc.GetElementsBy(FilterLinks))
            {
                string url = element.Attributes["href"];
                string absoluteUrl = GetAbsoluteUrl(this.Url, url);

                if (!this.IsDownloaded(absoluteUrl))
                {
                    this.DownloadCssRecursively(this.webClient.DownloadString(absoluteUrl), url);
                    Console.WriteLine("DEBUG: Downloaded {0}", absoluteUrl);
                }
            }

            // Download the css resources that appear inside the html
            foreach (HtmlElement element in htmlDoc.GetElementsByTagName("style"))
            {
                var urlRegex = new Regex(@"url\([""']?([-a-zA-Z0-9@:%_\+.~#?&//=]*)[""']?\)");  // The url is in group[1]
                foreach (Match match in urlRegex.Matches(element.Content))
                {
                    string url = match.Groups[1].Value;
                    string absoluteUrl = GetAbsoluteUrl(this.Url, url);
                    // In case this url was not downloaded yet
                    if (!this.IsDownloaded(absoluteUrl))
                    {
                        try
                        {
                            string extension = this.GetCssResourceExtension(url);
                            string filePath = Path.Combine(this.CssResourcesPath, cssResourceId.ToString() + extension);
                            this.webClient.DownloadFile(absoluteUrl, filePath);
                            this.RegisterDownload(url, filePath, Resource.CssResource);
                            Console.WriteLine("DEBUG: Downloaded {0}", absoluteUrl);
                        }
                        catch (WebException ex)
                        {
                            Console.WriteLine("DEBUG: ERROR: Failed downloading {0}   ==> MESSAGE: {1}", absoluteUrl, ex.Message);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Downloads the javascript from the html page
        /// </summary>
        public void DownloadJs()
        {
            foreach (HtmlElement element in htmlDoc.GetElementsByTagName("script"))
            {
                // In case the script element has an 'src' attribute
                if (element.Attributes.Keys.Contains("src"))
                {
                    string src = element.Attributes["src"];
                    string absoluteUrl = GetAbsoluteUrl(this.Url, src);

                    if (!this.IsDownloaded(absoluteUrl))
                    {
                        string extension = this.GetLastDownloadedFileExtension();
                        string filePath = Path.Combine(this.JsPath, this.jsId.ToString() + extension);     // The local file path
                        try
                        {
                            this.webClient.DownloadFile(absoluteUrl, filePath);
                            this.RegisterDownload(absoluteUrl, filePath, Resource.Img);
                            Console.WriteLine("DEBUG: Downloaded {0}", absoluteUrl);
                        }
                        catch (WebException ex)
                        {
                            Console.WriteLine("DEBUG: ERROR: Failed to download {0}  MESSAGE==>{1}", absoluteUrl, ex.Message);
                        }
                    }
                }
            }

        }

        #region Private Methods
        /// <summary>
        /// Downloads the page favicon
        /// </summary>
        private void DownloadFavicon()
        {
            string faviconUrl = "https://www.google.com/s2/favicons?domain=" + OrganicUtility.GetDomainName(this.Url);
            this.webClient.DownloadFile(faviconUrl, Path.Combine(this.basePath, "favicon.ico"));
        }

        /// <summary>
        /// Returns the mime type of the last response.
        /// for example -> if an image was downloaded, it may return .png, .jpg etc...
        /// </summary>
        /// <returns></returns>
        private string GetLastDownloadedFileExtension()
        {            
            try
            {
                string mimeType = this.webClient.ResponseHeaders[HttpResponseHeader.ContentType];
                if (mimeType.Contains(";"))
                    mimeType = mimeType.Remove(mimeType.IndexOf(';'));
                return MimeTypeUtility.GetExtension(mimeType);
            }
            catch (Exception)
            {
                return "";
            }            
        }

        /// <summary>
        /// Removes srcset from the html code because it's making the images
        /// disappear
        /// </summary>
        private void RemoveSrcsetAttribute()
        {
            var srcsetRegex = new Regex(@"srcset=[""'].+[""']", RegexOptions.IgnoreCase);
            foreach (Match match in srcsetRegex.Matches(this.HtmlCode))
                this.HtmlCode = HtmlCode.Replace(match.Value, "");
        }

        /// <summary>
        /// Inserts the downloaded local files into the html code
        /// so that the links in srcs will refer to the local files.
        /// </summary>
        private void InsertLinks()
        {
            foreach (KeyValuePair<string, string> pair in resourcesNames)
            {
                Console.WriteLine("DEBUG: Replacing {0} -> {1}", pair.Key, pair.Value);

                this.HtmlCode = HtmlCode.Replace(pair.Key, GetRelativeUrl(pair.Value));
            }
        }

        /// <summary>
        /// Saves the main page in the given path
        /// </summary>
        /// <param name="path">Path the save the html file in</param>
        private void SaveMainPage()
        {
            using (StreamWriter sw = new StreamWriter(this.MainPagePath))
                sw.Write(this.HtmlCode);
        }

        /// <summary>
        /// Returns a relative url of a downloaded file, the result url
        /// is relative to the main page file.
        /// </summary>
        /// <returns>Relative url of a downloaded file</returns>
        private static string GetRelativeUrl(string filePath)
        {
            string relativeDirectory = string.Empty;
            // In case filePath contains the path to a css resource
            if (filePath.Contains(CssResourcesDirectoryName))
                relativeDirectory = CssResourcesDirectoryName;
            // In case filePath contains the path to a css file
            else if (filePath.Contains(CssDirectoryName))
                relativeDirectory = CssDirectoryName;
            // In case filePath contains the path to a javascript file
            else if (filePath.Contains(JsDirectoryName))
                relativeDirectory = JsDirectoryName;
            // In case filePath contains the path to an image
            else if (filePath.Contains(ImgsDirectoryName))
                relativeDirectory = ImgsDirectoryName;

            return Path.Combine(relativeDirectory, Path.GetFileName(filePath)).Replace(Path.DirectorySeparatorChar, '/');
        }

        /// <summary>
        /// Determines whether the given url is relative or not
        /// </summary>
        /// <param name="url">URL to check</param>
        /// <returns>Whether the given URL is relative</returns>
        private static bool IsRelativeUrl(string url)
        {
            return !url.StartsWith("http");
        }

        /// <summary>
        /// Returns the absolute Url of the relative url.
        /// for example:
        ///     current = "https://www.google.com", relativeUrl = "/helloworld"     -> "https://www.google.com/helloworld"
        /// </summary>
        /// <param name="baseUrl"></param>
        /// <param name="relativeUrl"></param>
        private static string GetAbsoluteUrl(string currentUrl, string relativeUrl)
        {
            ////Unescape the urls (remove html escapings)
            relativeUrl = HttpUtility.HtmlDecode(relativeUrl);

            // Get base url, for example 'https://www.google.com/foo/foo1/foo2' -> base url = 'https://www.google.com'
            string baseUrl = Regex.Match(currentUrl, @"https?://[a-zA-Z\.0-9]*").Groups[0].Value;
            bool isSecured = baseUrl.StartsWith("https");

            // In case starts with double slash
            if (relativeUrl.StartsWith("//"))
            {
                if (isSecured)
                    return "https:" + relativeUrl;
                else
                    return "http:" + relativeUrl;
            }
            // In case starts with slash
            else if (relativeUrl.StartsWith("/"))
                return baseUrl + relativeUrl;
            // in case the relative url is actually an absolute url
            else if (relativeUrl.StartsWith("http://") || relativeUrl.StartsWith("https://"))
                return relativeUrl;
            // In case the relative url starts with '..'
            else if (relativeUrl.StartsWith(".."))
            {
                string resultUrl = currentUrl;
                // In case url ends with '/', remove it
                if (relativeUrl.EndsWith("/"))
                    relativeUrl.Remove(relativeUrl.Length - 1);

                // Move backwards in the url
                resultUrl = resultUrl.Remove(resultUrl.LastIndexOf('/'));
                while (relativeUrl.StartsWith(".."))
                {
                    resultUrl = resultUrl.Remove(resultUrl.LastIndexOf('/'));
                    relativeUrl = relativeUrl.Substring(relativeUrl.IndexOf('/') + 1);
                }
                resultUrl += "/" + relativeUrl;     // Add the relative url to the result
                return resultUrl;
            }
            // In case the relative url is just a relative url
            else
                return currentUrl + relativeUrl;
        }


        /// <summary>
        /// Sometimes the given mime type is invalid, instead of .svg we get .png, 
        /// this method checks the url for an extension.
        /// </summary>
        /// <param name="url">Css resource url</param>
        /// <returns>The extension of the resource</returns>
        private string GetCssResourceExtension(string url)
        {
            if (url.EndsWith(".png"))
                return ".png";
            else if (url.EndsWith(".svg"))
                return ".svg";
            else
                return this.GetLastDownloadedFileExtension();
        }

        /// <summary>
        /// Used to mark a url as downloaded, In order to be able to retreive it's file name
        /// and insert it to the page.
        /// </summary>
        /// <param name="url">Absolute url to the downloaded file</param>
        /// <param name="resourceId">Id of the given file</param>
        private void RegisterDownload(string url, string filePath, Resource resourceType)
        {
            switch (resourceType)
            {
                case Resource.Css:
                    this.cssId++;
                    break;

                case Resource.Img:
                    this.imgId++;
                    break;

                case Resource.Js:
                    this.jsId++;
                    break;

                case Resource.CssResource:
                    this.cssResourceId++;
                    break;

                default:
                    throw new NotImplementedException();
            }

            this.resourcesNames.Add(url, filePath);
        }


        /// <summary>
        /// Checks whether the url was downloaded.
        /// </summary>
        /// <param name="absoluteUrl"> Absolute url to a resource. </param>
        /// <returns> whether the url was downloaded. </returns>
        private bool IsDownloaded(string absoluteUrl)
        {
            return this.resourcesNames.Keys.Any(relativeUrl => GetAbsoluteUrl(this.Url, relativeUrl) == absoluteUrl);
        }


        /// <summary>
        /// Downloads the css resources. for example a css file may contain 'url("url/to/resource)"', in that case,
        /// we have to download this resource
        /// 
        /// TODO: CHECKK!!
        /// </summary>
        /// <param name="cssCode"></param>
        /// <returns></returns>
        private void DownloadCssRecursively(string cssCode, string stylesheetUrl, string currentUrl = "")
        {
            // Get the absolute url of the current position
            if (currentUrl == string.Empty) currentUrl = this.Url;
            string absoluteCurrentUrl = stylesheetUrl;  // The absolute url of the location the code is currently in
            if (IsRelativeUrl(absoluteCurrentUrl))
                absoluteCurrentUrl = GetAbsoluteUrl(currentUrl, absoluteCurrentUrl);

            // Regexes for finding the resources used inside the css code
            var urlRegex = new Regex(@"url\([""']?([-a-zA-Z0-9@:%_\+.~#?&//=]*)[""']?\)");  // The url is in group[1]
            var importRegex = new Regex(@"@import (?:url\()?[""']?([-a-zA-Z0-9@:%_\+.~#?&//=]*)[""']?\)?"); // The url is in group[1]

            var resourcesUrlList = new List<string>();      // A list of all urls that are used
            var importsUrlList = new List<string>();        // A list of the url of the imported css sheets

            // Build the resources list
            foreach (Match match in urlRegex.Matches(cssCode))
            {
                // In case the resources url list does not contain this url
                string url = match.Groups[1].Value;
                if (!resourcesUrlList.Contains(url))
                    resourcesUrlList.Add(url);
            }

            // Build the imported urls list
            foreach (Match match in importRegex.Matches(cssCode))
            {
                string url = match.Groups[1].Value;

                // In case the import urls list does not contain this url
                if (!importsUrlList.Contains(url))
                    importsUrlList.Add(url);    // Add it to the list

                // In case the resources url list contain this url
                if (resourcesUrlList.Contains(url))
                    resourcesUrlList.Remove(url);   // Remove it from the list
            }

            // Download all resources used in this stylesheet
            foreach (string url in resourcesUrlList)
            {   
                string absoluteUrl = GetAbsoluteUrl(absoluteCurrentUrl, url);
                // In case the resource was not downloaded yet
                if (!this.IsDownloaded(absoluteUrl))
                {
                    string _extension = GetCssResourceExtension(url);
                    string filePath = Path.Combine(this.CssResourcesPath, this.cssResourceId.ToString() + _extension);
                    this.RegisterDownload(absoluteUrl, filePath, Resource.CssResource);
                    try
                    {
                        this.webClient.DownloadFile(absoluteUrl, filePath);
                        Console.WriteLine("DEBUG: Downloaded {0}", absoluteUrl);
                        cssCode = cssCode.Replace(url, "../" + GetRelativeUrl(filePath));    // Replace url by the local file (Relative to the css file)
                    }
                    catch (System.Net.WebException ex)
                    {
                        Console.WriteLine("DEBUG: ERROR: Could not download {0}    MESSAGE ==> {1}", absoluteUrl, ex.Message);
                    }
                }
                // In case this resource was already downloaded
                else
                {
                    cssCode = cssCode.Replace(url, GetRelativeUrl("../" + this.resourcesNames[url]));   // Replace url by the local file ( Relative to the css file)
                }
            }

            // Download all imported stylesheets
            foreach (string url in importsUrlList)
            {
                string absoluteUrl = GetAbsoluteUrl(absoluteCurrentUrl, url);

                // In case the stylesheet was not downloaded yet
                if (!this.IsDownloaded(absoluteUrl))
                {
                    try
                    {
                        string innerCssCode = this.webClient.DownloadString(url);

                        this.DownloadCssRecursively(innerCssCode, url);     // Recursively download the inner css file
                        Console.WriteLine("DEBUG: Downloaded inner stylesheet {0}", absoluteUrl);

                        int previousId = cssId - 1;     // The id of the inner css resource
                        cssCode.Replace(url, CssDirectoryName + "/" + previousId.ToString());   // Replace the url by the local location of the stylesheet
                    }
                    catch (WebException ex)
                    {
                        Console.WriteLine("DEBUG: ERROR: Failed to download {0}  MESSAGE==>{1}", absoluteUrl, ex.Message);                        
                    }
                }
                // In case the stylesheet was already downloaded
                else
                {
                    cssCode.Replace(url, GetRelativeUrl(this.resourcesNames[absoluteUrl]));  // Replace the url by the local location of the stylesheet
                }

            }

            // Save the css file
            string extension = ".css";
            string finalCssFilePath = Path.Combine(this.CssPath, this.cssId.ToString() + extension);            
            using (StreamWriter stream = new StreamWriter(finalCssFilePath))
                stream.Write(cssCode);
            this.RegisterDownload(stylesheetUrl, finalCssFilePath, Resource.Css);
        }
        #endregion

        #region Protected Event Methods
        /// <summary>
        /// Invoking the StartedDownloading event subscribers.
        /// </summary>
        protected virtual void OnStartedDownloading() => this.StartedDownloading?.Invoke(this, EventArgs.Empty);

        /// <summary>
        /// Invoking the FinishedDownloading event subscribers.
        /// </summary>
        protected virtual void OnFinishedDownloading() => this.FinishedDownloading?.Invoke(this, EventArgs.Empty);
        #endregion
    }
}
