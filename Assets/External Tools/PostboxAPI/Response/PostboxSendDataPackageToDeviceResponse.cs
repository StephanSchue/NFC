using System;
using System.Xml;
using System.Collections;
using System.Collections.Generic;

namespace PostboxAPI
{
    /// <summary>
    /// Response class of "SendDataPackageToDevice" call. 
    /// Returns the TransactionId of the Created Package and the CallStatus that is Success or Error.
    /// Errors could be found in the Errors Variable.
    /// </summary>
    public class PostboxSendDataPackageToDeviceResponse : PostboxResponse
    {
        /// <summary>
        /// Identifier of the created Package. That is importent for later calls
        /// </summary>
        public string TransactionId { get; private set; }


        /// <summary>
        /// Initializes a new instance of the <see cref="PostboxResponse"/> class via XmlDocument.
        /// </summary>
        /// <param name="localRequestId">localRequest identifier.</param>
        /// <param name="callName">callname of request</param>
        /// <param name="response">response object.</param>
        public PostboxSendDataPackageToDeviceResponse(string localRequestId, PostboxCallName callName, XmlDocument response) : base(localRequestId, callName, response)
        {
            if (response == null)
                return;

            XmlNode result = response.SelectSingleNode("/PostBoxGlobal/Result");

            if (result != null)
            {
                XmlNode transactionIdNode = result.SelectSingleNode("TransactionId");
                TransactionId = transactionIdNode.InnerText;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PostboxResponse"/> class via JSONObject.
        /// </summary>
        /// <param name="localRequestId">localRequest identifier.</param>
        /// <param name="callName">callname of request</param>
        /// <param name="response">response object.</param>
        public PostboxSendDataPackageToDeviceResponse(string localRequestId, PostboxCallName callName, JSONObject response) : base(localRequestId, callName, response)
        {
            if (response == null)
                return;

            JSONObject result = response.GetField("Result");

            if (result != null)
            {
                TransactionId = result.GetField("TransactionId").str;
            }
        }
    }
}