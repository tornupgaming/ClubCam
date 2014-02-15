using System;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager
{
	private static CameraManager _Instance;

	public static CameraManager Instance {
		get {
			if (_Instance == null) {
				_Instance = new CameraManager ();
			}
			return _Instance;
		}
	}

	private Dictionary<string, WebCam> m_WebCams = new Dictionary<string, WebCam> ();

	public void UpdateCameraList(){
		Debug.Log ("Updating camera list");
		WebCamDevice[] devices = WebCamTexture.devices;
		for (int i = 0; i < devices.Length; i++) {
			if(!m_WebCams.ContainsKey(devices[i].name)){
				Debug.Log ("Adding new camera to list: " + devices[i].name);
				m_WebCams.Add (devices [i].name, new WebCam (){ Name = devices[i].name, Device = devices[i] });
			}
		}
	}

	public List<WebCam> GetAllDevices(){
		List<WebCam> cams = new List<WebCam> ();
		foreach (WebCam cam in m_WebCams.Values) {
			cams.Add (cam);
		}
		return cams;
	}
}


