using UnityEngine;
using System.Collections;
using ClickerEngine;
using System.Collections.Generic;

public class MenuMainDisplay : IMenu {

	public UILabel lbl_BeatNumber;

	List<WebCam> m_ActiveWebCams;
	private WebCam m_CurrentCamera = null;
	public UITexture tex_CameraView, tex_Logo;

	private bool m_UsingVisualizer = false;
	private int m_BeatsUntilVisualizerEnding = 64;
	private int m_BeatsUntilVisualizerPlaying = 64;
	private int m_CurrentVisualizerBeat = 0;
	private const float PlayingTrackBPM = 145;

	private float m_BPM = 175;
	private float m_TimeBetweenSwitches = 1.0f;
	private float m_CurrentTime = 0.0f;
	private int m_BeatsPerSwitch = 8;
	private int m_CurrentBeat;

	int width = 1024; // texture width
	int height = 768; // texture height
	Color backgroundColor = Color.black;
	Color waveformColor = Color.green;
	int size = 4096; // size of sound segment displayed in texture

	private Color[] blank; // blank image array
	private Texture2D m_VisualizationTexture;
	private float[] samples; // audio samples array
	private Vector3 RColor = new Vector3(Color.red.r, Color.red.g, Color.red.b);
	private Vector3 GColor = new Vector3(Color.green.r, Color.green.g, Color.green.b);
	Vector3 colVec;
	Color lerpColor;
	float div;

	public override void OnStart ()
	{
		// create the samples array
		samples = new float[size];
		// create the texture and assign to the guiTexture:
		m_VisualizationTexture = new Texture2D(width, height);

		// create a 'blank screen' image
		blank = new Color[width * height];
		for (int i = 0; i < blank.Length; i++) {
			blank [i] = backgroundColor;
		}
		// refresh the display each 100mS
		//StartCoroutine (PeriodiclyGetWave ());
		lerpColor = new Color (255, 255, 255, 255);
		div = (1.0f / height) * 2;

		base.OnStart ();
	}

	public override void OnShow ()
	{
		tex_Logo.gameObject.SetActive (false);
		base.OnShow ();
		tex_CameraView.width = Screen.width;
		tex_CameraView.height = Screen.height;

		m_ActiveWebCams = new List<WebCam> ();
		List<WebCam> cams = CameraManager.Instance.GetAllDevices ();
		foreach (WebCam cam in cams) {
			if (cam.Active) {
				Debug.Log ("Added webcam to active cams: " + cam.Name);
				m_ActiveWebCams.Add (cam);
				if (cam.Texture != null) {
					cam.Texture.Stop ();
					cam.Texture = null;
				}
				cam.Texture = new WebCamTexture (cam.Name, Screen.width, Screen.height, 60);
				cam.Texture.Play ();
				
			} else {
				Debug.Log ("Camera wasn't active: " + cam.Name);
			}
		}

		if (m_ActiveWebCams.Count > 0) {
			SetViewedCamera (m_ActiveWebCams [0]);
		}
	}

	public override void OnHide ()
	{
		base.OnHide ();
		if (m_ActiveWebCams != null) {
			foreach (WebCam cam in m_ActiveWebCams) {
				cam.Texture.Stop ();
				cam.Texture = null;
			}
			m_ActiveWebCams = null;
		}
	}

	public override string ToString ()
	{
		return "MenuMainDisplay";
	}

	private void SetViewedCamera(WebCam cam){
		m_CurrentCamera = cam;
		tex_CameraView.mainTexture = cam.Texture;
	}

	private WebCam GetNextCamera(){
		int index = m_ActiveWebCams.IndexOf (m_CurrentCamera);
		index++;
		if (index >= m_ActiveWebCams.Count) {
			index = 0;
		}
		return m_ActiveWebCams [index];
	}

	public override void OnBackButtonPressed ()
	{
		MenuManager.Instance.PopMenu ();
	}

	private void UpdatePlayingTrackPitch(){
		audio.pitch = m_BPM / PlayingTrackBPM;
	}
	bool pulseOn = true;
	public override void OnUpdate ()
	{
		m_CurrentTime += Time.deltaTime;
		if (m_CurrentTime >= m_TimeBetweenSwitches) {
			m_CurrentTime -= m_TimeBetweenSwitches;
			m_CurrentBeat++;
			m_CurrentVisualizerBeat++;

			if (m_CurrentVisualizerBeat >= ((m_UsingVisualizer) ? m_BeatsUntilVisualizerEnding : m_BeatsUntilVisualizerPlaying)) {
				m_CurrentVisualizerBeat = 0;
				if (m_UsingVisualizer) {
					audio.Stop ();
					tex_Logo.gameObject.SetActive (false);
				} else {
					UpdatePlayingTrackPitch ();
					audio.Play ();
					tex_Logo.gameObject.SetActive (true);
					pulseOn = true;
				}
				m_UsingVisualizer = !m_UsingVisualizer;
				tex_CameraView.mainTexture = m_VisualizationTexture;

			}

			if (!m_UsingVisualizer) {

				if (m_CurrentBeat >= m_BeatsPerSwitch) {
					m_CurrentBeat = 0;
					SetViewedCamera (GetNextCamera ());
				}
			} else {
				if (pulseOn) {
					PulseLogo ();
				}
				pulseOn = !pulseOn;
			}
		}

		if (m_UsingVisualizer) {
			GetCurWave();

		}

		if (Input.GetKeyDown (KeyCode.Space)) {
			m_CurrentTime = 0;
			m_CurrentBeat = 0;
			m_CurrentVisualizerBeat = 0;
		}
		if(Input.GetKeyDown(KeyCode.H)){
			lbl_BeatNumber.cachedGameObject.SetActive(!lbl_BeatNumber.cachedGameObject.activeSelf);
		}
		if (Input.GetKeyDown (KeyCode.Alpha1)) {
			m_BeatsPerSwitch = 1;
		}
		if (Input.GetKeyDown (KeyCode.Alpha2)) {
			m_BeatsPerSwitch = 2;
		}
		if (Input.GetKeyDown (KeyCode.Alpha3)) {
			m_BeatsPerSwitch = 4;
		}
		if (Input.GetKeyDown (KeyCode.Alpha4)) {
			m_BeatsPerSwitch = 8;
		}
		if (Input.GetKeyDown (KeyCode.Alpha5)) {
			m_BeatsPerSwitch = 16;
		}
		if (Input.GetKeyDown (KeyCode.Alpha6)) {
			m_BeatsPerSwitch = 32;
		}
		if (Input.GetKeyDown (KeyCode.Alpha7)) {
			m_BeatsPerSwitch = 64;
		}

		if (Input.GetKey (KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift)) {
			if (Input.GetKeyDown (KeyCode.UpArrow)) {
				m_BPM += 1.0f;
				UpdatePlayingTrackPitch ();
			}
			if (Input.GetKeyDown (KeyCode.DownArrow)) {
				m_BPM -= 1.0f;
				UpdatePlayingTrackPitch ();
			}
		} else {
			if (Input.GetKey(KeyCode.UpArrow)) {
				m_BPM += 1.0f;
				UpdatePlayingTrackPitch ();
			}
			if (Input.GetKey(KeyCode.DownArrow)) {
				m_BPM -= 1.0f;
				UpdatePlayingTrackPitch ();
			}
		}

		m_TimeBetweenSwitches = BPMToSeconds (m_BPM);
		if (m_UsingVisualizer) {
			lbl_BeatNumber.text = (1+m_CurrentVisualizerBeat).ToString() + "/" + m_BeatsUntilVisualizerEnding.ToString() + System.Environment.NewLine + "BPM: " + m_BPM;
		} else {
			lbl_BeatNumber.text = (1+m_CurrentBeat).ToString() + "/" + m_BeatsPerSwitch.ToString() + System.Environment.NewLine + "BPM: " + m_BPM;
		}


		base.OnUpdate ();
	}

	private void PulseLogo(){
		tex_Logo.transform.localScale = Vector3.one * 1.2f;
		TweenScale.Begin (tex_Logo.gameObject, m_TimeBetweenSwitches - PulseJumpTime, Vector3.one * 0.7f).method = UITweener.Method.Linear;
	}

	float PulseJumpTime = 0.05f;

	private float BPMToSeconds(float bpm){
		return 1.0f / (bpm / 60.0f);
	}

	// VISUALIZER

	private void GetCurWave(){
		m_VisualizationTexture.SetPixels(blank, 0);
		audio.GetOutputData(samples, 0);
		for (int i = 0; i < size; i++){
			SetPixel(width * i / size, (int)(height * (samples[i]+1f)/2), 3, samples[i]);
		}
		m_VisualizationTexture.Apply();
	}

	private void SetPixel(int x, int y, int radius, float t){
		SetPixelWithColouredHeight(x-1, y,t);

		SetPixelWithColouredHeight(x, y-1,t);
		SetPixelWithColouredHeight(x, y,t);
		SetPixelWithColouredHeight(x, y+1,t);

		SetPixelWithColouredHeight(x+1, y,t);
	}


	private void SetPixelWithColouredHeight(int x, int y, float t){
		//float t = (div * y) - 1;
		colVec = Vector3.Lerp (GColor, RColor, ((Mathf.Abs(t) + 0.15f) * 1.5f));
		lerpColor.r = colVec.x;
		lerpColor.g = colVec.y;
		lerpColor.b = colVec.z;
		m_VisualizationTexture.SetPixel(x, y, lerpColor);
	}
}
