using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

[CustomEditor(typeof(LocalizedTextMesh))]
[CanEditMultipleObjects]
public class LocalizedTextMeshEditor : Editor 
{
    private LocalizedTextMesh myController = null;

    private ILocalizedObject[] localisationObjects;

    private int languageIsoCodeIndex = 0;
    private string languageIsoCode = "";
    private string languageContent = null;


    private void OnEnable()
    {
        myController = (LocalizedTextMesh)target;

        UpdateLanguage();
    }

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        // Update the serializedProperty - always do this in the beginning of OnInspectorGUI.
        serializedObject.Update();

        // --- VIEW ---
        EditorGUILayout.Space();

        if(myController.Contents != null)
        {
            for (int i = 0; i < myController.Contents.Length; i++)
            {
                EditorGUILayout.BeginHorizontal();

                LocalisationTextElement entry = myController.Contents[i];

                entry.isoCode = EditorGUILayout.TextField(entry.isoCode);
                entry.text = EditorGUILayout.TextField(entry.text);

                myController.Contents[i] = entry;

                EditorGUILayout.EndHorizontal();
            }
        }

        // --- Controls ---
        Undo.RecordObject(target, "Generate LocalisationText");

        // Apply changes to the serializedProperty - always do this in the end of OnInspectorGUI.
        serializedObject.ApplyModifiedProperties();

        // --- List of Localisation Elements ---
        //localisationObjects = GameObject.FindObjectsOfType<ILocalizedObject>();
        //Debug.Log(localisationObjects.Length);
    }

    public void UpdateLanguage()
    {
        if(myController.Contents.Length != LocalisationController.Instance.Languages.Length)
        {
            while(myController.Contents.Length != LocalisationController.Instance.Languages.Length)
            {
                if(myController.Contents.Length > LocalisationController.Instance.Languages.Length)
                {
                    RemoveNode(myController.Contents[myController.Contents.Length-1]);
                }
                else
                {
                    LocalisationLanguageElement langElement = LocalisationController.Instance.Languages[myController.Contents.Length];
                    AddNode(langElement.isoCode, null);
                }
            }
        }
    }

    /// <summary>
    /// Adds the node.
    /// </summary>
    /// <param name="name">Name.</param>
    /// <param name="isoCode">Iso code.</param>
    public void AddNode(string isoCode, string sprite)
    {
        List<LocalisationTextElement> languageList = new List<LocalisationTextElement>(myController.Contents);
        languageList.Add(new LocalisationTextElement(isoCode, sprite));

        myController.Contents = languageList.ToArray();
        languageList = null;
    }

    /// <summary>
    /// Removes the node.
    /// </summary>
    /// <param name="entry">Entry.</param>
    public void RemoveNode(LocalisationTextElement entry)
    {
        List<LocalisationTextElement> languageList = new List<LocalisationTextElement>(myController.Contents);
        languageList.Remove(entry);

        myController.Contents = languageList.ToArray();
        languageList = null;
    }
}
