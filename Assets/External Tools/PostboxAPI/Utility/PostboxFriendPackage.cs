namespace PostboxAPI
{
    using System;

    /// <summary>
    /// This Class is used for prepare the Data of the GetFriendlist Response for the user.
    /// </summary>
    public struct PostboxDevicePackage
    {
        /// <summary>
        /// The identifier of the device for the api
        /// </summary>
        public string DeviceId { get; private set; }

        /// <summary>
        /// The model of device. Like: "Samsung Galaxy S4"
        /// </summary>
        public string DeviceModel { get; private set; }

        /// <summary>
        /// The type of device. Like: "standalone", "handeld", aso.
        /// </summary>
        public string DeviceType { get; private set; }

        /// <summary>
        /// The device operating system
        /// </summary>
        public string DeviceOS { get; private set; }

        /// <summary>
        /// Time where device is registered on the server
        /// </summary>
        public DateTime CreatedAt { get; private set; }

        /// <summary>
        /// Status of device. 1=activ; 0=inactiv;
        /// </summary>
        public int Status { get; private set; }


        /// <summary>
        /// Constructor of the PostboxFriendPackage
        /// </summary>
        /// <param name="deviceId">Unique ID of the device</param>
        /// <param name="deviceModel">Model of the device</param>
        /// <param name="deviceType">Type of the device</param>
        /// <param name="deviceOS">OS of the device</param>
        /// <param name="createdAt">Date of registration of the device</param>
        /// <param name="status">Status of the device. 1=activ, 0=inactiv</param>
        public PostboxDevicePackage(string deviceId, string deviceModel, string deviceType, string deviceOS, string createdAt, string status)
        {
            DeviceId = deviceId;
            DeviceModel = deviceModel;
            DeviceType = deviceType;
            DeviceOS = deviceOS;
            CreatedAt = DateTime.Parse(createdAt);
            Status = Convert.ToInt32(status);
        }
    }
}