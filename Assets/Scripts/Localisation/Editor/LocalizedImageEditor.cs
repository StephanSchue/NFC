using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

[CustomEditor(typeof(LocalizedImage))]
[CanEditMultipleObjects]
public class LocalizedImageEditor : Editor 
{
    private LocalizedImage myController = null;

    private ILocalizedObject[] localisationObjects;

    private int languageIsoCodeIndex = 0;
    private string languageIsoCode = "";
    private Sprite languageSprite = null;


    private void OnEnable()
    {
        myController = (LocalizedImage)target;

        UpdateLanguage();
    }

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        // Update the serializedProperty - always do this in the beginning of OnInspectorGUI.
        serializedObject.Update();

        // --- VIEW ---
        EditorGUILayout.Space();

        if(myController.Sprites != null)
        {
            for (int i = 0; i < myController.Sprites.Length; i++)
            {
                EditorGUILayout.BeginHorizontal();

                LocalisationSpriteElement entry = myController.Sprites[i];

                entry.isoCode = EditorGUILayout.TextField(entry.isoCode);
                entry.sprite = EditorGUILayout.ObjectField(entry.sprite, typeof(Sprite), false) as Sprite;

                myController.Sprites[i] = entry;

                EditorGUILayout.EndHorizontal();
            }
        }

        // --- Controls ---
                            /*
        EditorGUILayout.LabelField("New Entry", EditorStyles.boldLabel);

        EditorGUILayout.BeginHorizontal();

        languageIsoCodeIndex = EditorGUILayout.Popup(languageIsoCodeIndex, LocalisationController.Instance.IsoCodes);
        languageIsoCode = LocalisationController.Instance.IsoCodes[languageIsoCodeIndex];

        languageSprite = EditorGUILayout.ObjectField(languageSprite, typeof(Sprite), false) as Sprite;

        EditorGUILayout.EndHorizontal();

        if(GUILayout.Button("Add new Node"))
        {
            AddNode(languageIsoCode, languageSprite);
            languageIsoCode = "";
            languageSprite = null;

            if(languageIsoCodeIndex < LocalisationController.Instance.Languages.Length)
            {
                ++languageIsoCodeIndex;
            }
        }
        */
        if(GUILayout.Button("Remove last Node"))
        {
            RemoveNode(myController.Sprites[myController.Sprites.Length-1]);
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
    public void AddNode(string isoCode, Sprite sprite)
    {
        List<LocalisationSpriteElement> languageList = new List<LocalisationSpriteElement>(myController.Sprites);
        languageList.Add(new LocalisationSpriteElement(isoCode, sprite));

        myController.Sprites = languageList.ToArray();
        languageList = null;
    }

    /// <summary>
    /// Removes the node.
    /// </summary>
    /// <param name="entry">Entry.</param>
    public void RemoveNode(LocalisationSpriteElement entry)
    {
        List<LocalisationSpriteElement> languageList = new List<LocalisationSpriteElement>(myController.Sprites);
        languageList.Remove(entry);

        myController.Sprites = languageList.ToArray();
        languageList = null;
    }

    public void UpdateLanguage()
    {
        if(myController.Sprites.Length != LocalisationController.Instance.Languages.Length)
        {
            while(myController.Sprites.Length != LocalisationController.Instance.Languages.Length)
            {
                if(myController.Sprites.Length > LocalisationController.Instance.Languages.Length)
                {
                    RemoveNode(myController.Sprites[myController.Sprites.Length-1]);
                }
                else
                {
                    LocalisationLanguageElement langElement = LocalisationController.Instance.Languages[myController.Sprites.Length];
                    AddNode(langElement.isoCode, null);
                }
            }
        }
    }
}
