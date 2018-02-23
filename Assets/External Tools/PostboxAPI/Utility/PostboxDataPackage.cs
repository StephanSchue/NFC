namespace PostboxAPI
{
    using System;
    using System.Xml;

    /// <summary>
    /// This Class is used for prepare the Data of the GetDataPackage Response for the user.
    /// </summary>
    public class PostboxDataPackage
    {
        /// <summary>
        /// Identifer of the package
        /// </summary>
        public string TransactionId { get; private set; }

        /// <summary>
        /// App identifier of sender
        /// </summary>
        public string SenderAppId { get; private set; }

        /// <summary>
        /// Device identifier of sender
        /// </summary>
        public string SenderDeviceId { get; private set; }

        /// <summary>
        /// Time when the package was created on server
        /// </summary>
        public DateTime CreatedAt { get; private set; }

        /// <summary>
        /// Time of LastUpdate of the package
        /// </summary>
        public DateTime UpdatedAt { get; private set; }

        /// <summary>
        /// Current status of the package <see cref="PostboxDataPackageStatus"/>.
        /// </summary>
        public PostboxDataPackageStatus Status { get; private set; }

        /// <summary>
        /// RawData of the package
        /// </summary>
        public string Data { get; private set; }

        /// <summary>
        /// XMLData of the package
        /// </summary>
        public XmlDocument XMLData { get; private set; }

        /// <summary>
        /// JSONData of the package
        /// </summary>
        public JSONObject JSONData { get; private set; }

        /// <summary>
        /// The data is from type text
        /// </summary>
        public bool IsText { get; private set; }

        /// <summary>
        /// The data is from type xml
        /// </summary>
        public bool IsXML { get; private set; }

        /// <summary>
        /// The data is from type json
        /// </summary>
        public bool IsJSON { get; private set; }

        /// <summary>
        /// Additional parameters that given to the package
        /// </summary>
        public PostboxCallParameter[] Parameters;


        /// <summary>
        /// Constructor of the PostboxDataPackage
        /// </summary>
        /// <param name="transactionId">transactionId of DataPackage</param>
        /// <param name="senderAppId">AppID of sender</param>
        /// <param name="senderDeviceId">DeviceID of sender</param>
        /// <param name="createdAt">Serverdate of creating the package</param>
        /// <param name="updatedAt">Serverdate of the last update of the package</param>
        /// <param name="status">current Status of the package</param>
        /// <param name="data">data of the package</param>
        /// <param name="parameters">Additional Parameters</param>
        public PostboxDataPackage(string transactionId, string senderAppId, string senderDeviceId, string createdAt, string updatedAt, string status, string data, params PostboxCallParameter[] parameters)
        {
            TransactionId = transactionId;
            SenderAppId = senderAppId;
            SenderDeviceId = senderDeviceId;
            CreatedAt = DateTime.Parse(createdAt);
            UpdatedAt = DateTime.Parse(updatedAt);
            Status = StatusStringToEnum(status);
            Data = data;

            // Check if XMLData and Decode
            PostboxXMLCreator xmlCreator = new PostboxXMLCreator();
            XMLData = xmlCreator.Decode(data); 
            if(XMLData != null) {IsXML = true;}

            // Check if JSONData and Decode
            PostboxJSONCreator jsonCreator = new PostboxJSONCreator();
            JSONData = jsonCreator.Decode(data);
            if(JSONData != null) {IsJSON = true;}

            if(!IsXML && !IsJSON)
            {
                IsText = true;
            }

            Parameters = parameters;
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