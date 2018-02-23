using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using PostboxAPI;

public class SendMessageToDevice : MonoBehaviour
{
    public InputField DeviceIdInput;
    public InputField DeviceMessage;

    public void SendDeviceMessage()
    {
        if(!System.String.IsNullOrEmpty(DeviceIdInput.text) && !System.String.IsNullOrEmpty(DeviceMessage.text))
        {
            PostboxAPIUnityConnector.Instance.SendDataPackageToDevice(DeviceIdInput.text, DeviceMessage.text, SendDeviceMessageCallback);
        }
    }

    public void SendDeviceMessageCallback(PostboxSendDataPackageToDeviceResponse response)
    {
        switch (response.CallStatus)
        {
            case PostboxCallStatus.Success:
                Debug.Log("Message successfull sended!");
                break;
            case PostboxCallStatus.Error:
                response.LogAllAPIErrors();
                break;
            default:
                break;
        }
    }
}

