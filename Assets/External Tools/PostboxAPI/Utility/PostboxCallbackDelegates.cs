namespace PostboxAPI
{
    #region Callback-Delegates

    /// <summary>
    /// Callback to get the DeviceID of the current Device.
    /// </summary>
    /// <param name="deviceID">Device Identifier</param>
    public delegate void PostboxDeviceIDCallback(string deviceID);

    /// <summary>
    /// Callback for all API-Calls.
    /// </summary>
    /// <typeparam name="T">Type of PostboxResponse child class</typeparam>
    /// <param name="response"></param>
    public delegate void PostboxResponseCallback<T>(T response) where T : PostboxResponse;

    /// <summary>
    /// Callback for tasks after the APICall used internal
    /// </summary>
    /// <typeparam name="T">Type of PostboxResponse child class</typeparam>
    /// <param name="response">PostboxResponse</param>
    /// <param name="userCallback">Callback of user</param>
    /// <param name="timeout">max time in seconds, that will wait for the operation</param>
    /// <param name="parameters">additional parameters</param>
    public delegate void PostboxDelayCallback<T>(PostboxResponse response, PostboxResponseCallback<T> userCallback, int timeout, params PostboxCallParameter[] parameters) where T : PostboxResponse;

    #endregion
}