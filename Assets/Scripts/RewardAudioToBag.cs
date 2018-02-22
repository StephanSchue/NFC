using UnityEngine;
using System.Collections;

public class RewardAudioToBag : MonoBehaviour 
{
    public RewardAnimationController rewardAnimationController;
	
    public void PlayBagAppears()
    {
        rewardAnimationController.PlayBagAppears();
    }
}
