using System.Collections.Generic;

namespace Organic_Browser.Utils
{
    /// <summary>
    /// This class represents a Chromium Embedded Browser initializer
    /// </summary>
    class CefInitializer
    {
        // Private attributes
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

        public static void Initialize()
        {
            // Add the settings to the CefSettings object
            var settings = new CefSharp.Wpf.CefSettings();
            foreach (KeyValuePair<string, string> command in Commands)
            {
                settings.CefCommandLineArgs.Add(command);
            }


            // Initialize Cef
            CefSharp.Cef.Initialize(settings);
        }
    }
}
