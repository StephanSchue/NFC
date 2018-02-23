namespace PostboxAPI
{
    /// <summary>
    /// Interface for Request/Response OutputCreator
    /// </summary>
    public interface IPostboxOutputCreator
    {
        /// <summary>
        /// Prepair an PostboxRequest for sending via the connector
        /// </summary>
        /// <param name="callName">Callname for API Service</param>
        /// <param name="parameters">Additional Parameters</param>
        string CreateRequest(string callName, params PostboxCallParameter[] parameters);

        /// <summary>
        /// Return the given string with indent and line breaks
        /// </summary>
        /// <param name="input">String in specific data format for OutputCreator</param>
        /// <returns>Formated string</returns>
        string FormatString(string input);
    }
}