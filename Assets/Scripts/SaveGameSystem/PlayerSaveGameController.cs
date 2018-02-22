using UnityEngine;
using System.Collections;

[System.Serializable]
public class PlayerSaveGameController
{
    public PlayerSaveGame current { get; private set; }

    public PlayerSaveGameController()
    {
        current = new PlayerSaveGame();
    }

    public void AddNewPlant(int type, Vector3 position, SpriteRenderer spriteRenderer)
    {
        if (current.usedSeeds < current.seedPlaces.Length) 
        {
            current.seedPlaces[current.usedSeeds] = new PlayerSaveGame.SeedPlace(type, position, true, spriteRenderer);
            ++current.usedSeeds;
        } 
        else 
        {
            Debug.Log("List is full!");
        }
    }

    public void IncreaseStartupCount()
    {
        ++current.startUps;
    }


    public int GetStartUpCount()
    {
        return current.startUps;
    }

    public void SetLanguage(int languageIndex)
    {
        current.language = languageIndex;
    }

    public int GetLanguage()
    {
        return current.language;
    }

    public PlayerSaveGame.SeedPlace[] GetSeedPlaces()
    {
        return current.seedPlaces;
    }

    public void SaveData()
    {
        DataStorage.SaveToFile<PlayerSaveGame>(current, StorageMethod.JSON);
    }

    public void ClearData()
    {
        current = new PlayerSaveGame();
        DataStorage.SaveToFile<PlayerSaveGame>(current, StorageMethod.JSON);
    }

    public void LoadData()
    {
        current = new PlayerSaveGame();
        current = DataStorage.LoadFromFile<PlayerSaveGame>(StorageMethod.JSON);

        if(current == null)
            current = new PlayerSaveGame();
    }

    public int GetCountUnplacedSeeds()
    {
        return current.collectedSeeds - current.usedSeeds;
    }

    public int GetCountPlacedSeeds()
    {
        return current.usedSeeds;
    }

    public int GetCountCollectedSeeds()
    {
        return current.collectedSeeds;
    }

    public void EnableTutorial()
    {
        current.showTutorial = true;
    }

    public void DisableTutorial()
    {
        current.showTutorial = false;
    }

    public bool IsTutorialEnabled()
    {
        return current.showTutorial;
    }

    public void EnableIntro()
    {
        current.showIntro = true;
    }

    public void DisableIntro()
    {
        current.showIntro = false;
    }

    public bool IsIntroEnabled()
    {
        return current.showIntro;
    }

    public void IncreaseGamesPlayed()
    {
        current.lastGamePlayed = System.DateTime.Now.Ticks;
        ++current.gamesPlayed;
    }

    public void UnlockSecret()
    {
        current.secret = true;
    }

    public void DisableSecret()
    {
        current.secret = false;
    }
}