using UnityEngine;
using System;
using System.Xml;
using System.Collections;
using System.Collections.Generic;

namespace PostboxAPI
{
    /// <summary>
    /// This class provide the main functionality to communicate with the API
    /// </summary>
    public class PostboxAPIUnityConnector : PostboxSingleton<PostboxAPIUnityConnector>, IPostboxAPIConnector
    {
        protected PostboxAPIUnityConnector() { }

        #region Variables & Settings

        // --- API-Settings ---

        /// <summary>
        /// URL of the server
        /// </summary>
        public string url = "http://pba.stephan-schueritz.de/";

        /// <summary>
        /// Start API on Awake, to Start manually use BootAPI()
        /// </summary>
        public bool StartOnAwake = true;

        [Header("You get this ID from the WebLogin:")]
        /// <summary>
        /// App Identifier
        /// </summary>
        public string appId = "";

        private string apiVersion = "1.0";

        [Header("Data for extended API connection")]

        /// <summary>
        /// Username
        /// </summary>
        public string userName = "";

        /// <summary>
        /// Userpassword
        /// </summary>
        public string userPassword = "";

        /// <summary>
        /// LicenceKey of user
        /// </summary>
        public string LiceneKey = "";

        // --- Call communication type ---
        /// <summary>
        /// Type of Communcation for Requests and Response
        /// </summary>
        public PostboxCommunicationType communicationType = PostboxCommunicationType.xml;

        // --- Package managment ----
        private static IDictionary<string, PostboxResponse> PostboxResponses = new Dictionary<string, PostboxResponse>();
        private static IDictionary<string, Coroutine> PostboxCoroutines = new Dictionary<string, Coroutine>();

        // --- Helper class ---
        private WWWForm connection;
        private PostboxXMLCreator xmlCreator;
        private PostboxJSONCreator jsonCreator;

        /// <summary>
        /// Postbox UserSettings
        /// </summary>
        private PostboxPlayerConfig UserSettings;

        // --- Register Call Variables ---
        private int maxRequestTimeout = 10; // TODO: The Calls that used until the API will stopped
        private int curRequestTimeout = 0;

        // --- Interval Settings ---
        [Header("Auto PingResponse Settings")]
        /// <summary>
        /// Bool for AutoPing
        /// </summary>
        public bool AutoPingResponse = true;
        
        /// <summary>
        /// Interval of AutoPing
        /// </summary>
        public int AutoPingResponseInterval = 5;
        private string autoPingProcessId = "";

        private const int DefaultTimeoutValue = 1000;

        // --- Plugin Status ---
        /// <summary>
        /// Return true, if the app is block by the API
        /// </summary>
        public bool BlockedByAPI { get; private set; }

        private bool apiReady = false;

        /// <summary>
        /// Return true, if the API is ready for usage. (require an valid appID and deviceID check)
        /// </summary>
        public bool APIReady
        {
            get
            {
                return apiReady;
            }
            private set
            {
                apiReady = value;

                if(apiReady)
                {
                    // Start PingResponse
                    if (AutoPingResponse)
                    {
                        StartAutoPingResponse();
                    }
                }
            }
        }

        #endregion

        #region Initialziation

        /// <summary>
        /// Use this for initialize singelton and helper classes
        /// </summary>
        private void Awake()
        {
            if(StartOnAwake)
            {
                BootAPI();
            }
        }

        /// <summary>
        /// Start the API Connection
        /// </summary>
        public void BootAPI()
        {
            // Set States
            BlockedByAPI = false;
            APIReady = false;

            // Init Logging Class
            bool saveInFileSystem = Application.isEditor;
            Debug.Log("Write log in Filesystem " + saveInFileSystem);
            PostboxLogbook.Instance.SetUserInformation(this.appId, "", saveInFileSystem);

            if(saveInFileSystem)
            { 
                string path = Application.dataPath + "/PostboxLog/";

                if (Application.isEditor)
                {
                    path = Application.temporaryCachePath + "/PostboxLogEditor/";
                }

                if (string.IsNullOrEmpty(path))
                {
                    Debug.Log("Logfile path is not set!");
                }
                else
                {
                    Debug.Log("Logfile will saved at: " + path);
                }


                if (!PostboxLogbook.Instance.SetPath(path))
                {
                    Debug.LogError("LogFiles path couldn't be created!");
                }
            }

            // Check APPID
            if (System.String.IsNullOrEmpty(appId))
            {
                // AppID is Emtpy
                Debug.LogError("AppID is not set. Please insert an AppID in the component settings.");
            }
            else
            {
                PostboxLogbook.Instance.Log("--- Initalize Postbox API ---", PostboxLogbook.NotificationType.Notification);

                // HTTP-Call helper class
                connection = new WWWForm();
                UserSettings = new PostboxPlayerConfig("", "", "");

                // XML-Helper
                xmlCreator = new PostboxXMLCreator(this.appId, UserSettings.DeviceId,
                                                    this.LiceneKey, this.apiVersion,
                                                    UserSettings.UserName, UserSettings.UserPassword);

                jsonCreator = new PostboxJSONCreator(this.appId, UserSettings.DeviceId,
                                                    this.LiceneKey, this.apiVersion,
                                                    UserSettings.UserName, UserSettings.UserPassword);

                // Load UserSettings
                LoadUserSettings();

                // Check if device has registerd on API
                CheckAppId(this.appId, this.GetAppIdCallback);
            }
        }

        #endregion

        #region Callbacks used for Initialziation

        /// <summary>
        /// Method used for initalize check of AppID.
        /// Calling DeviceIdCheck / Registration of the Device if response is sucessful
        /// </summary>
        /// <param name="response">PostboxCheckAppIDResponse</param>
        private void GetAppIdCallback(PostboxCheckAppIDResponse response)
        {
            // --- Check Response ---
            PostboxAPIUnityConnector.Instance.RemoveResponse(response.LocalRequestId);

            switch (response.CallStatus)
            {
                case PostboxCallStatus.Success:
                    if (response.Status)
                    {
                        PostboxLogbook.Instance.Log(String.Format("Successful found app on API."), PostboxLogbook.NotificationType.Notification);

                        // --- Check DeviceID and do Action ---
                        if(System.String.IsNullOrEmpty(this.UserSettings.DeviceId))
                        {
                            // If DeviceId not set - register device
                            PostboxLogbook.Instance.Log(String.Format("DeviceId not set. Do Register."), PostboxLogbook.NotificationType.Notification);
                            RegisterDevice(this.RegisterDeviceIdCallback);
                        }
                        else
                        {
                            // If DeviceId is set - check id
                            PostboxLogbook.Instance.Log(String.Format("Now checking device connect on API."), PostboxLogbook.NotificationType.Notification);
                            CheckDeviceId(this.UserSettings.DeviceId, this.CheckDeviceIdCallback);
                        }
                    }
                    else
                    {
                        PostboxLogbook.Instance.Log(String.Format("APP could not be found on API."), PostboxLogbook.NotificationType.Error);
                    }
                    break;
                case PostboxCallStatus.Error:
                    response.LogAllAPIErrors();
                    break;
                default:
                    ++curRequestTimeout;
                    break;
            }
        }

        /// <summary>
        /// Method used for initialize check of the DeviceID.
        /// Calling DeviceID Registration if it is not set.
        /// </summary>
        /// <param name="response">PostboxCheckDeviceIdResponse</param>
        private void CheckDeviceIdCallback(PostboxCheckDeviceIdResponse response)
        {
            // --- Check Device ---
            PostboxAPIUnityConnector.Instance.RemoveResponse(response.LocalRequestId);

            switch (response.CallStatus)
            {
                case PostboxCallStatus.Success:

                    // --- Check Device Exists ---
                    if (response.Status)
                    {
                        PostboxLogbook.Instance.Log(String.Format("Successful found device on API."), PostboxLogbook.NotificationType.Notification);
                        PostboxLogbook.Instance.Log("PostboxAPI: Ready for usage.", PostboxLogbook.NotificationType.Notification);
                        APIReady = true;
                    }
                    else
                    {
                        PostboxLogbook.Instance.Log(String.Format("Nothing found. Do Register."), PostboxLogbook.NotificationType.Notification);
                        RegisterDevice(this.RegisterDeviceIdCallback);
                        APIReady = false;
                    }
                    break;
                case PostboxCallStatus.Error:
                    response.LogAllAPIErrors();
                    break;
                default:
                    ++curRequestTimeout;
                    break;
            }
        }

        /// <summary>
        /// Method used for initalize registration of an device.
        /// Update the UserSettings with the new DeviceID
        /// </summary>
        /// <param name="response">PostboxRegisterDeviceResponse</param>
        private void RegisterDeviceIdCallback(PostboxRegisterDeviceResponse response)
        {
            // -- Check if Registration is successful --
            PostboxAPIUnityConnector.Instance.RemoveResponse(response.LocalRequestId);

            switch (response.CallStatus)
            {
                case PostboxCallStatus.Success:
                    // -- Save Usersettings --
                    PostboxLogbook.Instance.Log("Successful registerd new device.", PostboxLogbook.NotificationType.Notification);
                    SetDeviceId(response.DeviceId);
                    SaveUserSettings(UserSettings);

                    PostboxLogbook.Instance.Log("Ready for usage.", PostboxLogbook.NotificationType.Notification);
                    APIReady = true;
                    break;
                case PostboxCallStatus.Fail:
                    PostboxLogbook.Instance.Log(String.Format("{0}: {1}", response.NotificationCode, response.NotificationDescription), PostboxLogbook.NotificationType.Warning);

                    // -- Save Usersettings --
                    if (response.NotificationCode == "Exists")
                    {
                        PostboxLogbook.Instance.Log("Resave DeviceId in SaveFile", PostboxLogbook.NotificationType.Notification);
                        SetDeviceId(response.DeviceId);
                        SaveUserSettings(UserSettings);

                        PostboxLogbook.Instance.Log("Ready for usage.", PostboxLogbook.NotificationType.Notification);
                        APIReady = true;
                    }
                    break;
                default:
                    // -- Some Error --
                    PostboxLogbook.Instance.Log(String.Format("{0}: {1}", response.NotificationCode, response.NotificationDescription), PostboxLogbook.NotificationType.Warning);
                    ++curRequestTimeout;
                    break;
            }
        }

        #endregion 

        #region GetDeviceID

        /// <summary>
        /// Method to get the deviceId of the current device
        /// </summary>
        /// <param name="userCallback">Callback method of the user</param>
        public void GetDeviceID(PostboxDeviceIDCallback userCallback)
        {
            StartCoroutine(ProcessDeviceID(userCallback));
        }

        /// <summary>
        /// Method that waiting as long the deviceID is not set
        /// </summary>
        /// <param name="userCallback">Callback method of the user</param>
        private IEnumerator ProcessDeviceID(PostboxDeviceIDCallback userCallback)
        {
            Boolean status = true;

            while (status)
            {
                if (this.UserSettings == null)
                    yield return new WaitForEndOfFrame();

                if (!String.IsNullOrEmpty(this.UserSettings.DeviceId))
                {
                    userCallback(this.UserSettings.DeviceId);
                    status = false;
                }

                yield return new WaitForEndOfFrame();
            }
        }

        #endregion

        #region Core methods for server calls

        /// <summary>
        /// General function for send an Call to the API
        /// </summary>
        /// <typeparam name="T">Type of PostboxResponse child class</typeparam>
        /// <param name="callName">Callname of the PostboxService</param>
        /// <param name="localRequestId">Identifier for package managment</param>
        /// <param name="userCallback">Callback method of the user</param>
        /// <param name="callback">Internal Callback, after Response is set</param>
        /// <param name="timeout">Max call lifetime in milliseconds</param>
        /// <param name="parameter">Parameters for the call</param>
        public IEnumerator ProcessRequest<T>(PostboxCallName callName, string localRequestId, PostboxResponseCallback<T> userCallback = null, PostboxDelayCallback<T> callback = null, int timeout = DefaultTimeoutValue, params PostboxCallParameter[] parameter) where T : PostboxResponse
        {
            // --- Pause all Calls, exept the initialization calls at Start of the API, to prevent errors ---
            if (APIReady == false && (callName != PostboxCallName.CheckAppID && 
                                        callName != PostboxCallName.CheckDeviceID && 
                                        callName != PostboxCallName.RegisterDevice))
            {
                while (!APIReady)
                {
                    yield return new WaitForEndOfFrame();
                }
            }

            // --- 1. Create request string xml/json ---
            string requestString = "";
            PostboxLogbook.Instance.Log("Start Request '" + callName.ToString() + "'", PostboxLogbook.NotificationType.Notification);

            string url = this.url;

            switch (communicationType)
            {
                case PostboxCommunicationType.xml:
                    requestString = xmlCreator.CreateRequest(callName.ToString(), parameter);
                    url += "index2.xml.php";
                    PostboxLogbook.Instance.Log("Request " + callName.ToString() + ":" + System.Environment.NewLine + xmlCreator.FormatString(requestString), PostboxLogbook.NotificationType.APICalls);
                    break;
                case PostboxCommunicationType.json:
                    requestString = jsonCreator.CreateRequest(callName.ToString(), parameter);
                    url += "index.json.php";
                    PostboxLogbook.Instance.Log("Request " + callName.ToString() + ":" + System.Environment.NewLine + jsonCreator.FormatString(requestString), PostboxLogbook.NotificationType.APICalls);
                    break;
                default:
                    break;
            }

            // Add call to data param that will be readed at the api
            if (requestString != null && requestString != "")
            {
                connection.AddField("data", requestString);
            }

            // --- 2. Send request to server ---
            WWW PostboxResponse = new WWW(url, connection);
            yield return PostboxResponse;

            // --- 3. Create PostboxResponse ---
            XmlDocument xmlDocument = null;  
            JSONObject jsonObject = null;
            PostboxResponse response = null;
            bool validData = false;

            if (!System.String.IsNullOrEmpty(PostboxResponse.text))
            {
                // Decode Response 
                switch (communicationType)
                {
                    case PostboxCommunicationType.xml:
                        xmlDocument = xmlCreator.Decode(PostboxResponse.text);

                        if(xmlDocument != null)
                        {
                            PostboxLogbook.Instance.Log("Response " + callName.ToString() + ":" + System.Environment.NewLine + xmlCreator.FormatString(PostboxResponse.text), PostboxLogbook.NotificationType.APICalls);
                            validData = true;
                        }
                        
                        break;
                    case PostboxCommunicationType.json:
                        jsonObject = jsonCreator.Decode(PostboxResponse.text);

                        if (jsonObject != null)
                        {
                            PostboxLogbook.Instance.Log("Response " + callName.ToString() + ":" + System.Environment.NewLine + jsonCreator.FormatString(PostboxResponse.text), PostboxLogbook.NotificationType.APICalls);
                            validData = true;
                        }

                        break;
                    default:
                        break;
                }
            }

            if (!validData)
            {
                PostboxLogbook.Instance.Log("Response " + callName.ToString() + ": No result from server. Check if the specified URL can be accessed: " + url, PostboxLogbook.NotificationType.Error);
            }

            // Create Response Object
            switch (communicationType)
            {
                case PostboxCommunicationType.xml:
                    response = CreateResponse(callName, localRequestId, xmlDocument);
                    AddResponse(localRequestId, response);
                    break;
                case PostboxCommunicationType.json:
                    response = CreateResponse(callName, localRequestId, jsonObject);
                    AddResponse(localRequestId, response);
                    break;
                default:
                    break;
            }

            // --- 4. Init AfterResponse Callback ---
            if (callback != null)
            {
                callback(response, userCallback, timeout, parameter);
            }
        }

        /// <summary>
        /// Method for checking package arriving and call userCallback
        /// </summary>
        /// <typeparam name="T">Type of PostboxResponse child class</typeparam>
        /// <param name="response">Child class of PostboxResponse</param>
        /// <param name="callback">Callback with PostboxResponse child class type</param>
        /// <param name="timeout">Max call lifetime in milliseconds</param>
        /// <param name="parameters">Additional Parameters</param>
        private void AfterResponseTask<T>(PostboxResponse response, PostboxResponseCallback<T> callback, int timeout = DefaultTimeoutValue, params PostboxCallParameter[] parameters) where T : PostboxResponse
        {
            if (callback != null)
            {
                var timestamp = DateTime.Now.AddMilliseconds(timeout);
                StartCoroutine(RequestResponseToUser<T>(response, callback, timestamp.Ticks));
            }
        }

        /// <summary>
        /// Checking the response in data management and start userCallback
        /// </summary>
        /// <typeparam name="T">Type of PostboxResponse child class</typeparam>
        /// <param name="response">Child class of PostboxResponse</param>
        /// <param name="userCallback">Callback with PostboxResponse child class type</param>
        /// <param name="endTime">Time when the call will be canceled</param>
        private IEnumerator RequestResponseToUser<T>(PostboxResponse response, PostboxResponseCallback<T> userCallback, long endTime) where T : PostboxResponse
        {
            Boolean status = true;

            while (status)
            {
                if (DateTime.Now.Ticks >= endTime)
                {
                    status = false;
                    yield return null;
                }
                else
                {
                    if (response != null && ResponseExists(response.LocalRequestId))
                    {
                        // Send Callback
                        userCallback(GetResponse<T>(response.LocalRequestId));

                        status = false;
                        yield return null;
                    }
                }

                yield return new WaitForEndOfFrame();
            }

            RemoveResponse(response.LocalRequestId);
        }

        /// <summary>
        /// A General Method to implement Interval Requests
        /// </summary>
        /// <param name="callName">Callname of the PostboxService</param>
        /// <param name="userCallback">Child class of PostboxResponse</param>
        /// <param name="interval">loop interval in seconds. Set 0 for now loop.</param>
        /// <param name="timeout">Time in seconds for send a new status request</param>
        /// <param name="parameter">Parameters for the call</param>
        public IEnumerator ProcessIntervalRequest<T>(PostboxCallName callName, PostboxResponseCallback<T> userCallback, int interval, int timeout, params PostboxCallParameter[] parameter) where T : PostboxResponse
        {
            float cooldown = interval;
            float nextRefreshTime = 0.0f;

            while (true)
            {
                if(!APIReady)
                {
                    yield return new WaitForEndOfFrame();
                }

                if (Time.time > nextRefreshTime)
                {
                    // Call The Server for get Messages
                    StartCoroutine(ProcessRequest(callName,
                                                    GenerateLocalRequestId(callName),
                                                    userCallback,
                                                    AfterResponseTask<T>,
                                                    timeout,
                                                    parameter));

                    if (interval <= 0)
                    {
                        break;
                    }

                    nextRefreshTime = Time.time + cooldown;
                }

                yield return new WaitForEndOfFrame();
            }
        }

        #endregion

        #region Methods creating responses

        /// <summary>
        /// Create an response of XML data for the callName
        /// </summary>
        /// <param name="callName">Callname for Response</param>
        /// <param name="requestId">localRequestID of package managment</param>
        /// <param name="xmlDocument">XML Object</param>
        /// <returns>PostboxResponse child class via callName</returns>
        protected PostboxResponse CreateResponse(PostboxCallName callName, string requestId, XmlDocument xmlDocument)
        {
            PostboxResponse output = null;

            switch (callName) //typeof(T).ToString()
            {
                case PostboxCallName.GetServerTime:
                    output = new PostboxServerTimeResponse(requestId, callName, xmlDocument);
                    break;
                case PostboxCallName.SendDataPackageToDevice:
                    output = new PostboxSendDataPackageToDeviceResponse(requestId, callName, xmlDocument);
                    break;
                case PostboxCallName.UpdateDataPackageStatus:
                    output = new PostboxUpdateDataPackageResponse(requestId, callName, xmlDocument);
                    break;
                case PostboxCallName.GetDataPackages:
                    output = new PostboxGetDataPackagesResponse(requestId, callName, xmlDocument);
                    break;
                case PostboxCallName.CheckDeviceID:
                    output = new PostboxCheckDeviceIdResponse(requestId, callName, xmlDocument);
                    break;
                case PostboxCallName.RegisterDevice:
                    output = new PostboxRegisterDeviceResponse(requestId, callName, xmlDocument);
                    break;
                case PostboxCallName.AddDeviceToFriendlist:
                    output = new PostboxAddToFriendlistResponse(requestId, callName, xmlDocument);
                    break;
                case PostboxCallName.RemoveDeviceFromFriendlist:
                    output = new PostboxRemoveFromFriendlistResponse(requestId, callName, xmlDocument);
                    break;
                case PostboxCallName.GetFriendlist:
                    output = new PostboxGetFriendlistResponse(requestId, callName, xmlDocument);
                    break;
                case PostboxCallName.GetDataPackageStatus:
                    output = new PostboxGetDataPackageStatusResponse(requestId, callName, xmlDocument);
                    break;
                case PostboxCallName.CheckAppID:
                    output = new PostboxCheckAppIDResponse(requestId, callName, xmlDocument);
                    break;
                case PostboxCallName.RegisterPlayer:
                    output = new PostboxRegisterPlayerResponse(requestId, callName, xmlDocument);
                    break;
                case PostboxCallName.PingDevice:
                    output = new PostboxResponse(requestId, callName, xmlDocument);
                    break;
                case PostboxCallName.PingResponse:
                    output = new PostboxResponse(requestId, callName, xmlDocument);
                    break;
                case PostboxCallName.PingStatus:
                    output = new PostboxPingDeviceResponse(requestId, callName, xmlDocument);
                    break;
                default:
                    break;
            }

            return output;
        }

        /// <summary>
        /// Create an response of JSON object for the callName
        /// </summary>
        /// <param name="callName">Callname for Response</param>
        /// <param name="requestId">localRequestID of package managment</param>
        /// <param name="jsonObject">JSON Object</param>
        /// <returns>PostboxResponse child class via callName</returns>
        protected PostboxResponse CreateResponse(PostboxCallName callName, string requestId, JSONObject jsonObject)
        {
            PostboxResponse output = null;

            switch (callName) //typeof(T).ToString()
            {
                case PostboxCallName.GetServerTime:
                    output = new PostboxServerTimeResponse(requestId, callName, jsonObject);
                    break;
                case PostboxCallName.SendDataPackageToDevice:
                    output = new PostboxSendDataPackageToDeviceResponse(requestId, callName, jsonObject);
                    break;
                case PostboxCallName.UpdateDataPackageStatus:
                    output = new PostboxUpdateDataPackageResponse(requestId, callName, jsonObject);
                    break;
                case PostboxCallName.GetDataPackages:
                    output = new PostboxGetDataPackagesResponse(requestId, callName, jsonObject);
                    break;
                case PostboxCallName.CheckDeviceID:
                    output = new PostboxCheckDeviceIdResponse(requestId, callName, jsonObject);
                    break;
                case PostboxCallName.RegisterDevice:
                    output = new PostboxRegisterDeviceResponse(requestId, callName, jsonObject);
                    break;
                case PostboxCallName.AddDeviceToFriendlist:
                    output = new PostboxAddToFriendlistResponse(requestId, callName, jsonObject);
                    break;
                case PostboxCallName.RemoveDeviceFromFriendlist:
                    output = new PostboxRemoveFromFriendlistResponse(requestId, callName, jsonObject);
                    break;
                case PostboxCallName.GetFriendlist:
                    output = new PostboxGetFriendlistResponse(requestId, callName, jsonObject);
                    break;
                case PostboxCallName.GetDataPackageStatus:
                    output = new PostboxGetDataPackageStatusResponse(requestId, callName, jsonObject);
                    break;
                case PostboxCallName.CheckAppID:
                    output = new PostboxCheckAppIDResponse(requestId, callName, jsonObject);
                    break;
                case PostboxCallName.RegisterPlayer:
                    output = new PostboxRegisterPlayerResponse(requestId, callName, jsonObject);
                    break;
                case PostboxCallName.PingDevice:
                    output = new PostboxResponse(requestId, callName, jsonObject);
                    break;
                case PostboxCallName.PingResponse:
                    output = new PostboxResponse(requestId, callName, jsonObject);
                    break;
                case PostboxCallName.PingStatus:
                    output = new PostboxPingDeviceResponse(requestId, callName, jsonObject);
                    break;
                default:
                    break;
            }

            return output;
        }

        #endregion

        #region ------------ ServerTime ------------

        /// <summary>
        /// Requests the current DateTime of the Server
        /// </summary>
        /// <param name="userCallback">Callback method of the user</param>
        public void RequestServerSystemTime(PostboxResponseCallback<PostboxServerTimeResponse> userCallback)
        {
            // --- Start Request ---
            string localRequestId = GenerateLocalRequestId(PostboxCallName.GetServerTime);
            StartCoroutine(ProcessRequest(PostboxCallName.GetServerTime, localRequestId, userCallback, AfterResponseTask<PostboxServerTimeResponse>));
        }

        #endregion

        #region ------------ Data Packages ------------

        /// <summary>
        /// Sends an package for an other device to the server
        /// </summary>
        /// <param name="receiverDeviceId">DeviceId of the receiver</param>
        /// <param name="data">Data for the other device</param>
        /// <param name="userCallback">Callback method of the user</param>
        /// <param name="receiverAppId">AppId of receiver, to send packages accross apps</param>
        /// <param name="timeout">Max call lifetime in milliseconds</param>
        /// <param name="parameters">Additional parameters for the call</param>
        public void SendDataPackageToDevice(string receiverDeviceId, string data, PostboxResponseCallback<PostboxSendDataPackageToDeviceResponse> userCallback, string receiverAppId = "", int timeout = DefaultTimeoutValue, params PostboxCallParameter[] parameters)
        {
            // --- Start Request ---
            string localRequestId = GenerateLocalRequestId(PostboxCallName.SendDataPackageToDevice);

            PostboxCallParameter[] parameters2 = new PostboxCallParameter[(parameters.Length + 3)];

            parameters2[0] = new PostboxCallParameter("ReceiverAppID", receiverAppId != "" ? receiverAppId : appId);
            parameters2[1] = new PostboxCallParameter("ReceiverDeviceID", receiverDeviceId);
            parameters2[2] = new PostboxCallParameter("Data", data);

            for (int i = 0; i < parameters.Length; i++)
            {
                parameters2[i + 3] = parameters[i];
            }

            StartCoroutine(ProcessRequest(PostboxCallName.SendDataPackageToDevice, localRequestId, userCallback, AfterResponseTask<PostboxSendDataPackageToDeviceResponse>, timeout, parameters2));
        }

        /// <summary>
        /// Update the status of an defined package of the user
        /// </summary>
        /// <param name="packageTransactionId">TransactionId of the package that will be affeted</param>
        /// <param name="status">New package status</param>
        /// <param name="userCallback">Callback method of the user</param>
        /// <param name="message">Additional notification for staus change</param>
        /// <param name="timeout">Max call lifetime in milliseconds</param>
        /// <param name="parameters">Additional parameters for the call</param>
        public void UpdateDataPackageStatus(string packageTransactionId, PostboxDataPackageStatus status, PostboxResponseCallback<PostboxUpdateDataPackageResponse> userCallback, string message = "", int timeout = DefaultTimeoutValue, params PostboxCallParameter[] parameters)
        {
            // --- Start Request ---
            string localRequestId = GenerateLocalRequestId(PostboxCallName.UpdateDataPackageStatus);

            PostboxCallParameter[] parameters2 = new PostboxCallParameter[(parameters.Length + 3)];

            parameters2[0] = new PostboxCallParameter("TransactionId", packageTransactionId);
            parameters2[1] = new PostboxCallParameter("Status", status.ToString());
            parameters2[2] = new PostboxCallParameter("Notification", message);

            for (int i = 0; i < parameters.Length; i++)
            {
                parameters2[i + 3] = parameters[i];
            }

            StartCoroutine(ProcessRequest(PostboxCallName.UpdateDataPackageStatus, localRequestId, userCallback, AfterResponseTask<PostboxUpdateDataPackageResponse>, timeout, parameters2));
        }

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
        public string GetDataPackages(PostboxResponseCallback<PostboxGetDataPackagesResponse> userCallback, int interval=0, bool filterByStatus = true, PostboxDataPackageStatus status = PostboxDataPackageStatus.open, string fromDevice = "", string fromApp="", int timeout = DefaultTimeoutValue)
        {
            PostboxCallParameter statusFlags = null;
            PostboxCallParameter fromDeviceParam = null;
            PostboxCallParameter fromAppParam = null;
            fromDevice = fromDevice.Trim();
            fromApp = fromApp.Trim();

            if (filterByStatus)
            {
                statusFlags = new PostboxCallParameter("Status", ((int)status).ToString());
            }

            // Device-Filter
            if (!String.IsNullOrEmpty(fromDevice))
            {
                fromDeviceParam = new PostboxCallParameter("FromDevice", fromDevice);
            }

            // App-Filter
            if (!String.IsNullOrEmpty(fromApp))
            {
                fromAppParam = new PostboxCallParameter("FromDevice", fromApp);
            }

            Coroutine co = StartCoroutine(ProcessIntervalRequest<PostboxGetDataPackagesResponse>(PostboxCallName.GetDataPackages,
                                                                                            userCallback,
                                                                                            interval,
                                                                                            timeout,
                                                                                            statusFlags, fromDeviceParam, fromAppParam));
            return AddParallelProcess(co);
        }

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
        public string GetDataPackagesFiltered(PostboxResponseCallback<PostboxGetDataPackagesResponse> userCallback, int interval = 0, bool filterByStatus = true, PostboxDataPackageStatus status = PostboxDataPackageStatus.open, string[] fromDevices = null, string[] fromApps = null, int timeout = DefaultTimeoutValue)
        {
            string fromDevicesString = "";
            string fromAppsString = "";

            // -- Use string Join to concatenate the string elements. --
            if (fromDevices != null)
            {
                fromDevicesString = string.Join(",", fromDevices);
            }

            if (fromApps != null)
            {
                fromAppsString = string.Join(",", fromApps);
            }

            return GetDataPackages(userCallback, interval, filterByStatus, status, fromDevicesString, fromAppsString, timeout);
        }

        /// <summary>
        /// Stop the GetDataPackage Loop for a specific call
        /// </summary>
        /// <param name="processId">The identifier of the specific process</param>
        public void StopGetPackages(string processId)
        {
            StopParallelProcess(processId);
        }

        /// <summary>
        /// Request the current status of the defined package
        /// </summary>
        /// <param name="packageTransactionId">TransactionId of the package that will be affeted</param>
        /// <param name="userCallback">Callback method of the user</param>
        /// <param name="interval"></param>
        /// <param name="timeout">Max call lifetime in milliseconds</param>
        /// <returns>Return the ID of the Process that needed for stop it manually.</returns>
        public string GetDataPackageStatus(string packageTransactionId, PostboxResponseCallback<PostboxGetDataPackageStatusResponse> userCallback, int interval=0, int timeout= DefaultTimeoutValue)
        {
            Coroutine co = StartCoroutine(ProcessIntervalRequest<PostboxGetDataPackageStatusResponse>(PostboxCallName.GetDataPackageStatus,
                                                                                                userCallback,
                                                                                                interval,
                                                                                                timeout,
                                                                                                new PostboxCallParameter("TransactionID", packageTransactionId)));
            return AddParallelProcess(co);
        }

        /// <summary>
        /// Stop the GetPackageStatus Loop for a specific call
        /// </summary>
        /// <param name="processId">The identifier of the specific process</param>
        public void StopGetPackageStatus(string processId)
        {
            StopParallelProcess(processId);
        }

        /// <summary>
        /// Set status of specific data package status to "received"
        /// </summary>
        /// <param name="packageTransactionId">TransactionId of the package that will be affeted</param>
        /// <param name="userCallback">Callback method of the user</param>
        /// <param name="message">Additional notification for staus change</param>
        /// <param name="timeout">Max call lifetime in milliseconds</param>
        /// <param name="parameters">Additional parameters for the call</param>
        public void SetDataPackageReceived(string packageTransactionId, PostboxResponseCallback<PostboxUpdateDataPackageResponse> userCallback, string message = "", int timeout = DefaultTimeoutValue, params PostboxCallParameter[] parameters)
        {
            UpdateDataPackageStatus(packageTransactionId, PostboxDataPackageStatus.received, userCallback, message, timeout, parameters);
        }

        /// <summary>
        /// Set status of specific data package status to "processing"
        /// </summary>
        /// <param name="packageTransactionId">TransactionId of the package that will be affeted</param>
        /// <param name="userCallback">Callback method of the user</param>
        /// <param name="message">Additional notification for staus change</param>
        /// <param name="timeout">Max call lifetime in milliseconds</param>
        /// <param name="parameters">Additional parameters for the call</param>
        public void SetDataPackageProcessing(string packageTransactionId, PostboxResponseCallback<PostboxUpdateDataPackageResponse> userCallback, string message = "", int timeout = DefaultTimeoutValue, params PostboxCallParameter[] parameters)
        {
            UpdateDataPackageStatus(packageTransactionId, PostboxDataPackageStatus.processing, userCallback, message, timeout, parameters);
        }

        /// <summary>
        /// Set status of specific data package status to "done"
        /// </summary>
        /// <param name="packageTransactionId">TransactionId of the package that will be affeted</param>
        /// <param name="userCallback">Callback method of the user</param>
        /// <param name="message">Additional notification for staus change</param>
        /// <param name="timeout">Max call lifetime in milliseconds</param>
        /// <param name="parameters">Additional parameters for the call</param>
        public void SetDataPackageDone(string packageTransactionId, PostboxResponseCallback<PostboxUpdateDataPackageResponse> userCallback, string message = "", int timeout = DefaultTimeoutValue, params PostboxCallParameter[] parameters)
        {
            UpdateDataPackageStatus(packageTransactionId, PostboxDataPackageStatus.done, userCallback, message, timeout, parameters);
        }

        /// <summary>
        /// Set status of specific data package status to "error"
        /// </summary>
        /// <param name="packageTransactionId">TransactionId of the package that will be affeted</param>
        /// <param name="userCallback">Callback method of the user</param>
        /// <param name="message">Additional notification for staus change</param>
        /// <param name="timeout">Max call lifetime in milliseconds</param>
        /// <param name="parameters">Additional parameters for the call</param>
        public void SetDataPackageError(string packageTransactionId, PostboxResponseCallback<PostboxUpdateDataPackageResponse> userCallback, string message, int timeout = DefaultTimeoutValue, params PostboxCallParameter[] parameters)
        {
            UpdateDataPackageStatus(packageTransactionId, PostboxDataPackageStatus.error, userCallback, message, timeout, parameters);
        }

        #endregion

        #region ------------ App ------------

        /// <summary>
        /// Validate the given AppID on the server
        /// </summary>
        /// <param name="appId">AppID of the given App</param>
        /// <param name="userCallback">Callback method of the user</param>
        /// <param name="timeout">Max call lifetime in milliseconds</param>
        public void CheckAppId(string appId, PostboxResponseCallback<PostboxCheckAppIDResponse> userCallback, int timeout = DefaultTimeoutValue)
        {
            // --- Start Request ---
            string localRequestId = GenerateLocalRequestId(PostboxCallName.CheckAppID);
            StartCoroutine(ProcessRequest(PostboxCallName.CheckAppID, localRequestId, userCallback, AfterResponseTask<PostboxCheckAppIDResponse>, timeout, new PostboxCallParameter("AppID", appId)));
        }

        #endregion

        #region  ------------ Device ------------

        /// <summary>
        /// Validate the given Device on the server
        /// </summary>
        /// <param name="deviceId">DeviceID of the given Device</param>
        /// <param name="userCallback">Callback method of the user</param>
        /// <param name="timeout">Max call lifetime in milliseconds</param>
        public void CheckDeviceId(string deviceId, PostboxResponseCallback<PostboxCheckDeviceIdResponse> userCallback, int timeout = 1000)
        {
            // --- Start Request ---
            string localRequestId = GenerateLocalRequestId(PostboxCallName.CheckDeviceID);
            StartCoroutine(ProcessRequest(PostboxCallName.CheckDeviceID, localRequestId, userCallback, AfterResponseTask<PostboxCheckDeviceIdResponse>, timeout, new PostboxCallParameter("DeviceID", deviceId)));
        }

        /// <summary>
        /// Register an new device at the API and inform the user about the success of the operation
        /// </summary>
        /// <param name="userCallback">Callback method of the user</param>
        /// <param name="timeout">Max call lifetime in milliseconds</param>
        public void RegisterDevice(PostboxResponseCallback<PostboxRegisterDeviceResponse> userCallback, int timeout = 1000)
        {
            // --- Start Request ---
            string localRequestId = GenerateLocalRequestId(PostboxCallName.RegisterDevice);

            StartCoroutine(ProcessRequest(PostboxCallName.RegisterDevice, localRequestId,
                                            userCallback,
                                            AfterResponseTask<PostboxRegisterDeviceResponse>,
                                            timeout,
                                            //new PostboxCallParameter("MacAddress", GetMacAddress()),
                                            new PostboxCallParameter("Model", SystemInfo.deviceModel.ToString()),
                                            new PostboxCallParameter("Type", SystemInfo.deviceType.ToString()),
                                            new PostboxCallParameter("OS", SystemInfo.operatingSystem)));
        }

        #endregion

        #region  ------------ Friendlist ------------

        ///// <summary>
        ///// Send Request for Add the given deviceId to playerFriendlist
        ///// </summary>
        ///// <returns></returns>
        //public string AddFriendToList(string friendDeviceId, PostboxResponseCallback callback)
        //{
        //    // --- Start Request ---
        //    string localRequestId = GenerateLocalRequestId(PostboxCallName.AddDeviceToFriendlist);
        //    StartCoroutine(ProcessRequest(PostboxCallName.AddDeviceToFriendlist, localRequestId, callback,
        //                                    new PostboxCallParameter("PublicDeviceID", UserSettings.PublicDeviceId),
        //                                    new PostboxCallParameter("FriendPublicDeviceID", friendDeviceId)));

        //    return localRequestId;
        //}

        ///// <summary>
        ///// Send Request for Remove the given deviceId to playerFriendlist
        ///// </summary>
        ///// <returns></returns>
        //public string RemoveFriendFromList(string friendDeviceId, PostboxResponseCallback callback)
        //{
        //    // --- Start Request ---
        //    string localRequestId = GenerateLocalRequestId(PostboxCallName.RemoveDeviceFromFriendlist);
        //    StartCoroutine(ProcessRequest(PostboxCallName.RemoveDeviceFromFriendlist, localRequestId, callback,
        //                                    new PostboxCallParameter("PublicDeviceID", UserSettings.PublicDeviceId),
        //                                    new PostboxCallParameter("FriendPublicDeviceID", friendDeviceId)));

        //    return localRequestId;
        //}

        ///// <summary>
        ///// Get Friendlist for current user
        ///// </summary>
        ///// <returns></returns>
        //public string GetFriendlist(PostboxResponseCallback callback)
        //{
        //    // --- Start Request ---
        //    string localRequestId = GenerateLocalRequestId(PostboxCallName.GetFriendlist);
        //    StartCoroutine(ProcessRequest(PostboxCallName.GetFriendlist, localRequestId, callback, new PostboxCallParameter("PublicDeviceID", UserSettings.PublicDeviceId)));

        //    return localRequestId;
        //}

        //#endregion

        //#region functions for Player
        //public string RegisterPlayer(string name, string email, string password, PostboxResponseCallback callback) // Action<
        //{
        //    string localRequestId = GenerateLocalRequestId(PostboxCallName.RegisterPlayer);
        //    StartCoroutine(ProcessRequest(PostboxCallName.RegisterPlayer, localRequestId, callback, 
        //                                    new PostboxCallParameter("Name", name),
        //                                    new PostboxCallParameter("Email", email),
        //                                    new PostboxCallParameter("Password", password)));

        //    return localRequestId;
        //}

        //#endregion

        //#region functions for Player

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
        /// <param name="userCallback">Callback method of the user</param>
        /// <param name="timeout">Max call lifetime in milliseconds</param>
        public void PingDevice(string deviceId, PostboxResponseCallback<PostboxPingDeviceResponse> userCallback, int timeout = 3000)
        {
            string localRequestId = GenerateLocalRequestId(PostboxCallName.PingDevice);

            StartCoroutine(ProcessRequest(PostboxCallName.PingDevice, localRequestId, 
                                            userCallback, AfterPingResponseTask<PostboxPingDeviceResponse>, timeout,
                                            new PostboxCallParameter("DeviceID", deviceId)));
        }

        /// <summary>
        /// Method for checking arriving of the ping call and start request the ping status
        /// </summary>
        /// <typeparam name="T">Type of PostboxResponse child class</typeparam>
        /// <param name="response">Child class of PostboxResponse</param>
        /// <param name="userCallback">Callback with PostboxResponse child class type</param>
        /// <param name="timeout">Max call lifetime in milliseconds</param>
        /// <param name="parameters">Additional Parameters</param>
        private void AfterPingResponseTask<T>(PostboxResponse response, PostboxResponseCallback<T> userCallback, int timeout = 3000, params PostboxCallParameter[] parameters) where T : PostboxResponse
        {
            var timestamp = DateTime.Now.AddMilliseconds(timeout);

            if(response != null && response.CallStatus == PostboxCallStatus.Success)
            {
                // Start Loop for request ping status
                StartCoroutine(RequestPingResponseToUser<T>(parameters[0].Value, response, userCallback, timestamp.Ticks, 5000));
            }
            else
            {
                // log errors
                response.LogAllAPIErrors();
            }
        }

        /// <summary>
        /// Loop PingStatus Request and inform userCallback until the ping request is end
        /// </summary>
        /// <typeparam name="T">Type of PostboxResponse child class</typeparam>
        /// <param name="deviceID">DeviceID of the other Device</param>
        /// <param name="response">Child class of PostboxResponse</param>
        /// <param name="userCallback">Callback with PostboxResponse child class type</param>
        /// <param name="endTime">Time when the call will be canceled</param>
        /// <param name="callFrequence">Time in seconds for send a new status request</param>
        private IEnumerator RequestPingResponseToUser<T>(string deviceID, PostboxResponse response, PostboxResponseCallback<T> userCallback, long endTime, int callFrequence) where T : PostboxResponse
        {
            string localRequestId = "";
            T pingResponse = null;
            DateTime callTimeNext = DateTime.Now;

            while (true)
            {
                if(!APIReady)
                    yield return new WaitForEndOfFrame();

                if (DateTime.Now.Ticks >= endTime)
                {
                    yield return null;
                }
                else
                {
                    if (ResponseExists(localRequestId))
                    {
                        pingResponse = GetResponse<T>(localRequestId);
                        RemoveResponse(localRequestId);
                    }

                    if (pingResponse != null)
                    {
                        // Send Callback
                        userCallback(pingResponse);

                        PostboxPingDeviceResponse pa = pingResponse as PostboxPingDeviceResponse;

                        if (pa.PingStatus == PostboxPingStatus.device)
                        {
                            endTime = 0;
                            yield return null;
                        }
                        else
                        {
                            // Send new status-request
                            pingResponse = null;
                        }
                    }
                    else if (DateTime.Now.Ticks >= callTimeNext.Ticks)
                    {
                        // Request new Status
                        string requestString = "";
                        PostboxCallName callName = PostboxCallName.PingStatus;

                        localRequestId = GenerateLocalRequestId(callName);
                        callTimeNext = callTimeNext.AddMilliseconds(callFrequence);

                        requestString = xmlCreator.CreateRequest(callName.ToString(), new PostboxCallParameter("DeviceID", deviceID));

                        // Add call to data param that will be readed at the api
                        if (requestString != null && requestString != "")
                        {
                            connection.AddField("data", requestString);
                        }

                        // --- 2. Send request to server ---
                        WWW PostboxResponse = new WWW(url, connection);
                        yield return PostboxResponse;

                        XmlDocument xmlDocument = xmlCreator.Decode(PostboxResponse.text);
                        PostboxResponse statusResponse = null;

                        if (PostboxResponse.text != null && PostboxResponse.text != "")
                        {
                            PostboxLogbook.Instance.Log("Response " + callName.ToString() + ":" + PostboxResponse.text, PostboxLogbook.NotificationType.APICalls);
                            xmlDocument = xmlCreator.Decode(PostboxResponse.text);
                        }
                        else
                        {
                            // If Empty
                            PostboxLogbook.Instance.Log("Response " + callName.ToString() + ": No result to server.", PostboxLogbook.NotificationType.Error);
                        }

                        statusResponse = CreateResponse(callName, localRequestId, xmlDocument);
                        AddResponse(localRequestId, statusResponse);
                    }

                    yield return new WaitForEndOfFrame();
                }
            }

            yield return null;
        }

        /// <summary>
        /// Response that the device is alive for the api
        /// </summary>
        /// <param name="userCallback">Child class of PostboxResponse</param>
        /// <param name="interval">loop interval in seconds. Set 0 for now loop.</param>
        /// <param name="timeout">Time in seconds for send a new status request</param>
        /// <returns>Return the ID of the Process that needed for stop it manually.</returns>
        public string PingResponse(PostboxResponseCallback<PostboxResponse> userCallback, int interval=0, int timeout = DefaultTimeoutValue)
        {
            Coroutine co = StartCoroutine(ProcessIntervalRequest<PostboxResponse>(PostboxCallName.PingResponse,
                                                                            userCallback,
                                                                            interval,
                                                                            timeout));
            return AddParallelProcess(co);
        }

        /// <summary>
        /// Start the auto ping parallel process
        /// </summary>
        public void StartAutoPingResponse()
        {
            if(System.String.IsNullOrEmpty(autoPingProcessId))
            { 
                AutoPingResponse = true;
                autoPingProcessId = PingResponse(null, AutoPingResponseInterval);
            }
            else
            {
                PostboxLogbook.Instance.Log("Autoping is already started. Stop the AutoPing before restart.", PostboxLogbook.NotificationType.Notification);
            }
        }

        /// <summary>
        /// Stop the auto ping parallel process
        /// </summary>
        public void StopAutoPingResponse()
        {
            AutoPingResponse = false;
            StopParallelProcess(autoPingProcessId);
            autoPingProcessId = "";
        }

        #endregion

        #region Methods of PostboxResponse DataManager

        /// <summary>
        /// Add PostboxResponse to ResponsesList
        /// </summary>
        /// <param name="localRequestId">internal requestId that will generated at the start of the call</param>
        /// <param name="PostboxResponse">Response of API-Libery</param>
        private void AddResponse(string localRequestId, PostboxResponse PostboxResponse)
        {
            PostboxResponses.Add(localRequestId, PostboxResponse);
        }

        /// <summary>
        /// Get PostboxResponse by localRequestId
        /// </summary>
        /// <param name="localRequestId">internal requestId that will generated at the start of the call</param>
        /// <returns></returns>
        public T GetResponse<T>(string localRequestId)
        {
            PostboxResponse res = null;
            PostboxResponses.TryGetValue(localRequestId, out res);
            return (T)Convert.ChangeType(res, typeof(T));
        }

        /// <summary>
        /// Check back if Key existis
        /// </summary>
        /// <param name="localRequestId">internal requestId that will generated at the start of the call</param>
        /// <returns></returns>
        public bool ResponseExists(string localRequestId)
        {
            return PostboxResponses.ContainsKey(localRequestId);
        }

        /// <summary>
        /// Remove PostboxResponse by localRequestId
        /// </summary>
        /// <param name="localRequestId">internal requestId that will generated at the start of the call</param>
        public void RemoveResponse(string localRequestId)
        {
            PostboxResponses.Remove(localRequestId);
        }

        #endregion

        #region Methods of PostboxCoroutine DataManager

        /// <summary>
        /// Add a Coroutine to the ProccessManager
        /// </summary>
        /// <param name="coroutine"></param>
        private string AddParallelProcess(Coroutine coroutine)
        {
            string coroutineId = GenerateCoroutineId();
            PostboxCoroutines.Add(coroutineId, coroutine);

            return coroutineId;
        }

        /// <summary>
        /// Stop and Remove a Coroutine from the ProccessManager
        /// </summary>
        /// <param name="proccessId">process identifier</param>
        /// <returns></returns>
        public bool StopParallelProcess(string proccessId)
        {
            Coroutine coroutine;
            bool status = PostboxCoroutines.TryGetValue(proccessId, out coroutine);
            
            if(coroutine != null)
            {
                StopCoroutine(coroutine);
                PostboxCoroutines.Remove(proccessId);
            }

            return status;
        }

        /// <summary>
        /// Stop and Remove all Coroutines from the ProccessManager
        /// </summary>
        public void StopAllParallelProcesses()
        {
            foreach (KeyValuePair<string,Coroutine> item in PostboxCoroutines)
            {
                StopParallelProcess(item.Key);
            }
        }

        #endregion

        #region Utilities

        /// <summary>
        /// Generate the request identifier for the RequestList.
        /// </summary>
        /// <returns>The request identifier.</returns>
        private string GenerateLocalRequestId(PostboxCallName callName)
        {
            TimeSpan span = DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1, 0, 0, 0));
            string localRequestId = callName.ToString() + span.Ticks.ToString();

            return localRequestId;
        }

        /// <summary>
        /// Genertate an idientifier for the CoroutineList
        /// </summary>
        /// <returns>Identfifier</returns>
        private string GenerateCoroutineId()
        {
            TimeSpan span = DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1, 0, 0, 0));
            string localRequestId = span.Ticks.ToString();

            return localRequestId;
        }

        #endregion

        #region Methods for UserSettings

        /// <summary>
        /// Save UserSettings to PlayerPrefs
        /// </summary>
        /// <param name="userSettings">The PostboxPlayerConfig object for saving</param>
        public bool SaveUserSettings(PostboxPlayerConfig userSettings)
        {
            bool status = false;
            UserSettings = userSettings;

            if (UserSettings.Check())
            {
                if (Application.isEditor)
                {
                    // Create an seperat ConfigFile for the editor
                    PlayerPrefs.SetString("PostboxSettingsEditor", jsonCreator.EncodeUserSettings(UserSettings));
                    PlayerPrefs.Save();
                }
                else
                {
                    // Create an ConfigFile for the build
                    PlayerPrefs.SetString("PostboxSettings", jsonCreator.EncodeUserSettings(UserSettings));
                    PlayerPrefs.Save();
                }

                PostboxLogbook.Instance.Log("Usersettings saved.", PostboxLogbook.NotificationType.Notification);
                status = true;
            }
            else
            {
                PostboxLogbook.Instance.Log("Usersettings invalid, and not be saved.", PostboxLogbook.NotificationType.Error);
            }

            // Refresh UserSettings Variable
            LoadUserSettings();

            return status;
        }

        /// <summary>
        /// Load Settings and write it into the userSettings variable of the connector
        /// </summary>
        /// <returns>boolean for the successful load of the data</returns>
        public bool LoadUserSettings()
        {
            PostboxLogbook.Instance.Log("Begin loading user settings.", PostboxLogbook.NotificationType.Notification);

            bool status = false;
            string postboxUserSettings = "";

            if (Application.isEditor)
            {
                // Get the ConfigFile of the editor
                postboxUserSettings = PlayerPrefs.GetString("PostboxSettingsEditor", "");
            }
            else
            {
                // Get the ConfigFile of the build
                postboxUserSettings = PlayerPrefs.GetString("PostboxSettings", "");
            }

            // Empty class variable
            UserSettings = new PostboxPlayerConfig("", "", "");

            if (postboxUserSettings != "")
            {
                // Decode UserSettings
                JSONObject jsonPostboxUserSettings = jsonCreator.Decode(postboxUserSettings);

                UserSettings = new PostboxPlayerConfig(jsonPostboxUserSettings.GetField("DeviceId").str,
                                                        jsonPostboxUserSettings.GetField("UserName").str,
                                                        jsonPostboxUserSettings.GetField("UserPassword").str);

                // Check UserSettings
                if (UserSettings.Check())
                {
                    PostboxLogbook.Instance.Log("Usersettings load correct.", PostboxLogbook.NotificationType.Notification);
                }
                else
                {
                    PostboxLogbook.Instance.Log("Usersettings not valid. Please renew Settings.", PostboxLogbook.NotificationType.Error);
                }
            }
            else
            {
                PostboxLogbook.Instance.Log("PostboxSettings not set on PlayerPrefs.", PostboxLogbook.NotificationType.Warning);
                status = false;
            }

            // ReInit OutputCreator with userdata
            xmlCreator = new PostboxXMLCreator(this.appId, UserSettings.DeviceId,
                                        this.LiceneKey, this.apiVersion,
                                        UserSettings.UserName, UserSettings.UserPassword);

            jsonCreator = new PostboxJSONCreator(this.appId, UserSettings.DeviceId,
                                                this.LiceneKey, this.apiVersion,
                                                UserSettings.UserName, UserSettings.UserPassword);

            return status;
        }

        /// <summary>
        /// Reset the UserSettings in PlayerPrefs
        /// </summary>
        public void ResetUserSettings()
        {
            APIReady = false;

            if (Application.isEditor)
            {
                // Delete the ConfigFile of the editor
                PlayerPrefs.DeleteKey("PostboxSettingsEditor");
            }
            else
            {
                // Delete the ConfigFile of the editor
                PlayerPrefs.DeleteKey("PostboxSettings");
            }

            // Reset PlayerPrefs and Class Variable
            PlayerPrefs.Save();
            UserSettings = new PostboxPlayerConfig("", "", "");
        }

        /// <summary>
        /// Set the DeviceID to UserSettings
        /// </summary>
        /// <param name="deviceId"></param>
        private void SetDeviceId(string deviceId)
        {
            PostboxPlayerConfig userSettings = new PostboxPlayerConfig(deviceId, UserSettings.UserName, UserSettings.UserPassword);
            UserSettings = userSettings;
        }

        /// <summary>
        ///  Set the UserName & Password to UserSettings
        /// </summary>
        /// <param name="userName">API-Username</param>
        /// <param name="userPassword">API-Userpassword</param>
        private void SetUserData(string userName, string userPassword)
        {
            PostboxPlayerConfig userSettings = new PostboxPlayerConfig(UserSettings.DeviceId, userName, userName);
            UserSettings = userSettings;
        }

        #endregion
    }
}