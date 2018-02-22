using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class CreditsHearthController : MonoBehaviour 
{
    public AudioClip unlockSound;
    public Image secrectSpriteRenderer;
    public Sprite[] secretSprites;

    private PlayerSaveGameController saveGameController;

    private void Awake()
    {
        saveGameController = new PlayerSaveGameController();
        saveGameController.LoadData();

        if(saveGameController.current.secret)
            ShowSecret();
    }

    public void UnlockSecret()
    {
        saveGameController.UnlockSecret();
        saveGameController.SaveData();

        AudioManager.Instance.SetSFXChannel(unlockSound, null, 0f, 0);
        Debug.Log("Secret unlocked");
    }

    public void ShowSecret()
    {
        if(secretSprites != null && secretSprites.Length > 0)
            secrectSpriteRenderer.sprite = secretSprites[Random.Range(0, secretSprites.Length)];
    }
}
