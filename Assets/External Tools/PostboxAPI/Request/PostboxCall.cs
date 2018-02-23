namespace PostboxAPI
{
    /// <summary>
    /// Baseclass for PostboxRequest, that holds information of the call
    /// </summary>
    public class PostboxCall
    {
        //<Request>
        // <PostboxGlobal>
        //  <AppID>HVIIM2VWQJW6YUSF</AppID>
        //  <Version>1.0</Version>
        //  <UserName />
        //  <UserPassword />
        //  <LicenseKey />
        //  <DeviceID>b81de5fd947cb792f7213f2f8cff234d5f2291a0</DeviceID>
        //  <CallName>GetDataPackages</CallName>
        // </PostboxGlobal>
        // <Param>
        //  <Status>open</Status>
        // </Param>
        //</Request>

        /// <summary>
        /// App identifier
        /// </summary>
        public string AppID { get; private set; }

        /// <summary>
        /// Version of the api
        /// </summary>
        public string Version { get; private set; }

        /// <summary>
        /// Username
        /// </summary>
        public string UserName { get; private set; }

        /// <summary>
        /// Crypeted userpassword
        /// </summary>
        public string UserPassword { get; private set; }

        /// <summary>
        /// The used licence key
        /// </summary>
        public string LicenseKey { get; private set; }

        /// <summary>
        /// The device identifier
        /// </summary>
        public string DeviceID { get; private set; }

        /// <summary>
        /// Call identifier of the request
        /// </summary>
        public string CallName { get; private set; }


        /// <summary>
        /// Constuct a API Call
        /// </summary>
        /// <param name="appId">AppID of the connector class</param>
        /// <param name="deviceId">DeviceID of the connector class</param>
        /// <param name="liceneKey">LiceneKey of the connector class</param>
        /// <param name="apiVersion">API-Version of the connector class</param>
        /// <param name="userName">UserName of the connector class</param>
        /// <param name="userPassword">UserPassword of the connector class</param>
        /// <param name="parameters">Additional Parameters</param>
        public PostboxCall(string appId, string deviceId,
                                 string liceneKey = "", string apiVersion = "1.0",
                                 string userName = "", string userPassword = "", params PostboxCallParameter[] parameters)
        {
            AppID = appId;
            Version = apiVersion;
            DeviceID = deviceId;
            LicenseKey = liceneKey;
            UserName = userName;
            UserPassword = userPassword;
        }


    }
}