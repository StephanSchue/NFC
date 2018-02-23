namespace PostboxAPI
{
    /// <summary>
    /// The SaveFile for UserSettings of the PostboxConnector
    /// </summary>
    public class PostboxPlayerConfig
    {
        /// <summary>
        /// Device identifier
        /// </summary>
        public string DeviceId { get; private set; }

        /// <summary>
        /// Username
        /// </summary>
        public string UserName { get; private set; }

        /// <summary>
        /// Crypeted userpassword
        /// </summary>
        public string UserPassword { get; private set; }

        /// <summary>
        /// Constuctor of SaveFile
        /// </summary>
        /// <param name="deviceId">Unique Device Id set by API</param>
        /// <param name="userName">Username</param>
        /// <param name="userPassword">Userpassword</param>
        public PostboxPlayerConfig(string deviceId, string userName, string userPassword)
        {
            DeviceId = deviceId;
            UserName = userName;
            UserPassword = userPassword;
        }

        /// <summary>
        /// Validate the UserSettings
        /// </summary>
        /// <returns>true = if all data correct; false = if data wrong</returns>
        public bool Check()
        {
            bool status = true;

            if (DeviceId == "")
            {
                PostboxLogbook.Instance.Log("DeviceId not set. Please request the userdata again.", PostboxLogbook.NotificationType.Error);
                status = false;
            }

            if (UserName == "")
            {
                PostboxLogbook.Instance.Log("Username not set. Some functions need the userdata for calling.", PostboxLogbook.NotificationType.Warning);
            }

            if (UserPassword == "")
            {
                PostboxLogbook.Instance.Log("Userpassword not set. Some functions need the userdata for calling.", PostboxLogbook.NotificationType.Warning);
            }

            return status;
        }
    }
}