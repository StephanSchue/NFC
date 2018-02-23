namespace PostboxAPI
{
    /// <summary>
    /// Class for Call Parameters used for creation api request
    /// </summary>
    public class PostboxCallParameter
    {
        /// <summary>
        /// The Key of the Parameter
        /// </summary>
        public string Key { get; set; }

        /// <summary>
        /// The Parameter Value
        /// </summary>
        public string Value { get; set; }

        /// <summary>
        /// Set an call parameter.
        /// </summary>
        /// <param name="key">The Key of the Parameter</param>
        /// <param name="value">The Parameter Value</param>
        public PostboxCallParameter(string key, string value)
        {
            Key = key;
            Value = value;
        }
    }
}