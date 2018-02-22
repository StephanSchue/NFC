using UnityEngine;
using UnityEngine.UI;
using System.Collections;

[RequireComponent(typeof(Dropdown))]
[System.Serializable]
public class LanguageDropdown : MonoBehaviour, ILocalizedObject
{
    public Dropdown dropDown { get; set; }
    [SerializeField][HideInInspector] public LocalisationSpriteElement[] Sprites = new LocalisationSpriteElement[0];

    protected void Awake()
    {
        dropDown = GetComponent<Dropdown>();
        LocalisationController.OnLanguageIndexChanged += OnLanguageChanged;

        dropDown.value = LocalisationController.Instance.CurrentLanguageIndex;
    }

    protected void OnDestroy()
    {
        LocalisationController.OnLanguageChanged -= OnLanguageChanged;
    }
    
    #region ILocalizedObject implementation

    public void OnLanguageChanged(string isoCode)
    {
        throw new System.NotImplementedException();
    }

    public void OnLanguageChanged(int index)
    {
        dropDown.value = index;
    }

    #endregion
}
