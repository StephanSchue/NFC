using UnityEngine;
using System.Collections;

[System.Serializable]
public class LoadingScreenParamContainer : Singleton<LoadingScreenParamContainer>  
{
    protected LoadingScreenParamContainer() {} // guarantee this will be always a singleton only - can't use the constructor!

    public string currentScene { get; private set; }
    public string sceneToBeLoaded  { get; private set; }

    public void SetupLoadScreen(string currentScene, string sceneToBeLoaded)
    {
        this.currentScene = currentScene;
        this.sceneToBeLoaded = sceneToBeLoaded;
    }
}
