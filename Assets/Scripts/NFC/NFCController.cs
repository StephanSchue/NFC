using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.Events;

public class NFCController : MonoBehaviour
{
    public Text debugOutput;
    public Button scanNFCButton;

    public AmiiboData activeAmiibo;

    private string qrString = "";
    private bool background = true;

    [System.Serializable]
    public struct AmiiboData
    {
        public string id;
        public string name;
        public NFCContainerBase controller;
        public AudioClip announcmentSFX;
    }

    public AmiiboData[] amiibos;

    public UnityEvent OnReadFinished;

    private void Start()
    {
        debugOutput.text = "";
        EnableBackgroundScan();
        ScanNFC();
    }

    public void ScanNFC()
    {
        debugOutput.text += "\nScanNFC";
        AndroidNFCReader.ScanNFC(gameObject.name, "OnFinishScan");
    }

    public void EnableBackgroundScan()
    {
        debugOutput.text += "\nEnableBackgroundScan";
        AndroidNFCReader.enableBackgroundScan();
        background = true;
        scanNFCButton.interactable = !background;
    }

    public void DisableBackgroundScan()
    {
        debugOutput.text += "\nDisableBackgroundScan";
        AndroidNFCReader.disableBackgroundScan();
        background = false;
        scanNFCButton.interactable = !background;
    }

    // NFC callback
    private void OnFinishScan(string result)
    {
        // Cancelled
        if (result == AndroidNFCReader.CANCELLED)
        {
            debugOutput.text += "\nCanceled: ";
        }
        else if (result == AndroidNFCReader.ERROR)
        {
            // Error
            debugOutput.text += "\nError: ";
        }
        else if (result == AndroidNFCReader.NO_HARDWARE)
        {
            // No hardware
            debugOutput.text += "\nNo hardware: ";
        }
        else if (result == AndroidNFCReader.NO_ALLOWED_OS)
        {
            debugOutput.text += "\nNo OS: ";
            //ActivateAmiiboWorld(amiibos[Random.Range(0, amiibos.Length)].id);
        }
        else if(!string.IsNullOrEmpty(result))
        {
            debugOutput.text += "\nSuccess: ";
            ActivateAmiiboWorld(result);
        }

        //qrString = getToyxFromUrl(result);
        debugOutput.text += result;
    }

    public void ActivateAmiiboWorld(string id)
    {
        AmiiboData amiibo = new AmiiboData();
        bool status = false;

        for(int i = 0; i < amiibos.Length; i++)
        {
            if (amiibos[i].id == id)
            {
                amiibo = amiibos[i];
                status = true;
            }
        }

        if(status)
        {
            if(activeAmiibo.controller != null)
                activeAmiibo.controller.Deinitialize();

            amiibo.controller.Initialize(amiibo);
            activeAmiibo = amiibo;

            if (OnReadFinished != null)
                OnReadFinished.Invoke();
        }
        else
        {
            Debug.Log("No Amiibo found!");
        }
    }

    // Extract toyxId from url
    private string getToyxFromUrl(string url)
    {
        int index = url.LastIndexOf('/') + 1;

        if (url.Length > index)
        {
            return url.Substring(index);
        }

        return url;
    }

}
