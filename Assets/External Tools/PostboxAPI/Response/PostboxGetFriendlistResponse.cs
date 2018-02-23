using System;
using System.Xml;
using System.Collections;
using System.Collections.Generic;

namespace PostboxAPI
{
    /// <summary>
    /// Response class of "GetFriendlist" call. 
    /// Returns a list of devices, the user is connected with in "friends".
    /// Also returns CallStatus that is Success or Error.
    /// Errors could be found in the Errors variable.
    /// </summary>
    public class PostboxGetFriendlistResponse : PostboxResponse
    {
        /// <summary>
        /// List of <see cref="PostboxDevicePackage"/>.
        /// </summary>
        public PostboxDevicePackage[] Friends { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="PostboxResponse"/> class via XmlDocument.
        /// </summary>
        /// <param name="localRequestId">localRequest identifier.</param>
        /// <param name="callName">callname of request</param>
        /// <param name="response">response object.</param>
        public PostboxGetFriendlistResponse(string localRequestId, PostboxCallName callName, XmlDocument response) : base(localRequestId, callName, response)
        {
            if (response == null)
                return;

            XmlNode result = response.SelectSingleNode("/PostBoxGlobal/Result");

            if (result != null)
            {
                // --- Friends ---
                XmlNode friendsNode = result.SelectSingleNode("Friends");
                XmlNodeList friendNodes = friendsNode.ChildNodes;

                if (friendNodes.Count > 0)
                {
                    Friends = new PostboxDevicePackage[friendNodes.Count];

                    for (int i = 0; i < Friends.Length; i++)
                    {
                        // create object
                        Friends[i] = new PostboxDevicePackage(friendNodes[i].Attributes.GetNamedItem("public_device_id").InnerText,
                                                                    friendNodes[i].Attributes.GetNamedItem("model").InnerText,
                                                                    friendNodes[i].Attributes.GetNamedItem("type").InnerText,
                                                                    friendNodes[i].Attributes.GetNamedItem("os").InnerText,
                                                                    friendNodes[i].Attributes.GetNamedItem("created_at").InnerText,
                                                                    friendNodes[i].Attributes.GetNamedItem("status").InnerText);
                    }
                }
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PostboxResponse"/> class via JSONObject.
        /// </summary>
        /// <param name="localRequestId">localRequest identifier.</param>
        /// <param name="callName">callname of request</param>
        /// <param name="response">response object.</param>
        public PostboxGetFriendlistResponse(string localRequestId, PostboxCallName callName, JSONObject response) : base(localRequestId, callName, response)
        {
            if (response == null)
                return;

            JSONObject result = response.GetField("Result");

            if (result != null)
            {
                // --- Friends ---
                JSONObject requestsNode = result.GetField("Friends");

                if(requestsNode != null & requestsNode.IsArray)
                {
                    List<JSONObject> requestNodes = requestsNode.list;

                    if (requestNodes != null && requestNodes.Count > 0)
                    {
                        Friends = new PostboxDevicePackage[requestNodes.Count];

                        for (int i = 0; i < Friends.Length; i++)
                        {
                            JSONObject request = requestNodes[i].GetField("Friend");

                            // create object
                            Friends[i] = new PostboxDevicePackage(request.GetField("public_device_id").str,
                                                                 request.GetField("model").str,
                                                                 request.GetField("type").str,
                                                                 request.GetField("os").str,
                                                                 request.GetField("created_at").str,
                                                                 request.GetField("status").str);
                        }
                    }
                }
            }
        }
    }    
}