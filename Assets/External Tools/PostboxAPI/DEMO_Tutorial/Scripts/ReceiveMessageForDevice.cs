using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using PostboxAPI;

public class ReceiveMessageForDevice : MonoBehaviour
{
    public Text MessageOutput;
    public int Cooldown = 5;

    private string processId = "";
    private bool reveiveMessages = true;

    // Use this for initialization
    private void Start()
    {
        GetMessages(Cooldown);
    }

    public void GetMessages(int interval)
    {
        processId = PostboxAPIUnityConnector.Instance.GetDataPackages(GetMessagesCallback, interval);
    }

    public void GetMessagesCallback(PostboxGetDataPackagesResponse response)
    {
        switch (response.CallStatus)
        {
            case PostboxCallStatus.Success:
                RefreshTextbox(response.packages);
                break;
            case PostboxCallStatus.Error:
                response.LogAllAPIErrors();
                break;
            default:
                break;
        }
    }

    private void RefreshTextbox(PostboxDataPackage[] packages)
    {
        if (packages != null)
        {
            MessageOutput.text = "";

            foreach (PostboxDataPackage package in packages)
            {
                if (package.IsJSON)
                {
                    MessageOutput.text += "[JSON] ";
                }
                else if (package.IsXML)
                {
                    MessageOutput.text += "[XML] ";
                }

                MessageOutput.text += package.Data + System.Environment.NewLine;

                SetMessageReceived(package);
            }
        }
    }

    public void SetMessageReceived(PostboxDataPackage package)
    {
        PostboxAPIUnityConnector.Instance.UpdateDataPackageStatus(package.TransactionId, 
                                                                    PostboxDataPackageStatus.received, 
                                                                    SetMessageReceivedCallback);
    }

    public void SetMessageReceivedCallback(PostboxUpdateDataPackageResponse response)
    {
        switch (response.CallStatus)
        {
            case PostboxCallStatus.Success:
                Debug.Log("Message has been marked as successfully received!");
                break;
            case PostboxCallStatus.Error:
                response.LogAllAPIErrors();
                break;
            default:
                break;
        }
    }

    public void ToggleReveiceMessages()
    {
        reveiveMessages = !reveiveMessages;
        if (reveiveMessages)
        {
            GetMessages(Cooldown);
        }
        else
        { 
            PostboxAPIUnityConnector.Instance.StopGetPackages(processId);
        }
    }
}

