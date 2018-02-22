using UnityEngine;
using System.Collections;

[System.Serializable]
public class PlayerSaveGame
{
    [SerializeField] public uint gamesPlayed = 0;
    [SerializeField] public long lastGamePlayed = 0;

    [SerializeField] public byte collectedSeeds = 0;
    [SerializeField] public byte usedSeeds = 0;

    [SerializeField] public int language = 0;
    [SerializeField] public int startUps = 0;

    [SerializeField] public bool showTutorial = false;
    [SerializeField] public bool showIntro = true;

    [SerializeField] public bool secret = false;

    [System.Serializable]
    public struct SeedPlace
    {
        [SerializeField] public int type;
        [SerializeField] public Vector3 position;
        [SerializeField] public bool placed;

        [SerializeField] public string sprite;
        [SerializeField] public string sortingLayer;
        [SerializeField] public bool flipX;

        public SeedPlace(int type, Vector3 position, bool placed = false, SpriteRenderer spriteRenderer = null)
        {
            this.type = type;
            this.position = position;
            this.placed = placed;

            if(spriteRenderer != null)
            {
                this.sprite = spriteRenderer.sprite != null ? spriteRenderer.sprite.name : "";
                this.sortingLayer = spriteRenderer.sortingLayerName;
                this.flipX = spriteRenderer.flipX;
            }
            else
            {
                this.sprite = "";
                this.sortingLayer = "";
                this.flipX = false;
            }
        }
    }

    [SerializeField] public SeedPlace[] seedPlaces = new SeedPlace[50];
}