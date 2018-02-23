using System;
using System.Xml;

namespace PostboxAPI
{
    /// <summary>
    /// Response class of "GetDataPackageStatus" call. 
    /// Returns the status an specific package in "Status"
    /// Also returns CallStatus that is Success or Error.
    /// Errors could be found in the Errors variable.
    /// </summary>
    public class PostboxGetDataPackageStatusResponse : PostboxResponse
    {
        /// <summary>
        /// Current package status <see cref="PostboxDataPackageStatus"/>.
        /// </summary>
        public PostboxDataPackageStatus Status { get; private set; }

        /// <summary>
        /// Additional message of package status.
        /// </summary>
        public string Notification { get; private set; }


        /// <summary>
        /// Initializes a new instance of the <see cref="PostboxResponse"/> class via XmlDocument.
        /// </summary>
        /// <param name="localRequestId">localRequest identifier.</param>
        /// <param name="callName">callname of request</param>
        /// <param name="response">response object.</param>
        public PostboxGetDataPackageStatusResponse(string localRequestId, PostboxCallName callName, XmlDocument response) : base(localRequestId, callName, response)
        {
            if (response == null)
                return;

            XmlNode result = response.SelectSingleNode("/PostBoxGlobal/Result");

            if (result != null)
            {
                XmlNode statusNode = result.SelectSingleNode("Status");
                Status = StatusStringToEnum(statusNode.InnerText);

                XmlNode notificationNode = result.SelectSingleNode("Notification");
                Notification = notificationNode.InnerText;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PostboxResponse"/> class via JSONObject.
        /// </summary>
        /// <param name="localRequestId">localRequest identifier.</param>
        /// <param name="callName">callname of request</param>
        /// <param name="response">response object.</param>
        public PostboxGetDataPackageStatusResponse(string localRequestId, PostboxCallName callName, JSONObject response) : base(localRequestId, callName, response)
        {
            if (response == null)
                return;

            JSONObject result = response.GetField("Result");

            if (result != null)
            {
                Status = StatusStringToEnum(result.GetField("Status").str);

                if(result.GetField("Notification") != null)
                { 
                    Notification = result.GetField("Notification").str;
                }
            }
        }

        /// <summary>
        /// Convert PackageStatus and return Enum
        /// </summary>
        /// <param name="status">Status of DataPackage Response</param>
        /// <returns>PostboxDataPackageStatus</returns>
        public static PostboxDataPackageStatus StatusStringToEnum(string status)
        {
            PostboxDataPackageStatus output = PostboxDataPackageStatus.error;

            if (Enum.IsDefined(typeof(PostboxDataPackageStatus), status))
            {
                output = (PostboxDataPackageStatus)Enum.Parse(typeof(PostboxDataPackageStatus), status);
            }
            else
            {
                PostboxLogbook.Instance.Log("PostboxDataPackageStatus '" + status + "' can't be converted. Not vaild.", PostboxLogbook.NotificationType.Error);
            }

            return output;
        }
    }
}