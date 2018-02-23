using UnityEngine;
using System.Collections;
using PostboxAPI;

public class GetSystemTime : MonoBehaviour
{
	// Use this for initialization
	private void Start ()
    {
        CallSeverTime();
    }

    public void CallSeverTime()
    {
        PostboxAPIUnityConnector.Instance.RequestServerSystemTime(ServerTimeCallback);
    }

    public void ServerTimeCallback(PostboxServerTimeResponse response)
    {
        switch (response.CallStatus)
        {
            case PostboxCallStatus.Success:
                Debug.Log(response.Timestamp);
                break;
            case PostboxCallStatus.Error:
                if(response.CheckErrorCode(PostboxAPIErrorCode.systemtime_error))
                {
                    response.LogAPIError(PostboxAPIErrorCode.systemtime_error);
                }
                break;
            default:
                break;
        }
    }

}
