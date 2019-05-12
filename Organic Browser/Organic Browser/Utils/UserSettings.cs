using System;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;

namespace Organic_Browser.Utils
{
    /// <summary>
    /// Contains all the user settings such as home page, and downloaded webpages location
    /// 
    /// (Implements Singleton design pattern)
    /// </summary>
    [DataContract]
    class UserSettings
    {
        private static UserSettings Instance = null;    // Single instance  
        
        // Private readonly attributes
        private const string JsonPath = "settings.json";                                            // Path to the json file
        private const string DefaultHomePage = "www.google.com";                                    // Default path to home page
        private const string DefaultNewTabPage = "www.google.com";
        private static readonly string DefaultDownloadPagesLocation = Environment.GetFolderPath(    // Default location to download webpages
            Environment.SpecialFolder.Desktop);     

        // Public properties (Data members in the json file)
        [DataMember(Name = "homePage")]
        public string HomePage { get; set; }                       // Home page URL
        [DataMember(Name = "downloadWebPagesLocation")]
        public string DownloadWebpagesLocation { get; set; }       // Downloaded webpage location
        [DataMember(Name = "newTabPage")]
        public string NewTabPage { get; set; }                     // Page that opens a new tab

        // Private constructor
        private UserSettings() { }

        /// <summary>
        /// Saves the settings to the json file
        /// </summary>
        public void Save()
        {
            Stream stream = new StreamWriter(JsonPath).BaseStream;
            try
            {
                var serializer = new DataContractJsonSerializer(typeof(UserSettings));
                serializer.WriteObject(stream, this);
            }
            finally
            {
                stream.Dispose();
            }
        }

        /// <summary>
        /// Loads the User settings from the json file (if exists)
        /// </summary>
        /// <returns></returns>
        public static UserSettings Load()
        {
            // In case settings file was not loaded yet
            if (Instance == null)
            {
                // In case file exists
                if (File.Exists(JsonPath))
                {
                    var stream = new StreamReader(JsonPath).BaseStream;
                    var serializer = new DataContractJsonSerializer(typeof(UserSettings));
                    UserSettings result = (UserSettings)serializer.ReadObject(stream);
                    stream.Dispose();
                    Instance = result;
                }
                // In case file does NOT exist
                else
                {
                    // Return a result with default settings
                    var result = new UserSettings
                    {
                        HomePage = DefaultHomePage,
                        DownloadWebpagesLocation = DefaultDownloadPagesLocation,
                        NewTabPage = DefaultNewTabPage
                    };
                    result.Save();  // Create a new json file with default values
                    Instance = result;
                }
            }
            return Instance;
        }
    }
}
