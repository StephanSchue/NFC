using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class NFCContainerBase : MonoBehaviour
{
    public AudioSource annoncementAudio;

    private void Reset()
    {
        GameObject aObj = GameObject.Find("AnnoncementAudio");

        if(aObj != null)
            annoncementAudio = aObj.GetComponent<AudioSource>();
    }

    public virtual void Initialize(NFCController.AmiiboData amiibo)
    {
        annoncementAudio.clip = amiibo.announcmentSFX;
        annoncementAudio.Play();
    }

    public abstract void Deinitialize();
}
