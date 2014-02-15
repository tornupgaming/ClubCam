using UnityEngine;
using System.Collections;

public class StretchBackground : MonoBehaviour {

	// Use this for initialization
	void Update () {
		UISprite spr = GetComponent<UISprite> ();
		spr.width = Screen.width;
		spr.height = Screen.height;
	}
}
