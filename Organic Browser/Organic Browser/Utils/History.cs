using System;
using System.IO;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;

namespace Organic_Browser.Utils
{
    /// <summary>
    /// Contains all the user's browsing history
    /// 
    /// (Implements Singleton design pattern)
    /// </summary>
    class History
    {
        // Single instance
        private static History Instance = null;

        // public read only data
        public const string HistoryPath = "history.json";      // Path of the history json file 

        /// <summary>
        /// Single url visit
        /// </summary>
        [DataContract]
        private class UrlVisit
        {
            [DataMember(Name = "date")]
            public string Date { get; set; }    // The date of the visit
            [DataMember(Name = "time")]
            public string Time { get; set; }    // The hour of the visit
            [DataMember(Name = "url")]
            public string Url { get; set; }     // The url that the user visited
        }

        private List<UrlVisit> UrlVisits { get; set; }

        /// <summary>
        /// Private constructor - for singleton design pattern
        /// </summary>
        private History() { }

        /// <summary>
        /// Returns the single instance
        /// </summary>
        /// <returns>a single History instance</returns>
        public static History Load()
        {
            if (History.Instance != null)
                return History.Instance;

            History instance = new History();
            // In case the history file exists
            if (File.Exists(HistoryPath))
            {
                Stream stream = new StreamReader(HistoryPath).BaseStream;
                try
                {
                    var serializer = new DataContractJsonSerializer(typeof(List<UrlVisit>));
                    instance.UrlVisits = (List<UrlVisit>)serializer.ReadObject(stream);
                    History.Instance = instance;
                }
                finally
                {
                    stream.Dispose();
                }
            }
            // In case the history file does not exist
            else
            {
                instance.UrlVisits = new List<UrlVisit>();
                instance.Save();

                History.Instance = instance;
            }

            return History.Instance;
        }

        /// <summary>
        /// Clear all the history that is saved 
        /// </summary>
        public void ClearAllHistory()
        {
            File.Delete(HistoryPath);   // Delete the history file, a new one will be created when it will be needed
        }

        /// <summary>
        /// Add a url to the history file
        /// </summary>
        /// <param name="url">Url the user visited</param>
        public void AddUrlVisit(string url)
        {
            UrlVisit visit = new UrlVisit()
            {
                Url = url,
                Time = this.GetCurrentTime(),
                Date = this.GetTodayDate()
            };
            this.UrlVisits.Add(visit);
            this.Save();
        }

        #region Private Functions

        /// <summary>
        /// Save the history by writing it to a json file.
        /// </summary>
        private void Save()
        {
            Stream stream = null;
            try
            {
                stream = new StreamWriter(HistoryPath).BaseStream;
                var serializer = new DataContractJsonSerializer(this.UrlVisits.GetType());
                serializer.WriteObject(stream, this.UrlVisits);
            }
            finally
            {
                stream.Dispose();
            }
        }

        /// <summary>
        /// Returns the current time string in this format -> 1:04
        /// </summary>
        /// <returns></returns>
        private string GetCurrentTime()
        {
            DateTime now = DateTime.Now;
            int hour = now.Hour, minute = now.Minute;
            string hourString = hour.ToString();
            string minuteString = minute.ToString();
            if (minute < 10)
                minuteString = "0" + minuteString;
            return string.Format("{0}:{1}", hourString, minuteString);
        }

        /// <summary>
        /// Returns today's date
        /// </summary>
        /// <returns></returns>
        private string GetTodayDate()
        {
            DateTime now = DateTime.Now;
            return string.Format("{0}.{1}.{2}", now.Month, now.Day, now.Year);
        }
        #endregion

    }
}
