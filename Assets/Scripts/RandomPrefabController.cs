using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RandomPrefabController : MonoBehaviour 
{
    public RandomPrefabChooser[] chooser;
    public bool autoRandomize = true;
    public int minPlacedCount = 1;
    public int maxPlacedCount = 3;
    private int curPlacedCount = 0;
    private static Random rng = new Random();  

    // Use this for initialization
    private void Awake() 
    {
        if(autoRandomize && curPlacedCount == 0)
            Generate();
    }

    public void GetChildren()
    {
        chooser = GetComponentsInChildren<RandomPrefabChooser>();

        if(chooser != null && chooser.Length > 0) 
        {
            for (int i = 0; i < chooser.Length; i++) 
            {
                chooser[i].Initialize();
            }
        }
    }

    public void Generate()
    {
        if(chooser != null && chooser.Length > 0)
        {
            RandomizeBuiltinArray(chooser);

            int count = Random.Range(minPlacedCount, maxPlacedCount + 1);

            for(int i = 0; i < count; i++) 
            {
                if(chooser[i].enabled)
                {
                    chooser[i].Spawn();
                    ++curPlacedCount;
                }
            }
        }
    }

    private static void RandomizeBuiltinArray(Object[] array)
    {
        for (var i = array.Length - 1; i > 0; i--) 
        {
            var r = Random.Range(0,i);
            Object tmp = array[i];
            array[i] = array[r];
            array[r] = tmp;
        }
    }
}
