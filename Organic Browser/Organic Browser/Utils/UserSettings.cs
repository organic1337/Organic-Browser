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
        private const string DefaultHomePage = "www.google.com";                                    // Default home page address
        private const string DefaultNewTabPage = "www.google.com";                                  // Default new tab page address
        private const string DefaultTheme = "auto";                                                 // Default app theme (light/dark/auto)


        // Public properties (Data members in the json file)
        [DataMember(Name = "homePage")]
        public string HomePage { get; set; }                       // Home page URL
        [DataMember(Name = "newTabPage")]
        public string NewTabPage { get; set; }                     // Page that opens a new tab
        [DataMember(Name = "theme")]
        public string ThemeString { get; set; }                    // Theme string representation

        public Theme Theme
        {
            get
            {
                switch (this.ThemeString)
                {
                    case "dark":
                        return Theme.Dark;
                    case "light":
                        return Theme.Light;
                    default:
                        return Theme.Auto;
                }
            }
            set
            {
                switch (value)
                {
                    case Theme.Dark:
                        this.ThemeString = "dark";
                        break;
                    case Theme.Light:
                        this.ThemeString = "light";
                        break;
                    default:
                        this.ThemeString = "auto";
                        break;
                }
            }
        }                           

        // Private constructor
        private UserSettings() { }

        /// <summary>
        /// Saves the settings to the json file
        /// </summary>
        public void Save()
        {
            Stream stream = new StreamWriter(AppData.SettingsPath).BaseStream;
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
                if (File.Exists(AppData.SettingsPath))
                {
                    var stream = new StreamReader(AppData.SettingsPath).BaseStream;
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
                        NewTabPage = DefaultNewTabPage,
                        ThemeString = DefaultTheme                       
                    };
                    result.Save();  // Create a new json file with default values
                    Instance = result;
                }
            }
            return Instance;
        }
    }
}
