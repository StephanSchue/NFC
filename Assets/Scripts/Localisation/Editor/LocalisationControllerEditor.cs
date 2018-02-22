using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

[CustomEditor(typeof(LocalisationController))]
[CanEditMultipleObjects]
public class LocalisationControllerEditor : Editor 
{
    private LocalisationController myController = null;

    private ILocalizedObject[] localisationObjects;

    private string languageLabel = "";
    private SystemLanguage languageObject = SystemLanguage.English;
    private string languageIsoCode = "";


    private void OnEnable()
    {
        myController = (LocalisationController)target;
    }

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        // Update the serializedProperty - always do this in the beginning of OnInspectorGUI.
        serializedObject.Update();

        // --- VIEW ---
        EditorGUILayout.Space();

        for (int i = 0; i < myController.Languages.Length; i++) 
        {
            EditorGUILayout.BeginHorizontal();

            LocalisationLanguageElement entry = myController.Languages[i];

            entry.label = EditorGUILayout.TextField(entry.label);
            entry.language = (SystemLanguage)EditorGUILayout.EnumPopup(entry.language);
            entry.isoCode = EditorGUILayout.TextField(entry.isoCode);

            myController.Languages[i] = entry;

            EditorGUILayout.EndHorizontal();
        }

        // --- Controls ---
        EditorGUILayout.LabelField("New Entry", EditorStyles.boldLabel);

        EditorGUILayout.BeginHorizontal();

        languageLabel = EditorGUILayout.TextField(languageLabel);
        languageObject = (SystemLanguage)EditorGUILayout.EnumPopup(languageObject as System.Enum);
        languageIsoCode = EditorGUILayout.TextField(languageIsoCode);

        EditorGUILayout.EndHorizontal();

        if(GUILayout.Button("Add new Node"))
        {
            AddNode(languageLabel, languageObject, languageIsoCode);
            languageLabel = "";
            languageObject = SystemLanguage.English;
            languageIsoCode = "";
        }

        if(GUILayout.Button("Remove last Node"))
        {
            RemoveNode(myController.Languages[myController.Languages.Length-1]);
        }

        Undo.RecordObject(target, "Generate Sprites");

        // Apply changes to the serializedProperty - always do this in the end of OnInspectorGUI.
        serializedObject.ApplyModifiedProperties();

        // --- List of Localisation Elements ---
        //localisationObjects = GameObject.FindObjectsOfType<ILocalizedObject>();
        //Debug.Log(localisationObjects.Length);
    }

    /// <summary>
    /// Adds the node.
    /// </summary>
    /// <param name="name">Name.</param>
    /// <param name="isoCode">Iso code.</param>
    public void AddNode(string name, SystemLanguage language, string isoCode)
    {
        List<LocalisationLanguageElement> languageList = new List<LocalisationLanguageElement>(myController.Languages);
        languageList.Add(new LocalisationLanguageElement(name, language, isoCode));

        myController.Languages = languageList.ToArray();
        languageList = null;
    }

    /// <summary>
    /// Removes the node.
    /// </summary>
    /// <param name="entry">Entry.</param>
    public void RemoveNode(LocalisationLanguageElement entry)
    {
        List<LocalisationLanguageElement> languageList = new List<LocalisationLanguageElement>(myController.Languages);
        languageList.Remove(entry);

        myController.Languages = languageList.ToArray();
        languageList = null;
    }
}
