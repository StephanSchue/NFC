using UnityEngine;
using System.Collections;
using PostboxAPI;

public class AutoPing : MonoBehaviour
{
    private bool status = true;

    public void ToggleStatus()
    {
        status = !status;
        if (status)
        {
            PostboxAPIUnityConnector.Instance.StartAutoPingResponse();
            //PostboxAPIUnityConnector.Instance.BootAPI();
        }
        else
        {
            PostboxAPIUnityConnector.Instance.StopAutoPingResponse();
            //PostboxAPIUnityConnector.Instance.ResetUserSettings();
        }
    }
}
