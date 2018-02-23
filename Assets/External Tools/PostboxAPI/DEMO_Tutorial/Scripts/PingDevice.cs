using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using PostboxAPI;

public class PingDevice : MonoBehaviour
{
    public InputField DeviceIdInput;

    // Use this for initialization
    private void Start ()
    {
        //PostboxAPIUnityConnector.instance.GetDeviceID(PingAnDevice);
    }

    public void PingAnDeviceFromInputfield()
    {
        if(!System.String.IsNullOrEmpty(DeviceIdInput.text))
        {
            PingAnDevice(DeviceIdInput.text);
        }
    }

    public void PingAnDevice(string deviceId)
    {
        PostboxAPIUnityConnector.Instance.PingDevice(deviceId, PingCallback);
    }

    public void PingCallback(PostboxPingDeviceResponse response)
    {
        switch (response.CallStatus)
        {
            case PostboxCallStatus.Success:

                switch (response.PingStatus)
                {
                    case PostboxPingStatus.server:
                        Debug.Log("Ping ist noch nicht abgerufen.");
                        break;
                    case PostboxPingStatus.device:
                        Debug.Log("Ping erfolgreich!");
                        break;
                    default:
                        break;
                }

                break;
            case PostboxCallStatus.Error:
                response.LogAllAPIErrors();
                break;
            default:
                break;
        }
    }

}
