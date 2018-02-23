using System;
using System.Xml;
using System.Collections;
using System.Collections.Generic;

namespace PostboxAPI
{
    /// <summary>
    /// Response class of "RemoveDeviceFromFriendlist" call. 
    /// Returns an notification code and an description.
    /// Also returns CallStatus that is Success or Error.
    /// Errors could be found in the Errors variable.
    /// </summary>
    public class PostboxRemoveFromFriendlistResponse : PostboxResponse
    {
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
        public PostboxRemoveFromFriendlistResponse(string localRequestId, PostboxCallName callName, XmlDocument response) : base(localRequestId, callName, response)
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
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PostboxResponse"/> class via JSONObject.
        /// </summary>
        /// <param name="localRequestId">localRequest identifier.</param>
        /// <param name="callName">callname of request</param>
        /// <param name="response">response object.</param>
        public PostboxRemoveFromFriendlistResponse(string localRequestId, PostboxCallName callName, JSONObject response) : base(localRequestId, callName, response)
        {
            if (response == null)
                return;

            JSONObject result = response.GetField("Result");

            if (result != null)
            {
                NotificationCode = result.GetField("NotificationCode").str;
                NotificationDescription = result.GetField("NotificationDescription").str;
            }
        }
    }
}