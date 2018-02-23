namespace PostboxAPI
{
    /// <summary>
    /// Request template for Output Creators, that holds information of the call
    /// </summary>
    public class PostboxRequest : PostboxCall
    {
        /// <summary>
        /// Parameters for the api call
        /// </summary>
        public PostboxCallParameter[] Parameters;

        /// <summary>
        /// Constuct a API Request
        /// </summary>
        /// <param name="appId">AppID of the connector class</param>
        /// <param name="deviceId">DeviceID of the connector class</param>
        /// <param name="liceneKey">LiceneKey of the connector class</param>
        /// <param name="apiVersion">API-Version of the connector class</param>
        /// <param name="userName">UserName of the connector class</param>
        /// <param name="userPassword">UserPassword of the connector class</param>
        /// <param name="parameters">Additional Parameters</param>
        public PostboxRequest(string appId, string deviceId,
                                 string liceneKey = "", string apiVersion = "1.0",
                                 string userName = "", string userPassword = "", params PostboxCallParameter[] parameters) :
                              base(appId, deviceId, liceneKey, apiVersion, userName, userPassword, parameters)
        {
            ApplyParameters(parameters);
        }
        
        /// <summary>
        /// Apply additional Parameters to the Request
        /// </summary>
        /// <param name="parameters"></param>
        public void ApplyParameters(params PostboxCallParameter[] parameters)
        {
            Parameters = parameters;
        }
    }
}