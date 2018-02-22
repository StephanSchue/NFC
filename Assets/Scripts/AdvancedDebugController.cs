using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System;

public class AdvancedDebugController : MonoBehaviour 
{
    public GameController gameController;

    public InputField ravenPositionInputfield;

    public void SetRavenPosition()
    {
        gameController.SetRavenPosition(Convert.ToInt32(ravenPositionInputfield.text));
        gameController.InstantMovePosition();
    }

    public void MoveForward()
    {
        gameController.DiceController.RollRaven();
    }

    public void MoveBackward()
    {
        int position =  string.IsNullOrEmpty(ravenPositionInputfield.text) ? 0 : Convert.ToInt32(ravenPositionInputfield.text) - 1;

        if(position < 0)
            return;

        ravenPositionInputfield.text = position.ToString();
        gameController.SetRavenPosition(position);
        gameController.MoveRaven();
    }

    #region Animation Trigger 

    public void LoopingAndLand()
    {
        ravenPositionInputfield.text = "0";
        SetRavenPosition();

        gameController.Raven.LoopingAndLand(null);
    }

    public void Fly_Land()
    {
        ravenPositionInputfield.text = "0";
        SetRavenPosition();

        gameController.Raven.FlyDown(null);
    }

    public void Idle()
    {
        gameController.Raven.SimpleIdle(null);
    }

    public void IdleGroovie()
    {
        gameController.Raven.IdleGroovie(null);
    }

    public void IdleGroovingSinging()
    {
        gameController.Raven.IdleGroovingSinging(null);
    }

    public void IdleDefaultSinging()
    {
        gameController.Raven.IdleDefaultSinging(null);
    }

    public void IdleAtBasket()
    {
        gameController.Raven.IdleAtBasket(null);
    }

    public void AppearBehindGrass01()
    {
        gameController.Raven.AppearBehindGrass01(null);
    }

    public void AppearBehindGrass02()
    {
        gameController.Raven.AppearBehindGrass02(null);
    }

    public void StairsUp()
    {
        gameController.Raven.StairsUp(null);
    }

    public void StairsDown()
    {
        gameController.Raven.StairsDown(null);
    }

    public void BigStepForward()
    {
        gameController.Raven.BigStepForward(null);
    }

    public void DigAndDissappear()
    {
        gameController.Raven.DigAndDissappear(null);
    }

    public void DiveAhead()
    {
        gameController.Raven.DiveAhead(null);
    }

    public void SimpleDive()
    {
        gameController.Raven.SimpleDive(null);
    }

    public void SecretAgentFlip()
    {
        gameController.Raven.SecretAgentFlip(null);
    }

    public void Stumble()
    {
        gameController.Raven.Stumble(null);
    }

    public void WhistleAndJump()
    {
        gameController.Raven.WhistleAndJump(null);
    }

    public void PoutAndStomp()
    {
        gameController.Raven.PoutAndStomp(null);
    }

    public void RantFlyOff()
    {
        gameController.Raven.RantFlyOff(null);
    }

    public void BasketAndRun()
    {
        gameController.SetRavenPosition(4);
        gameController.InstantMovePositionWithOutAnimation();

        gameController.Raven.BasketAndRun(null);
    }

    #endregion
}
