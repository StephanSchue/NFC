using System;
using System.Collections.Generic;

namespace PostboxAPI
{
    /// <summary>
    /// This is an C#-Logging class used for collect and write notifications and error of the API & this Plugin
    /// </summary>
    public class PostboxLogbook
    {
        // --- Singelton initialize ---
        private static PostboxLogbook instance;
        private PostboxLogbook() { }

        /// <summary>
        /// Singelton of Logbook
        /// </summary>
        public static PostboxLogbook Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new PostboxLogbook("", "", "");
                }
                return instance;
            }
        }

        // --- NotificationLevel ---

        /// <summary>
        /// Type of log message
        /// </summary>
        public enum NotificationType
        {
            Notification,
            Warning,
            Error,
            APICalls
        }

        // --- Logging Variables ---

        /// <summary>
        /// Path where logfiles where saved in filesystem
        /// </summary>
        public string filesystemPath { get; private set; }

        /// <summary>
        /// Type of message <see cref="PostboxLogbook.NotificationType" />.
        /// </summary>
        public NotificationType NotificationLevel { get; internal set; }

        private string appId = "";
        private string deviceId = "";
        private bool useFileSystem = false;

        private string session_identifier = "";

        private Queue<PostboxLogMessage> unreadedMessages = new Queue<PostboxLogMessage>();

        #region Initialize

        /// <summary>
        /// Initialize the logging class.
        /// Checking the logFile direction and create an session identifier
        /// </summary>
        /// <param name="appId">API App idenfifier</param>
        /// <param name="deviceId">DeviceID, generated at the first start of the App on an device</param>
        /// <param name="path">Absolute path for logFiles</param>
        public PostboxLogbook(string appId, string deviceId, string path = "")
        {
            // Set userInfos
            SetUserInformation(appId, deviceId, useFileSystem);

            // Set SessionIdentifier
            DateTime date = DateTime.Now;
            session_identifier = date.Millisecond.ToString();

            // Check Path
            SetPath(path);
        }

        /// <summary>
        /// Set Information of current App and Device that can be used by the logging script
        /// </summary>
        /// <param name="_appId">API App idenfifier</param>
        /// <param name="_deviceId">DeviceID, generated at the first start of the App on an device</param>
        public void SetUserInformation(string _appId, string _deviceId, bool saveInFileSystem)
        {
            appId = _appId;
            deviceId = _deviceId;
            useFileSystem = saveInFileSystem;
        }

        /// <summary>
        /// Path where the Logfiles should be saved. (think of plaform independency)
        /// </summary>
        /// <param name="path">Absolute path for logFiles</param>
        public bool SetPath(string path)
        {
            bool status = false;

            if(!useFileSystem)
                return false;

            // Check Parameter
            if (!System.String.IsNullOrEmpty(path))
            {
                filesystemPath = path;
            }
            else
            {
                //filesystemPath = "C:\\Users\\Public\\PostBoxAPI\\";
            }

            status = CheckDirectory(filesystemPath);

            return status;
        }

        /// <summary>
        /// Check if path exists. If there is no directory, the directory will be created.
        /// </summary>
        /// <param name="path">Absolute path for logFiles</param>
        /// <returns></returns>
        public bool CheckDirectory(string path)
        {
            bool status = false;

            if(!useFileSystem)
                return false;

            if(string.IsNullOrEmpty(path))
                return false;

            // Check Directory
            if(System.IO.Directory.Exists(filesystemPath))
            {
                status = true;
            }
            else
            {
                if (System.IO.Directory.CreateDirectory(filesystemPath) != null)
                {
                    status = true;
                }
                else
                {
                    status = false;
                }
            }

            return status;
        }

        #endregion

        #region Logging Methods

        /// <summary>
        /// Logging an Message
        /// </summary>
        /// <param name="text">Notification Text</param>
        /// <param name="level">Level of Notification (Notification, Warning, Error, APICalls)</param>
        public void Log(string text, NotificationType level)
        {
            if(!useFileSystem)
                return;

            DateTime date = DateTime.Now;

            if(CheckDirectory(filesystemPath))
            {
                using (System.IO.StreamWriter file =
                    new System.IO.StreamWriter(@filesystemPath + String.Format("logfile-{0}-{1}-{2}-{3}-{4}.txt", appId, session_identifier, date.Year, date.Month, date.Day), true))
                {
                    PostboxLogMessage LogMessage = new PostboxLogMessage(text, date, level);
                    unreadedMessages.Enqueue(LogMessage);
                    file.WriteLine(String.Format("{0} - [{1}] {2}", LogMessage.Time, LogMessage.NotificationLevel, LogMessage.Message));
                }
            }
        }

        /// <summary>
        /// Logging an APIError
        /// </summary>
        /// <param name="error">API Error Object of Response.Errors[]</param>
        /// <param name="level">Level of Notification (Notification, Warning, Error, APICalls)</param>
        public void Log(PostboxAPIError error, NotificationType level = NotificationType.Error)
        {
            this.Log(String.Format("[API-Error] {0} - {1}: {2}", error.ErrorCode, error.ErrorDescription, error.ErrorLongDescription), NotificationType.Error);
        }

        #endregion

        #region Methods of MessageQueue for external LogOutput

        /// <summary>
        /// Returns the last Message of the saved message queue
        /// </summary>
        /// <returns>PostboxLogMessage object or NULL if empty</returns>
        public PostboxLogMessage GetMessage()
        {
            PostboxLogMessage output = null;

            if (unreadedMessages.Count > 0)
            {
                output = unreadedMessages.Dequeue();
            }

            return output;
        }

        #endregion
    }
}