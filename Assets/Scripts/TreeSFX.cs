using UnityEngine;
using System.Collections;

public class TreeSFX : MonoBehaviour 
{
    public AudioClip[] sfx_plop;
	
    public void PlayPlop()
    {
        AudioManager.Instance.SetSFXChannel(sfx_plop[Random.Range(0, sfx_plop.Length)], null, 0, 2);
    }
}
