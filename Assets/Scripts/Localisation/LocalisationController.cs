using UnityEngine;
using System.Collections;

public delegate void LanguageChangedChallback(string isoCode);
public delegate void LanguageChangedIndexChallback(int index);


[System.Serializable]
public class LocalisationController : Singleton<LocalisationController> 
{
    protected LocalisationController() {} // guarantee this will be always a singleton only - can't use the constructor!

    // --- Variables ---
    [SerializeField][HideInInspector] public LocalisationLanguageElement[] Languages = new LocalisationLanguageElement[] {new LocalisationLanguageElement("Deutsch", SystemLanguage.German, "de"), new LocalisationLanguageElement("English", SystemLanguage.English, "en")};
    public static LanguageChangedChallback OnLanguageChanged;
    public static LanguageChangedIndexChallback OnLanguageIndexChanged;

    public LocalisationLanguageElement defaultLanguage 
    { 
        get
        {
            return (Languages != null && Languages.Length > 0) ? Languages[0]: new LocalisationLanguageElement();
        }

        private set
        {
            
        }
    }
    
    public string[] IsoCodes 
    { 
        get
        {
            string[] isoCodes = new string[Languages.Length];

            for (int i = 0; i < Languages.Length; i++)
            {
                isoCodes[i] = Languages[i].isoCode;
            }

            return isoCodes;
        }

        private set
        {

        }
    }

    private int currentLanguageIndex = 0;

    public int CurrentLanguageIndex 
    { 
        get
        {
            return currentLanguageIndex;
        }

        private set
        {

        }
    }

    public LocalisationLanguageElement CurrentLanguage
    { 
        get
        {
            return Languages[currentLanguageIndex];
        }

        private set
        {

        }
    }
     

    #region Inizalize/Deinizalize

    /// <summary>
    /// Awake this instance.
    /// </summary>
    private void Start()
    {
    
    }

    /// <summary>
    /// Raises the destory event.
    /// </summary>
    private void OnDestory()
    {

    }

    #endregion

    /// <summary>
    /// Changes the language.
    /// </summary>
    /// <param name="isoCode">Iso code.</param>
    public void ChangeLanguage(int index)
    {
        if(index < Languages.Length)
        {
            Debug.Log("Changed Language into " + Languages[index].isoCode);
            currentLanguageIndex = index;

            if(OnLanguageChanged != null)
                OnLanguageChanged(Languages[index].isoCode);

            if(OnLanguageIndexChanged != null)
                OnLanguageIndexChanged(index);
        }
    }

    /// <summary>
    /// Changes the language.
    /// </summary>
    /// <param name="isoCode">Iso code.</param>
    public void ChangeLanguage(string isoCode)
    {
        for (int i = 0; i < Languages.Length; i++)
        {
            if(isoCode == Languages[i].isoCode)
            {
                ChangeLanguage(i);
                return;
            }
        }

        // Fallback
        ChangeLanguage(SystemLanguage.English);
    }

    /// <summary>
    /// Changes the language.
    /// </summary>
    /// <param name="isoCode">Iso code.</param>
    public void ChangeLanguage(SystemLanguage systemLanguage)
    {
        for (int i = 0; i < Languages.Length; i++)
        {
            if(Languages[i].language.ToString().Contains(systemLanguage.ToString()))
            {
                ChangeLanguage(i);
                return;
            }
        }

        // Fallback
        ChangeLanguage(SystemLanguage.English);
    }
}

[System.Serializable]
public struct LocalisationLanguageElement
{
    [SerializeField] public string label;
    [SerializeField] public SystemLanguage language;
    [SerializeField] public string isoCode;

    public LocalisationLanguageElement(string label, SystemLanguage language, string isoCode)
    {
        this.label = label;
        this.language = language;
        this.isoCode = isoCode;
    }
}

[System.Serializable]
public struct LocalisationSpriteElement
{
    [SerializeField] public string isoCode;
    [SerializeField] public Sprite sprite;

    public LocalisationSpriteElement(string isoCode, Sprite sprite)
    {
        this.isoCode = isoCode;
        this.sprite = sprite;
    }
}

[System.Serializable]
public struct LocalisationTextElement
{
    [SerializeField] public string isoCode;
    [SerializeField] public string text;

    public LocalisationTextElement(string isoCode, string text)
    {
        this.isoCode = isoCode;
        this.text = text;
    }
}