using System;
using System.Xml;
using System.Collections;
using System.Collections.Generic;

namespace PostboxAPI
{
    /// <summary>
    /// Response class of "RegisterDevice" call. 
    /// Returns the DeviceID of the Device, Specific Information about the Registration in NotificationCode and -Message and the CallStatus that is Success or Error.
    /// Errors could be found in the Errors Variable.
    /// </summary>
    public class PostboxRegisterDeviceResponse : PostboxResponse
    {
        /// <summary>
        /// Identifier of the registed Device
        /// </summary>
        public string DeviceId { get; private set; }

        /// <summary>
        /// Short description of the result of the specific call
        /// </summary>
        public string NotificationCode { get; private set; }

        /// <summary>
        /// Description of the result of the specific call
        /// </summary>
        public string NotificationDescription { get; private set; }


        /// <summary>
        /// Initializes a new instance of the <see cref="PostboxResponse"/> class via XmlDocument.
        /// </summary>
        /// <param name="localRequestId">localRequest identifier.</param>
        /// <param name="callName">callname of request</param>
        /// <param name="response">response object.</param>
        public PostboxRegisterDeviceResponse(string localRequestId, PostboxCallName callName, XmlDocument response) : base(localRequestId, callName, response)
        {
            if (response == null)
                return;

            XmlNode result = response.SelectSingleNode("/PostBoxGlobal/Result");

            if (result != null)
            {
                XmlNode notificationNode = result.SelectSingleNode("NotificationCode");
                if (notificationNode != null)
                    NotificationCode = notificationNode.InnerText;

                XmlNode notificationDescNode = result.SelectSingleNode("NotificationDescription");
                if (notificationDescNode != null)
                    NotificationDescription = notificationDescNode.InnerText;

                XmlNode deviceIdNode = result.SelectSingleNode("DeviceID");
                if (deviceIdNode != null)
                    DeviceId = deviceIdNode.InnerText;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PostboxResponse"/> class via JSONObject.
        /// </summary>
        /// <param name="localRequestId">localRequest identifier.</param>
        /// <param name="callName">callname of request</param>
        /// <param name="response">response object.</param>
        public PostboxRegisterDeviceResponse(string localRequestId, PostboxCallName callName, JSONObject response) : base(localRequestId, callName, response)
        {
            if (response == null)
                return;

            JSONObject result = response.GetField("Result");

            if (result != null)
            {
                if (result.GetField("DeviceID") != null)
                { 
                    DeviceId = result.GetField("DeviceID").str;
                }

                if(result.GetField("NotificationCode") != null)
                {
                    NotificationCode = result.GetField("NotificationCode").str;
                }

                if (result.GetField("NotificationDescription") != null)
                {
                    NotificationDescription = result.GetField("NotificationDescription").str;
                }
            }
        }
    }
}