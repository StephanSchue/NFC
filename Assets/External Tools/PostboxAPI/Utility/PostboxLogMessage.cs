namespace PostboxAPI
{
    /// <summary>
    /// This Class represents the LogMessage that is used for the PostboxLogbook.
    /// It contains a message, the time of notification and the notification level.
    /// </summary>
    public class PostboxLogMessage
    {
        /// <summary>
        /// Description of the event
        /// </summary>
        public string Message { get; private set; }

        /// <summary>
        /// Time of the event
        /// </summary>
        public System.DateTime Time { get; private set; }

        /// <summary>
        /// Type of message <see cref="PostboxLogbook.NotificationType" />.
        /// </summary>
        public PostboxLogbook.NotificationType NotificationLevel { get; private set; }

        /// <summary>
        /// Constructor to setup an new LogMessage
        /// </summary>
        /// <param name="message">Message that will be logged</param>
        /// <param name="time">The time of the event</param>
        /// <param name="notificationLevel">The level of Notification</param>
        public PostboxLogMessage(string message, System.DateTime time, PostboxLogbook.NotificationType notificationLevel)
        {
            NotificationLevel = notificationLevel;
            Time = time;
            Message = message;
        }

        /// <summary>
        /// Output an formated string of the LogMessage
        /// </summary>
        /// <returns>Formatted String</returns>
        public string Print()
        {
            return System.String.Format("[{0}] {1}: {2}", Time.ToString("HH:mm:ss"), NotificationLevel.ToString(), Message);
        }
    }
}