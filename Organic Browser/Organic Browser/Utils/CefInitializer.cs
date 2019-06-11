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
        private const string CachePath = ".cache";                      // Path to the cache directory
        private static readonly Dictionary<string, string> Commands;    // Cef commands

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
            settings.CachePath = OrganicUtility.GetAbsolutePath(CachePath);

            // Initialize Cef
            CefSharp.Cef.Initialize(settings);

            settings.Dispose();
        }

        /// <summary>
        /// Makes sure that the cachefolder exists,
        /// if does not exist, creates one.
        /// </summary>
        private static void ValidateCacheFolder()
        {
            string absCachePath = OrganicUtility.GetAbsolutePath(CachePath);

            // In case history file does not exist, it means that someone deleted the hostory, therefore
            // the cached data should be deleted as well.
            if (!File.Exists(OrganicUtility.GetAbsolutePath(AppData.HistoryPath)) && File.Exists(absCachePath))
                Directory.Delete(absCachePath, recursive: true);

            if (!Directory.Exists(absCachePath))
                Directory.CreateDirectory(absCachePath);
        }
    }
}
