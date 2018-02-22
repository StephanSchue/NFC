using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

[CustomEditor(typeof(LanguageDropdown))]
[CanEditMultipleObjects]
public class LanguageDropdownEditor : Editor 
{
    private LanguageDropdown myController = null;

    private ILocalizedObject[] localisationObjects;

    private int languageIsoCodeIndex = 0;
    private string languageIsoCode = "";
    private Sprite languageSprite = null;


    private void OnEnable()
    {
        myController = (LanguageDropdown)target;
        myController.dropDown = myController.GetComponent<Dropdown>();

        SetupDropdown(LocalisationController.Instance.Languages);
    }

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        // Update the serializedProperty - always do this in the beginning of OnInspectorGUI.
        //serializedObject.Update();

        // --- VIEW ---

        //Undo.RecordObject(target, "Doing Stuff");

        // Apply changes to the serializedProperty - always do this in the end of OnInspectorGUI.
        //serializedObject.ApplyModifiedProperties();
    }

    /// <summary>
    /// Setups the dropdown.
    /// </summary>
    /// <param name="languages">Languages.</param>
    public void SetupDropdown(LocalisationLanguageElement[] languages)
    {
        if(languages.Length < 1)
            return;

        myController.dropDown.ClearOptions();

        List<Dropdown.OptionData> optionsList = new List<Dropdown.OptionData>();

        for (int i = 0; i < languages.Length; i++)
        {
            optionsList.Add(new Dropdown.OptionData(languages[i].label));
        }

        myController.dropDown.AddOptions(optionsList); 
    }
}
