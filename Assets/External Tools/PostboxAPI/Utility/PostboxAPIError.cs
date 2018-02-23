using System;

namespace PostboxAPI
{ 
    /// <summary>
    /// Representation of API Error Object for better usage in the plugin
    /// </summary>
    public class PostboxAPIError
    {
        /// <summary>
        /// Identifier of the error
        /// </summary>
        public PostboxAPIErrorCode ErrorCode { get; private set; }

        /// <summary>
        /// Short description of the API Error
        /// </summary>
        public string ErrorDescription { get; private set; }

        /// <summary>
        /// Long description of the API Error
        /// </summary>
        public string ErrorLongDescription { get; private set; }


        /// <summary>
        /// Constructor of the APIError Object
        /// </summary>
        /// <param name="errorCode">Server ErrorCode</param>
        /// <param name="errorDescription">Short Error Description</param>
        /// <param name="errorLongDescription">Long Error Description</param>
        public PostboxAPIError(string errorCode, string errorDescription, string errorLongDescription)
        {
            ErrorCode = (PostboxAPIErrorCode)Convert.ToInt32(errorCode);
            ErrorDescription = errorDescription;
            ErrorLongDescription = errorLongDescription;
        }
    }
}