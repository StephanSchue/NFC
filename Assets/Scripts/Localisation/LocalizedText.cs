using UnityEngine;
using UnityEngine.UI;
using System.Collections;

[RequireComponent(typeof(Text))]
[System.Serializable]
public class LocalizedText : MonoBehaviour, ILocalizedObject
{
    private Text textComponent;
    [SerializeField][HideInInspector] public LocalisationTextElement[] Contents = new LocalisationTextElement[0];

    protected void Awake()
    {
        textComponent = GetComponent<Text>();
        LocalisationController.OnLanguageChanged += OnLanguageChanged;
    }

    protected void OnDestroy()
    {
        LocalisationController.OnLanguageChanged -= OnLanguageChanged;
    }

    #region ILocalizedObject implementation

    public void OnLanguageChanged(string isoCode)
    {
        textComponent.text = GetContentByIsoCode(isoCode);
    }

    #endregion

    public string GetContentByIsoCode(string isoCode)
    {
        string defaultLanguageContent = null;

        for (int i = 0; i < Contents.Length; i++)
        {
            if(isoCode == Contents[i].isoCode)
            {
                return Contents[i].text;
            }
        }

        return defaultLanguageContent;
    }
}
