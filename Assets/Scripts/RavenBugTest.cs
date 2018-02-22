using UnityEngine;
using System.Collections;

public class RavenBugTest : MonoBehaviour 
{
    private RavenController ravenController;

    void Awake()
    {
        ravenController = GetComponent<RavenController>();
    }

	// Use this for initialization
	void Start() 
    {
        ravenController.Dive(0, Appear);
	}

    public void Appear()
    {
        ravenController.Appear(Throw);
    }

    public void Throw()
    {
        ravenController.Throw(null);
    }
}
