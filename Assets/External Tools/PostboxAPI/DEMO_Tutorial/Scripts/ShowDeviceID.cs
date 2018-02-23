using UnityEngine;
using UnityEngine.UI;
using PostboxAPI;

public class ShowDeviceID : MonoBehaviour
{
    public InputField DeviceIdLabel;

	// Use this for initialization
	private void Awake ()
    {
        CallDeviceID();
    }

    public void CallDeviceID()
    {
        PostboxAPIUnityConnector.Instance.GetDeviceID(SetDeviceIdToLabel);
    }

    public void SetDeviceIdToLabel(string deviceId)
    {
        DeviceIdLabel.text = deviceId;
    }
}


