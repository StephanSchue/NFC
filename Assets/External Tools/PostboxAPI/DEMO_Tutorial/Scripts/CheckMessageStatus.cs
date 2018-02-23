using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using PostboxAPI;

public class CheckMessageStatus : MonoBehaviour
{
    public InputField TransactionID;
    public int Interval = 5;

    private string processId = "";
    private bool reveiveMessages = true;

    public void CheckStatus()
    {
        if (!System.String.IsNullOrEmpty(TransactionID.text))
        {
            processId = PostboxAPIUnityConnector.Instance.GetDataPackageStatus(TransactionID.text, CheckMessageStatusCallback, Interval);
        }
    }

    public void CheckMessageStatusCallback(PostboxGetDataPackageStatusResponse response)
    {
        switch (response.CallStatus)
        {
            case PostboxCallStatus.Success:
                Debug.Log(response.Status + ": " + response.Notification);
                break;
            case PostboxCallStatus.Error:
                response.LogAllAPIErrors();
                break;
            default:
                break;
        }
    }

    public void ToggleCheckMessageStatus()
    {
        reveiveMessages = !reveiveMessages;
        if (reveiveMessages)
        {
            CheckStatus();
        }
        else
        {
            PostboxAPIUnityConnector.Instance.StopGetPackages(processId);
        }
    }
}
