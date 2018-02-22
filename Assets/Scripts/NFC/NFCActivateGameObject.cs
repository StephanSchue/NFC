using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NFCActivateGameObject : NFCContainerBase
{
    public GameObject world;

    public override void Initialize(NFCController.AmiiboData amiibo)
    {
        base.Initialize(amiibo);
        world.SetActive(true);
    }

    public override void Deinitialize()
    {
        world.SetActive(false);
    }
}
