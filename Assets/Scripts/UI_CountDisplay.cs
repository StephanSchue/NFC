using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class UI_CountDisplay : MonoBehaviour 
{
	public Image background;
	public Text content;

	public void SetCount(int count)
	{
		if(count > 0) 
		{
			content.text = count.ToString();
			//enabled = true;
		} 
		else 
		{
			//enabled = false;
		}
	}

	protected void OnEnable()
	{
		background.enabled = true;
		//content.enabled = true;
	}

	protected void OnDisable()
	{
		background.enabled = false;
		//content.enabled = false;
	}

    public void ShowContent(bool status)
    {
        content.enabled = status;
    }
}
