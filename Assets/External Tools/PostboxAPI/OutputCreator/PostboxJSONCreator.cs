using System;
using System.Linq;

namespace PostboxAPI
{
    /// <summary>
    /// This class en- and decode JSON for API and UserSettings
    /// </summary>
    public class PostboxJSONCreator : IPostboxOutputCreator
    {
        private PostboxRequest request;

        /// <summary>
        /// Empty Constructor for normal usage
        /// </summary>
        public PostboxJSONCreator()
        {

        }

        /// <summary>
        /// Constructor prepair the an RequestObject for usage
        /// </summary>
        /// <param name="appId">AppID of the connector class</param>
        /// <param name="deviceId">DeviceID of the connector class</param>
        /// <param name="liceneKey">LiceneKey of the connector class</param>
        /// <param name="apiVersion">API-Version of the connector class</param>
        /// <param name="userName">UserName of the connector class</param>
        /// <param name="userPassword">UserPassword of the connector class</param>
        public PostboxJSONCreator(string appId, string deviceId,
                                 string liceneKey = "", string apiVersion = "1.0",
                                 string userName = "", string userPassword = "")
        {
            request = new PostboxRequest(appId, deviceId, liceneKey, apiVersion, userName, userPassword);
        }

        /// <summary>
        /// Prepair an PostboxRequest for sending via the connector
        /// </summary>
        /// <param name="callName">Callname for API Service</param>
        /// <param name="parameters">Additional Parameters</param>
        /// <returns></returns>
        /// <returns>request as json-string</returns>
        public string CreateRequest(string callName, params PostboxCallParameter[] parameters)
        {
            JSONObject requestObj = new JSONObject();

            // --- Global-Header ---
            JSONObject PostboxGlobal = new JSONObject(JSONObject.Type.OBJECT);
            requestObj.AddField("PostBoxGlobal", PostboxGlobal);

            PostboxGlobal.AddField("AppID", request.AppID);
            PostboxGlobal.AddField("Version", request.Version);
            PostboxGlobal.AddField("UserName", request.UserName);
            PostboxGlobal.AddField("UserPassword", request.UserPassword);
            PostboxGlobal.AddField("LicenseKey", request.LicenseKey);
            PostboxGlobal.AddField("DeviceID", request.DeviceID);
            PostboxGlobal.AddField("CallName", callName);

            // -- Params --
            request.ApplyParameters(parameters);

            JSONObject param = new JSONObject(JSONObject.Type.ARRAY);
            JSONObject jsonData = null;
            requestObj.AddField("Param", param);

            foreach (PostboxCallParameter parameter in request.Parameters)
            {
                if (parameter != null)
                {
                    jsonData = Decode(parameter.Value);
                    if(jsonData != null)
                    {
                        param.AddField(parameter.Key, jsonData);
                    }
                    else
                    {
                        param.AddField(parameter.Key, parameter.Value);
                    }
                }
            }

            // -- Output --
            return requestObj.Print(false);
        }

        /// <summary>
        /// Decode an API Response in an JSON-Object for better working
        /// </summary>
        /// <param name="responseString"></param>
        /// <returns></returns>
        public JSONObject Decode(string responseString)
        {
            JSONObject doc = null;
            //responseString = Http//HttpUtility.HtmlDecode(responseString);

            if (!System.String.IsNullOrEmpty(responseString))
            {
                try
                {
                    doc = new JSONObject(responseString);

                    if(doc.IsNull)
                    {
                        doc = null;
                    }
                }
                catch (Exception ex)
                {
                    doc = null;
                    //PostboxLogbook.Instance.Log("An error occurred while parsing JSON -" + ex.Message, PostboxLogbook.NotificationType.Error);
                }
            }

            return doc;
        }

        /// <summary>
        /// Encode User Settings
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public string EncodeUserSettings(PostboxPlayerConfig obj)
        {
            JSONObject doc = new JSONObject(JSONObject.Type.OBJECT);
            doc.AddField("DeviceId", obj.DeviceId);
            doc.AddField("UserName", obj.UserName);
            doc.AddField("UserPassword", obj.UserPassword);

            return doc.Print(false);
        }

        /// <summary>
        /// Return the given string with indent and line breaks
        /// </summary>
        /// <param name="jsonString">String in json format</param>
        /// <returns>Formated string</returns>
        public string FormatString(string jsonString)
        {
            string output = null;

            try
            {
                JSONObject jsonObject = new JSONObject(jsonString);
                output = jsonObject.Print(true);
            }
            catch (Exception ex)
            {
                output = null;
                //PostboxLogbook.Instance.Log("An error occurred while parsing XML -" + ex.Message, PostboxLogbook.NotificationType.Error);
            }

            return output;
        }
    }
}