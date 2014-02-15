using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using ClickerEngine;

public class MenuSetupCameras : IMenu
{
	private const float ButtonYInitialPosition = 0.0f;
	private const float ButtonYSpacing = -80.0f;
	public UIButton btn_Back, btn_Active;
	public GameObject CameraButtonPrefab;
	public GameObject ScrollViewContentParent;
	public UITexture tex_CameraView;
	private List<MenuCameraListButton> m_CameraButtons;
	private WebCam m_CurrentWebCam;
	public Texture2D CameraWaitingTexture;

	public override void OnStart ()
	{
		base.OnStart ();
		btn_Back.onClick.Add (new EventDelegate (btn_Back_OnClick));
		btn_Active.onClick.Add (new EventDelegate (btn_Active_OnClick));
	}

	public override void OnShow ()
	{
		ConstructCameraList ();
		base.OnShow ();
		btn_Active.gameObject.SetActive (false);
	}

	public override void OnHide ()
	{
		base.OnHide ();
		if (m_CurrentWebCam != null) {
			m_CurrentWebCam.Texture.Stop ();
			m_CurrentWebCam = null;
			tex_CameraView.mainTexture = CameraWaitingTexture;
		}
		DestroyCameraList ();

	}

	public override void OnBackButtonPressed ()
	{
		MenuManager.Instance.PopMenu ();
	}

	private void btn_Back_OnClick ()
	{
		MenuManager.Instance.PopMenu ();
	}

	private void btn_Active_OnClick ()
	{
		if (m_CurrentWebCam != null) {
			m_CurrentWebCam.Active = !m_CurrentWebCam.Active;

			UISprite spr = btn_Active.GetComponent<UISprite> ();
			spr.spriteName = (m_CurrentWebCam.Active) ? "button-active" : "button-inactive";

			for (int i = 0; i < m_CameraButtons.Count; i++) {
				if (m_CameraButtons [i].AssociatedWebcam == m_CurrentWebCam) {
					m_CameraButtons[i].spr_LockedIcon.spriteName = (m_CurrentWebCam.Active) ? "icon-unlocked" : "icon-locked";
					break;
				}
			}
		}
	}

	public override string ToString ()
	{
		return "MenuSetupCameras";
	}

	private void OnWebCamButtonClicked (MenuCameraListButton btn)
	{
		if (btn.AssociatedWebcam != m_CurrentWebCam) {
			if (m_CurrentWebCam != null) {
				m_CurrentWebCam.Texture.Stop ();
				tex_CameraView.mainTexture = CameraWaitingTexture;
			}

			m_CurrentWebCam = btn.AssociatedWebcam;



			m_CurrentWebCam.Texture = new WebCamTexture (btn.AssociatedWebcam.Name, tex_CameraView.width, tex_CameraView.height, 60);
			m_CurrentWebCam.Texture.Play ();
			StartCoroutine (WaitForWebCamToStart (m_CurrentWebCam.Texture));

			btn_Active.gameObject.SetActive (true);
			UISprite spr = btn_Active.GetComponent<UISprite> ();
			spr.spriteName = (m_CurrentWebCam.Active) ? "button-active" : "button-inactive";
		}
	}

	private IEnumerator WaitForWebCamToStart(WebCamTexture tex){
		yield return new WaitForEndOfFrame ();
		while (!tex.isPlaying) {
			yield return new WaitForEndOfFrame ();
		}
		tex_CameraView.mainTexture = m_CurrentWebCam.Texture;
	}

	#region Scroll List Management

	private void ConstructCameraList ()
	{
		m_CameraButtons = new List<MenuCameraListButton> ();
		List<WebCam> webcamList = CameraManager.Instance.GetAllDevices ();
		for (int i = 0; i < webcamList.Count; i++) {
			CreateCameraButtonFromWebCam (webcamList [i]);
		}
	}

	private void CreateCameraButtonFromWebCam (WebCam camera)
	{
		Debug.Log ("Creating camera button: " + camera.Name);

		GameObject newBtnObj = Instantiate (CameraButtonPrefab) as GameObject;
		MenuCameraListButton btn = newBtnObj.GetComponent<MenuCameraListButton> ();
		btn.btn_Main.onClick.Add (new EventDelegate (() => {
			OnWebCamButtonClicked (btn);
		}));
		btn.AssociatedWebcam = camera;
		btn.lbl_CameraName.text = camera.Name;
		btn.spr_LockedIcon.spriteName = (camera.Active) ? "icon-unlocked" : "icon-locked";

		btn.transform.parent = ScrollViewContentParent.transform;

		float yOffset = ButtonYInitialPosition + (ButtonYSpacing * m_CameraButtons.Count);

		btn.transform.localPosition = new Vector3 (0,yOffset,0);
		btn.transform.localScale = Vector3.one;

		m_CameraButtons.Add (btn);
	}

	private void DestroyCameraList ()
	{
		if (m_CameraButtons != null) {
			for (int i = m_CameraButtons.Count - 1; i >= 0; i--) {
				Destroy (m_CameraButtons [i].gameObject);
			}
			m_CameraButtons.Clear ();
			m_CameraButtons = null;
		}
	}

	#endregion

}
