using System.Xml;

namespace PostboxAPI
{
    /// <summary>
    /// Response class of "UpdateDataPackageStatus" call. 
    /// Returns the CallStatus that is Success or Error.
    /// Errors could be found in the Errors Variable.
    /// </summary>
    public class PostboxUpdateDataPackageResponse : PostboxResponse
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PostboxResponse"/> class via XmlDocument.
        /// </summary>
        /// <param name="localRequestId">localRequest identifier.</param>
        /// <param name="callName">callname of request</param>
        /// <param name="response">response object.</param>
        public PostboxUpdateDataPackageResponse(string localRequestId, PostboxCallName callName, XmlDocument response) : base(localRequestId, callName, response)
        {
            if (response == null)
                return;

            XmlNode result = response.SelectSingleNode("/PostboxGlobal/Result");
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PostboxResponse"/> class via JSONObject.
        /// </summary>
        /// <param name="localRequestId">localRequest identifier.</param>
        /// <param name="callName">callname of request</param>
        /// <param name="response">response object.</param>
        public PostboxUpdateDataPackageResponse(string localRequestId, PostboxCallName callName, JSONObject response) : base(localRequestId, callName, response)
        {
            if (response == null)
                return;

            JSONObject result = response.GetField("Result");
        }
    }
}