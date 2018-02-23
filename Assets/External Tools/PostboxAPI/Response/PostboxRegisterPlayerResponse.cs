using System;
using System.Xml;
using System.Collections;
using System.Collections.Generic;

namespace PostboxAPI
{
    /// <summary>
    /// Response class of "RegisterPlayer" call. 
    /// Returns the CallStatus that is Success or Error.
    /// Errors could be found in the Errors Variable.
    /// </summary>
    public class PostboxRegisterPlayerResponse : PostboxResponse
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PostboxResponse"/> class via XmlDocument.
        /// </summary>
        /// <param name="localRequestId">localRequest identifier.</param>
        /// <param name="callName">callname of request</param>
        /// <param name="response">response object.</param>
        public PostboxRegisterPlayerResponse(string localRequestId, PostboxCallName callName, XmlDocument response) : base(localRequestId, callName, response)
        {
            if (response == null)
                return;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PostboxResponse"/> class via JSONObject.
        /// </summary>
        /// <param name="localRequestId">localRequest identifier.</param>
        /// <param name="callName">callname of request</param>
        /// <param name="response">response object.</param>
        public PostboxRegisterPlayerResponse(string localRequestId, PostboxCallName callName, JSONObject response) : base(localRequestId, callName, response)
        {
            if (response == null)
                return;
        }
    }
}