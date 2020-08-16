using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

//Simple Fade in script to manipulate images alpha colour during loading screens etc..
public class Fade_In : MonoBehaviour {

	public float fadeInTime;

	private Image fadePanel;
	private Color currentColor = Color.black;

	void Start () 
	{
		fadePanel = GetComponent<Image> ();
	}
	

	void Update () 
	{
		if (Time.timeSinceLevelLoad < fadeInTime) 
		{
			float alphaChange = Time.deltaTime / fadeInTime;
			currentColor.a   -= alphaChange;
			fadePanel.color   = currentColor;
		} 
		else 
		{
			gameObject.SetActive (false);
            SceneManager.LoadScene("MainMenu");
		}
	}
}
