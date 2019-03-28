using System.Collections.Generic;
using System.IO;

namespace Organic_Browser.Utils
{
    /// <summary>
    /// This class represents a Chromium Embedded Browser initializer
    /// </summary>
    class CefInitializer
    {
        // Private attributes
        private const string CachePath = ".cache";
        private static readonly Dictionary<string, string> Commands;

        static CefInitializer()
        {
            // All the commands for the CefSettingsCommandLine
            Commands = new Dictionary<string, string>
            {
                // Improve performence on high resolutions
                {"disable-gpu-vsync", "1"},     
                {"disable-gpu", "1"},
            };
        }

        /// <summary>
        /// Initializes CEF
        /// </summary>
        public static void Initialize()
        {
            // Add the settings to the CefSettings object
            var settings = new CefSharp.Wpf.CefSettings();
            foreach (KeyValuePair<string, string> command in Commands)
            {
                settings.CefCommandLineArgs.Add(command);
            }

            // Set the cache path in the CefSettings object
            ValidateCacheFolder();              // Make sure that the cache folder exists
            settings.CachePath = CachePath;
            

            // Initialize Cef
            CefSharp.Cef.Initialize(settings);
        }

        /// <summary>
        /// Makes sure that the cachefolder exists,
        /// if does not exist, creates one.
        /// </summary>
        private static void ValidateCacheFolder()
        {
            if (!Directory.Exists(CachePath))
                Directory.CreateDirectory(CachePath);
        }
    }
}
