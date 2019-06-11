using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Organic_Browser.Utils
{
    /// <summary>
    /// This class represents the application data saved in device disk.
    /// </summary>
    class AppData
    {
        // Static read only attributes
        public static readonly string ExeFileLocation = Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location);  // Path to the browser's exe file containing folder
        public static readonly string AppDataLocation = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Organic Browser");
        public static readonly string SettingsPath = Path.Combine(ExeFileLocation, "settings.json");                                    // Settings file path
        public static readonly string HistoryPath = Path.Combine(ExeFileLocation, "pages\\history\\history.json");                      // History file path
        public static readonly string DownloadedPagesLocation = Path.Combine(AppDataLocation, "saved_pages");                           // Location for downloaded pages

        /// <summary>
        /// Validates that the application data is valid, if invalid, creates new empty folders
        /// </summary>
        public static void Validate()
        {
            if (!Directory.Exists(AppDataLocation)) Directory.CreateDirectory(AppDataLocation);
            if (!Directory.Exists(DownloadedPagesLocation)) Directory.CreateDirectory(DownloadedPagesLocation);
        }
    }
}
