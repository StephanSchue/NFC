using System;
using System.Collections.Generic;
using System.Xml;

namespace PostboxAPI
{
    /// <summary>
    /// The base class for Responses
    /// </summary>
    public class PostboxResponse
    {
        // --- Global ---
        /// <summary>
        /// The identifier of the call
        /// </summary>
        public PostboxCallName CallName { get; private set; }

        /// <summary>
        /// The response status of processing the call
        /// </summary>
        public PostboxCallStatus CallStatus { get; private set; }

        /// <summary>
        /// The version of the API that was used
        /// </summary>
        public string Version { get; private set; }

        /// <summary>
        /// A list of Erros that will be filled if the CallStatus is Error
        /// </summary>
        public Dictionary<PostboxAPIErrorCode,PostboxAPIError> Errors = new Dictionary<PostboxAPIErrorCode, PostboxAPIError>();

        // --- Params ---
        /// <summary>
        /// The identifier of the data manager in the connector
        /// </summary>
        public string LocalRequestId { get; private set; }

        // --- Helper ---

        /// <summary>
        /// The raw XML-Object of the response data
        /// </summary>
        public XmlDocument XMLObject { get; private set; }

        /// <summary>
        /// The raw JSON-Object of the response data
        /// </summary>
        public JSONObject JSONObject { get; private set; }


        /// <summary>
        /// Initializes a new instance of the <see cref="PostboxResponse"/> class.
        /// </summary>
        /// <param name="localRequestId">local transaction identifier.</param>
        /// <param name="callName">callname of request</param>
        /// <param name="response">XmlDocument</param>
        public PostboxResponse(string localRequestId, PostboxCallName callName, XmlDocument response)
        {
            LocalRequestId = localRequestId;
            CallName = callName;
            XMLObject = response;

            if (response != null)
            {
                // PostboxGlobal-Node
                XmlNode postBoxGlobal = response.SelectSingleNode("PostBoxGlobal");

                if (postBoxGlobal != null)
                {
                    XmlNode callNamenNode = postBoxGlobal.SelectSingleNode("CallName");
                    CallName = CallNameStringToEnum(callNamenNode.InnerText);

                    XmlNode callStatusNode = postBoxGlobal.SelectSingleNode("CallStatus");
                    CallStatus = CallStatusStringToEnum(callStatusNode.InnerText);

                    XmlNode versionNode = postBoxGlobal.SelectSingleNode("Version");
                    Version = versionNode.InnerText;

                    // Error-Node
                    XmlNode errorList = postBoxGlobal.SelectSingleNode("ErrorList");

                    if (errorList != null)
                    {
                        XmlNodeList errorNodes = errorList.ChildNodes;

                        if (errorNodes.Count > 0)
                        {
                            Errors = new Dictionary<PostboxAPIErrorCode, PostboxAPIError>(errorNodes.Count);

                            for (int i = 0; i < errorNodes.Count; i++)
                            {
                                XmlNode errorCodeNode = errorNodes[i].SelectSingleNode("ErrorCode");
                                XmlNode errorDescriptionNode = errorNodes[i].SelectSingleNode("ErrorDescription");
                                XmlNode errorLongDescriptionNode = errorNodes[i].SelectSingleNode("ErrorLongDescription");

                                PostboxAPIError apiError = new PostboxAPIError(errorCodeNode.InnerText,
                                                                errorDescriptionNode.InnerText,
                                                                errorLongDescriptionNode.InnerText);

                                Errors.Add(apiError.ErrorCode, apiError);
                            }
                        }
                    }
                }
            }
            else
            {
                // If result is Empty
                CreateNoConnectionError();
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PostboxResponse"/> class.
        /// </summary>
        /// <param name="localRequestId">local transaction identifier.</param>
        /// <param name="callName">callname of request</param>
        /// <param name="response">JSONObject</param>
        public PostboxResponse(string localRequestId, PostboxCallName callName, JSONObject response)
        {
            LocalRequestId = localRequestId;
            CallName = callName;
            JSONObject = response;

            if (response != null)
            {
                // PostboxGlobal-Node
                JSONObject postBoxGlobal = response;

                if (postBoxGlobal != null)
                {
                    CallName = CallNameStringToEnum(postBoxGlobal.GetField("CallName").str);
                    CallStatus = CallStatusStringToEnum(postBoxGlobal.GetField("CallStatus").str);
                    Version = postBoxGlobal.GetField("Version").str;
                }

                // Error-Node
                JSONObject errorListNode = response.GetField("ErrorList");

                if (errorListNode != null && errorListNode.IsArray)
                {
                    List<JSONObject> errorList = errorListNode.list;

                    if (errorList.Count > 0)
                    {
                        Errors = new Dictionary<PostboxAPIErrorCode, PostboxAPIError>(errorList.Count);

                        for (int i = 0; i < errorList.Count; i++)
                        {
                            JSONObject error = errorList[i].GetField("Error");

                            if(error != null)
                            {
                                JSONObject errorCodeNode = error.GetField("ErrorCode");
                                JSONObject errorDescriptionNode = error.GetField("ErrorDescription");
                                JSONObject errorLongDescriptionNode = error.GetField("ErrorLongDescription");

                                PostboxAPIError apiError = new PostboxAPIError(errorCodeNode.i.ToString(),
                                                                                errorDescriptionNode.str,
                                                                                errorLongDescriptionNode.str);
                                Errors.Add(apiError.ErrorCode, apiError);
                            }
                        }
                    }
                }
            }
            else
            {
                // If result is Empty
                CreateNoConnectionError();
            }
        }

        /// <summary>
        /// Assign the call status of the given string
        /// </summary>
        /// <param name="callStatus">Server callstatus string</param>
        /// <returns>PostboxCallStatus</returns>
        public static PostboxCallStatus CallStatusStringToEnum(string callStatus)
        {
            PostboxCallStatus output = PostboxCallStatus.Undefined;

            if (Enum.IsDefined(typeof(PostboxCallStatus), callStatus))
            {
                output = (PostboxCallStatus)Enum.Parse(typeof(PostboxCallStatus), callStatus);
            }
            else
            {
                PostboxLogbook.Instance.Log("CallStatus '" + callStatus + "' can't be converted. Not vaild.", PostboxLogbook.NotificationType.Error);
            }

            return output;
        }

        /// <summary>
        /// Assign the call name of the given string
        /// </summary>
        /// <param name="callName">callname string</param>
        /// <returns>PostboxCallName</returns>
        public static PostboxCallName CallNameStringToEnum(string callName)
        {
            PostboxCallName output = PostboxCallName.NotFound;

            if (Enum.IsDefined(typeof(PostboxCallName), callName))
            {
                output = (PostboxCallName)Enum.Parse(typeof(PostboxCallName), callName);
            }
            else
            {
                PostboxLogbook.Instance.Log("CallName '" + callName + "' can't be converted. Not vaild.", PostboxLogbook.NotificationType.Error);
            }

            return output;
        }

        /// <summary>
        /// Set the no Connection Error to the Response
        /// </summary>
        private void CreateNoConnectionError()
        {
            CallStatus = PostboxCallStatus.Error;
            Errors = new Dictionary<PostboxAPIErrorCode, PostboxAPIError>(1);
            Errors.Add(PostboxAPIErrorCode.no_sever_connection, new PostboxAPIError("50", "No response from the server", "No response from the server. Please check your connection."));
        }

        #region Functions for errors

        /// <summary>
        /// Check if an specific errorcode is in response
        /// </summary>
        /// <param name="errorCode">errorcode for searching</param>
        /// <returns>Boolean for error existens</returns>
        public bool CheckErrorCode(PostboxAPIErrorCode errorCode)
        {
            PostboxAPIError error = null;

            if(Errors.TryGetValue(errorCode, out error))
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// Check if an specific errorcode is in response and get error as return
        /// </summary>
        /// <param name="errorCode">errorcode for searching</param>
        /// <param name="error">the requested APIError</param>
        /// <returns>Boolean for error existens</returns>
        public bool CheckErrorCode(PostboxAPIErrorCode errorCode, out PostboxAPIError error)
        {
            if (Errors.TryGetValue(errorCode, out error))
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// Log an specific error if it exists in response
        /// </summary>
        /// <param name="errorCode">errorcode for searching</param>
        public void LogAPIError(PostboxAPIErrorCode errorCode)
        {
            PostboxAPIError error = null;
            if (CheckErrorCode(errorCode, out error))
            {
                PostboxLogbook.Instance.Log(error);
            }
        }

        /// <summary>
        /// Log all error of the response
        /// </summary>
        public void LogAllAPIErrors()
        {
            if (Errors.Count > 0)
            {
                foreach (KeyValuePair<PostboxAPIErrorCode, PostboxAPIError> errorEntry in Errors)
                {
                    PostboxLogbook.Instance.Log(errorEntry.Value);
                }
            }
        }

        #endregion
    }
}