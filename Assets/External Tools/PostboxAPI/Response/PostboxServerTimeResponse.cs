using System;
using System.Xml;

namespace PostboxAPI
{
    /// <summary>
    /// Response class of "GetServerTime" call, that returns an Timestamp and an UniversalTimeStamp
    /// </summary>
    public class PostboxServerTimeResponse : PostboxResponse
    {
        // --- Return Values ---

        /// <summary>
        /// Timestamp in local time of the server
        /// </summary>
        public DateTime? Timestamp { get; private set; }

        /// <summary>
        /// Timestamp in universal time of the server
        /// </summary>
        public DateTime? UniversalTimeStamp { get; private set; }


        /// <summary>
        /// Initializes a new instance of the <see cref="PostboxResponse"/> class via XmlDocument.
        /// </summary>
        /// <param name="localRequestId">localRequest identifier.</param>
        /// <param name="callName">callname of request</param>
        /// <param name="response">response object.</param>
        public PostboxServerTimeResponse(string localRequestId, PostboxCallName callName, XmlDocument response) : base(localRequestId, callName, response)
        {
            if (response == null)
                return;

            XmlNode result = response.SelectSingleNode("/PostBoxGlobal/Result");

            if (result != null)
            {
                try
                {
                    XmlNode timeStampNode = result.SelectSingleNode("TimeStamp");
                    Timestamp = Convert.ToDateTime(timeStampNode.InnerText);
                }
                catch (FormatException ex)
                {
                    Timestamp = null;
                }

                try
                {
                    XmlNode universalTimeStampNode = result.SelectSingleNode("UniversalTimeStamp");
                    UniversalTimeStamp = Convert.ToDateTime(universalTimeStampNode.InnerText);
                }
                catch (FormatException ex)
                {
                    UniversalTimeStamp = null;
                } 
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PostboxResponse"/> class via JSONObject.
        /// </summary>
        /// <param name="localRequestId">localRequest identifier.</param>
        /// <param name="callName">callname of request</param>
        /// <param name="response">response object.</param>
        public PostboxServerTimeResponse(string localRequestId, PostboxCallName callName, JSONObject response) : base(localRequestId, callName, response)
        {
            if (response == null)
                return;

            JSONObject result = response.GetField("Result");

            if (result != null)
            {
                try
                {
                    Timestamp = Convert.ToDateTime(result.GetField("TimeStamp").str);
                }
                catch (FormatException ex)
                {
                    Timestamp = null;
                }

                try
                {
                    UniversalTimeStamp = Convert.ToDateTime(result.GetField("UniversalTimeStamp").str);
                }
                catch (FormatException ex)
                {
                    UniversalTimeStamp = null;
                }                
            }
        }
    }
}