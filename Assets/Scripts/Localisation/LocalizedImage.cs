using UnityEngine;
using UnityEngine.UI;
using System.Collections;

[RequireComponent(typeof(Image))]
[System.Serializable]
public class LocalizedImage : MonoBehaviour, ILocalizedObject
{
    private Image imageObject;
    [SerializeField][HideInInspector] public LocalisationSpriteElement[] Sprites = new LocalisationSpriteElement[0];

    protected void Awake()
    {
        imageObject = GetComponent<Image>();
        LocalisationController.OnLanguageChanged += OnLanguageChanged;
    }

    protected void OnDestroy()
    {
        LocalisationController.OnLanguageChanged -= OnLanguageChanged;
    }

    #region ILocalizedObject implementation

    public void OnLanguageChanged(string isoCode)
    {
        imageObject.sprite = GetSpriteByIsoCode(isoCode);
    }

    #endregion

    public Sprite GetSpriteByIsoCode(string isoCode)
    {
        Sprite defaultLanguageSprite = null;

        for (int i = 0; i < Sprites.Length; i++)
        {
            if(isoCode == Sprites[i].isoCode)
            {
                return Sprites[i].sprite;
            }
        }

        return defaultLanguageSprite;
    }
}
