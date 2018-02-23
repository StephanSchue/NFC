using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using PostboxAPI;

public class NFCSuperHeadball : NFCContainerBase
{
    public GameObject world;
    public InputField inputField;
    public Text statusField;

    public override void Initialize(NFCController.AmiiboData amiibo)
    {
        base.Initialize(amiibo);
        world.SetActive(true);
    }

    public override void Deinitialize()
    {
        world.SetActive(false);
    }

    public void SendCode()
    {
        string deviceID = inputField.text;
        PostboxAPIUnityConnector.Instance.SendDataPackageToDevice(deviceID, amiibo.id, ReceiveDeliveryStatus);
    }

    private void ReceiveDeliveryStatus(PostboxAPI.PostboxSendDataPackageToDeviceResponse response)
    {
        statusField.text = response.CallStatus.ToString();
    }
}
