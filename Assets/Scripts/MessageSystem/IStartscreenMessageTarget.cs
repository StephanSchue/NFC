using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;

public interface IStartscreenMessageTarget : IEventSystemHandler
{
	// functions that can be called via the messaging system
	void PressStart();
}