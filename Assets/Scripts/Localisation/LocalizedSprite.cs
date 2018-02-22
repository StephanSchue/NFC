using UnityEngine;
using System.Collections;

[RequireComponent(typeof(SpriteRenderer))]
[System.Serializable]
public class LocalizedSprite : MonoBehaviour, ILocalizedObject
{
    private SpriteRenderer spriteRenderer;
    [SerializeField][HideInInspector] public LocalisationSpriteElement[] Sprites = new LocalisationSpriteElement[0];

    protected void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        LocalisationController.OnLanguageChanged += OnLanguageChanged;
    }

    protected void OnDestroy()
    {
        LocalisationController.OnLanguageChanged -= OnLanguageChanged;
    }

    #region ILocalizedObject implementation

    public void OnLanguageChanged(string isoCode)
    {
        spriteRenderer.sprite = GetSpriteByIsoCode(isoCode);
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
