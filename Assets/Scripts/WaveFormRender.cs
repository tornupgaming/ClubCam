﻿﻿using UnityEngine;
using System.Collections;

public class WaveFormRender : MonoBehaviour {

	int width = 1024; // texture width
	int height = 768; // texture height
	Color backgroundColor = Color.black;
	Color waveformColor = Color.green;
	int size = 4096 * 2; // size of sound segment displayed in texture

	private Color[] blank; // blank image array
	private Texture2D texture;
	private float[] samples; // audio samples array
	private Vector3 RColor = new Vector3(Color.red.r, Color.red.g, Color.red.b);
	private Vector3 GColor = new Vector3(Color.green.r, Color.green.g, Color.green.b);

	void Start(){
		// create the samples array
		samples = new float[size];
		// create the texture and assign to the guiTexture:
		texture = new Texture2D(width, height);
		guiTexture.texture = texture;
		// create a 'blank screen' image
		blank = new Color[width * height];
		for (int i = 0; i < blank.Length; i++) {
			blank [i] = backgroundColor;
		}
		// refresh the display each 100mS
		StartCoroutine (PeriodiclyGetWave ());
	}

	private IEnumerator PeriodiclyGetWave(){
		while (true){
			GetCurWave();
			yield return new WaitForEndOfFrame ();
			//yield return new WaitForSeconds (10.0f);
		}
	}

	private void GetCurWave(){
		// clear the texture
		texture.SetPixels(blank, 0);
		// get samples from channel 0 (left)

		audio.GetOutputData(samples, 0);
		// draw the waveform
		for (int i = 0; i < size; i++){
			SetPixel(width * i / size, (int)(height * (samples[i]+1f)/2), 3);
		}
		// upload to the graphics card
		texture.Apply();
		//Debug.Break ();
	}

	private void SetPixel(int x, int y, int radius){
		//SetPixelWithColouredHeight(x-1, y-1);
		SetPixelWithColouredHeight(x-1, y);
		//SetPixelWithColouredHeight(x-1, y+1);

		SetPixelWithColouredHeight(x, y-1);
		SetPixelWithColouredHeight(x, y);
		SetPixelWithColouredHeight(x, y+1);

		//SetPixelWithColouredHeight(x+1, y-1);
		SetPixelWithColouredHeight(x+1, y);
		//SetPixelWithColouredHeight(x+1, y+1);
	}


	/*
	 * y = 385
	 * base = 1 / 768 = 0.0013
	 * 
	 * 500 
	 * 1.0 - ((1.0 / 768) * 500 * 2)
	 * 
	 * 
	 */


	private void SetPixelWithColouredHeight(int x, int y){
		float t = ((1.0f / height) * y*2) - 1;
		//Debug.Log ("t=" + t);
		Vector3 vec = Vector3.Lerp (GColor, RColor, ((Mathf.Abs(t) + 0.15f) * 1.5f));
		texture.SetPixel(x, y, new Color(vec.x, vec.y, vec.z));
	}
}
