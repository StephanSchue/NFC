using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class InitAppController : MonoBehaviour 
{
	// Use this for initialization
	void Start () 
    {
        PlayerSaveGameController saveGameController = new PlayerSaveGameController();

        saveGameController.LoadData();
        saveGameController.EnableIntro();
        saveGameController.SaveData();

        SceneManager.LoadScene("StartScreen", LoadSceneMode.Single);
	}
}
