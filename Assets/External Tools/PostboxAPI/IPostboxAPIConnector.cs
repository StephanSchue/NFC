using System.Collections;

namespace PostboxAPI
{
    /// <summary>
    /// Interface used for create an new Connector using the API
    /// </summary>
    interface IPostboxAPIConnector
    {
        /// <summary>
        /// General function for send an Call to the API
        /// </summary>
        /// <typeparam name="TRequest">Type of PostboxResponse child class</typeparam>
        /// <param name="callName">Callname of the PostboxService</param>
        /// <param name="localRequestId">Identifier for package managment</param>
        /// <param name="userCallback">Callback method of the user</param>
        /// <param name="callback">Internal Callback, after Response is set</param>
        /// <param name="timeout">Max call lifetime in milliseconds</param>
        /// <param name="parameter">Parameters for the call</param>
        IEnumerator ProcessRequest<TRequest>(PostboxCallName callName,
                                    string localRequestId,
                                    PostboxResponseCallback<TRequest> userCallback, 
                                    PostboxDelayCallback<TRequest> callback, 
                                    int timeout = 1000,
                                    params PostboxCallParameter[] parameter) where TRequest : PostboxResponse;


        /// <summary>
        /// A General Method to implement Interval Requests
        /// </summary>
        /// <param name="callName">Callname of the PostboxService</param>
        /// <param name="callback">Child class of PostboxResponse</param>
        /// <param name="interval">loop interval in seconds. Set 0 for now loop.</param>
        /// <param name="timeout">Time in seconds for send a new status request</param>
        /// <param name="parameter">Parameters for the call</param>
        IEnumerator ProcessIntervalRequest<TRequest>(PostboxCallName callName, 
                                                PostboxResponseCallback<TRequest> callback, 
                                                int interval, 
                                                int timeout, 
                                                params PostboxCallParameter[] parameter) where TRequest : PostboxResponse;

        #region ------------ ServerTime ------------

        /// <summary>
        /// Requests the current DateTime of the Server
        /// </summary>
        /// <param name="callback">Callback method of the user</param>
        void RequestServerSystemTime(PostboxResponseCallback<PostboxServerTimeResponse> callback);

        #endregion

        #region ------------ Data Packages ------------

        /// <summary>
        /// Sends an package for an other device to the server
        /// </summary>
        /// <param name="receiverDeviceId">DeviceId of the receiver</param>
        /// <param name="data">Data for the other device</param>
        /// <param name="callback">Callback method of the user</param>
        /// <param name="receiverAppId">AppId of receiver, to send packages accross apps</param>
        /// <param name="timeout">Max call lifetime in milliseconds</param>
        /// <param name="parameters">Additional parameters for the call</param>
        void SendDataPackageToDevice(string receiverDeviceId, string data, PostboxResponseCallback<PostboxSendDataPackageToDeviceResponse> callback, string receiverAppId = "", int timeout = 1000, params PostboxCallParameter[] parameters);

        /// <summary>
        /// Update the status of an defined package of the user
        /// </summary>
        /// <param name="packageTransactionId">TransactionId of the package that will be affeted</param>
        /// <param name="status">New package status</param>
        /// <param name="callback">Callback method of the user</param>
        /// <param name="message">Additional notification for staus change</param>
        /// <param name="timeout">Max call lifetime in milliseconds</param>
        /// <param name="parameters">Additional parameters for the call</param>
        void UpdateDataPackageStatus(string packageTransactionId, PostboxDataPackageStatus status, PostboxResponseCallback<PostboxUpdateDataPackageResponse> callback, string message = "", int timeout = 1000, params PostboxCallParameter[] parameters);

        /// <summary>
        /// Request new Packages for the user
        /// </summary>
        /// <param name="userCallback">Callback method of the user</param>
        /// <param name="interval">loop interval in seconds. Set 0 for now loop.</param>
        /// <param name="filterByStatus">Activate filter for PostboxDataPackageStatus</param>
        /// <param name="status">Set PostboxDataPackageStatus for filtering. You can filter multible status by using the bit-operator & to combine the status</param>
        /// <param name="fromDevice">Filter by specific device</param>
        /// <param name="fromApp">Filter by specific app</param>
        /// <param name="timeout">Max call lifetime in milliseconds</param>
        /// <returns>Return the ID of the Process that needed for stop it manually.</returns>
        string GetDataPackages(PostboxResponseCallback<PostboxGetDataPackagesResponse> userCallback, int interval = 0, bool filterByStatus = true, PostboxDataPackageStatus status = PostboxDataPackageStatus.open, string fromDevice = "", string fromApp = "", int timeout = 1000);

        /// <summary>
        /// Request new Packages for the user
        /// </summary>
        /// <param name="userCallback">Callback method of the user</param>
        /// <param name="interval">loop interval in seconds. Set 0 for now loop.</param>
        /// <param name="filterByStatus">Activate filter for PostboxDataPackageStatus</param>
        /// <param name="status">Set PostboxDataPackageStatus for filtering. You can filter multible status by using the bit-operator & to combine the status</param>
        /// <param name="fromDevices">Filter by specific devices</param>
        /// <param name="fromApps">Filter by specific apps</param>
        /// <param name="timeout">Max call lifetime in milliseconds</param>
        /// <returns>Return the ID of the Process that needed for stop it manually.</returns>
        string GetDataPackagesFiltered(PostboxResponseCallback<PostboxGetDataPackagesResponse> userCallback, int interval = 0, bool filterByStatus = true, PostboxDataPackageStatus status = PostboxDataPackageStatus.open, string[] fromDevices = null, string[] fromApps = null, int timeout = 1000);

        /// <summary>
        /// Request the current status of the defined package
        /// </summary>
        /// <param name="packageTransactionId">TransactionId of the package that will be affeted</param>
        /// <param name="callback">Callback method of the user</param>
        /// <param name="interval"></param>
        /// <param name="timeout">Max call lifetime in milliseconds</param>
        /// <returns>Return the ID of the Process that needed for stop it manually.</returns>
        string GetDataPackageStatus(string packageTransactionId, PostboxResponseCallback<PostboxGetDataPackageStatusResponse> callback, int interval=0, int timeout = 1000);


        #endregion

        #region  ------------ App ------------

        /// <summary>
        /// Validate the given AppID on the server
        /// </summary>
        /// <param name="appId">AppID of the given App</param>
        /// <param name="callback">Callback method of the user</param>
        /// <param name="timeout">Max call lifetime in milliseconds</param>
        void CheckAppId(string appId, PostboxResponseCallback<PostboxCheckAppIDResponse> callback, int timeout = 1000);

        #endregion

        #region  ------------ Device ------------

        /// <summary>
        /// Validate the given Device on the server
        /// </summary>
        /// <param name="deviceId">DeviceID of the given Device</param>
        /// <param name="callback">Callback method of the user</param>
        /// <param name="timeout">Max call lifetime in milliseconds</param>
        void CheckDeviceId(string deviceId, PostboxResponseCallback<PostboxCheckDeviceIdResponse> callback, int timeout = 1000);

        /// <summary>
        /// Register an new device at the API and inform the user about the success of the operation
        /// </summary>
        /// <param name="callback">Callback method of the user</param>
        /// <param name="timeout">Max call lifetime in milliseconds</param>
        void RegisterDevice(PostboxResponseCallback<PostboxRegisterDeviceResponse> callback, int timeout = 1000);

        #endregion

        #region  ------------ Friendlist ------------

        //string AddFriendToList(string friendDeviceId, PostboxResponseCallback<PostboxAddToFriendlistResponse> callback);
        //string RemoveFriendFromList(string friendDeviceId, PostboxResponseCallback<PostboxRemoveFromFriendlistResponse> callback);
        //string GetFriendlist(PostboxResponseCallback<PostboxGetFriendlistResponse> callback);

        #endregion

        #region  ------------ Ping ------------

        /// <summary>
        /// Ping an Device of an other player.
        /// The Method inform the callback about the current status of the ping.
        /// Pingstatus:
        /// - "sever" = the server receive the request
        /// - "device" = the other device response
        /// </summary>
        /// <param name="deviceId">DeviceID of the other Device</param>
        /// <param name="callback">Callback method of the user</param>
        /// <param name="timeout">Max call lifetime in milliseconds</param>
        void PingDevice(string deviceId, PostboxResponseCallback<PostboxPingDeviceResponse> callback, int timeout = 30000);

        /// <summary>
        /// Response that the device is alive for the api
        /// </summary>
        /// <param name="callback">Child class of PostboxResponse</param>
        /// <param name="interval">loop interval in seconds. Set 0 for now loop.</param>
        /// <param name="timeout">Time in seconds for send a new status request</param>
        /// <returns>Return the ID of the Process that needed for stop it manually.</returns>
        string PingResponse(PostboxResponseCallback<PostboxResponse> callback, int interval = 0, int timeout = 1000);

        #endregion
    }
}

