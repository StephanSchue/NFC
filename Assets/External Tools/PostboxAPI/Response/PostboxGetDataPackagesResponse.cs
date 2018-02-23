using System;
using System.Xml;
using System.Collections;
using System.Collections.Generic;

namespace PostboxAPI
{
    /// <summary>
    /// Response class of "GetDataPackages" call. 
    /// Returns an Array of "packages" that the user requested.
    /// Also returns CallStatus that is Success or Error.
    /// Errors could be found in the Errors variable.
    /// </summary>
    public class PostboxGetDataPackagesResponse : PostboxResponse
    {
        /// <summary>
        /// List of <see cref="PostboxDataPackage"/>.
        /// </summary>
        public PostboxDataPackage[] packages;

        /// <summary>
        /// Initializes a new instance of the <see cref="PostboxResponse"/> class via XmlDocument.
        /// </summary>
        /// <param name="localRequestId">localRequest identifier.</param>
        /// <param name="callName">callname of request</param>
        /// <param name="response">response object.</param>
        public PostboxGetDataPackagesResponse(string localRequestId, PostboxCallName callName, XmlDocument response) : base(localRequestId, callName, response)
        {
            if (response == null)
                return;

            XmlNode result = response.SelectSingleNode("/PostBoxGlobal/Result");

            if (result != null)
            {
                XmlNodeList requestNodes = result.ChildNodes;

                if (requestNodes.Count > 0)
                {
                    packages = new PostboxDataPackage[requestNodes.Count];

                    for (int i = 0; i < packages.Length; i++)
                    {
                        // list parameters
                        //XmlNodeList parameterNodes = requestNodes[i].SelectSingleNode("Params").ChildNodes;
                        //PostboxCallParameter[] parameters = new PostboxCallParameter[parameterNodes.Count];
                        /*
                        for (int y = 0; y < parameterNodes.Count; y++)
                        {
                            parameters[y] = new PostboxCallParameter(parameterNodes.Item(y).Name, parameterNodes.Item(y).InnerText);
                        }*/

                        // create object
                        packages[i] = new PostboxDataPackage(requestNodes[i].SelectSingleNode("TransactionId").InnerText,
                                                            requestNodes[i].SelectSingleNode("SenderAppId").InnerText,
                                                            requestNodes[i].SelectSingleNode("SenderDeviceId").InnerText,
                                                            requestNodes[i].SelectSingleNode("CreatedAt").InnerText,
                                                            requestNodes[i].SelectSingleNode("UpdatedAt").InnerText,
                                                            requestNodes[i].SelectSingleNode("Status").InnerText,
                                                            requestNodes[i].SelectSingleNode("Data").InnerText);
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
        public PostboxGetDataPackagesResponse(string localRequestId, PostboxCallName callName, JSONObject response) : base(localRequestId, callName, response)
        {
            if (response == null)
                return;

            JSONObject result = response.GetField("Result");

            if (result != null)
            {
                List<JSONObject> requestNodes = result.list;

                if (requestNodes != null && requestNodes.Count > 0)
                {
                    packages = new PostboxDataPackage[requestNodes.Count];

                    for (int i = 0; i < packages.Length; i++)
                    {
                        JSONObject request = requestNodes[i].GetField("Request");

                        // create object
                        packages[i] = new PostboxDataPackage(request.GetField("TransactionId").str,
                                                             request.GetField("SenderAppId").str,
                                                             request.GetField("SenderDeviceId").str,
                                                             request.GetField("CreatedAt").str,
                                                             request.GetField("UpdatedAt").str,
                                                             request.GetField("Status").str,
                                                             request.GetField("Data").str);
                    }
                }
            }
        }
    }
}