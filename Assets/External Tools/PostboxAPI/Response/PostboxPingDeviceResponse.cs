using System;
using System.Xml;

namespace PostboxAPI
{
    /// <summary>
    /// Response class of "PingResponse" call. 
    /// Returns the PingStatus that can get to values:
    /// - server = The server receive the request
    /// - device = The device receive the ping
    /// Also returns CallStatus that is Success or Error.
    /// Errors could be found in the Errors Variable.
    /// </summary>
    public class PostboxPingDeviceResponse : PostboxResponse
    {
        /// <summary>
        ///  Status of the ping request
        /// - server = The server receive the request
        /// - device = The device receive the ping
        /// </summary>
        public PostboxPingStatus PingStatus { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="PostboxResponse"/> class via XmlDocument.
        /// </summary>
        /// <param name="localRequestId">localRequest identifier.</param>
        /// <param name="callName">callname of request</param>
        /// <param name="response">response object.</param>
        public PostboxPingDeviceResponse(string localRequestId, PostboxCallName callName, XmlDocument response) : base(localRequestId, callName, response)
        {
            if (response == null)
                return;

            XmlNode result = response.SelectSingleNode("/PostBoxGlobal/Result");

            if (result != null)
            {
                switch (result.InnerText)
                {
                    case "server":
                        PingStatus = PostboxPingStatus.server;
                        break;
                    case "device":
                        PingStatus = PostboxPingStatus.device;
                        break;
                    default:
                        break;
                }
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PostboxResponse"/> class via JSONObject.
        /// </summary>
        /// <param name="localRequestId">localRequest identifier.</param>
        /// <param name="callName">callname of request</param>
        /// <param name="response">response object.</param>
        public PostboxPingDeviceResponse(string localRequestId, PostboxCallName callName, JSONObject response) : base(localRequestId, callName, response)
        {
            if (response == null)
                return;
        }
    }
}