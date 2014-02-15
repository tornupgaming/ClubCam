using System;
using ClickerEngine;
using UnityEngine;

public class MenuMain : IMenu
{
	public UIButton btn_StartDisplay, btn_SetupCameras;

	public override void OnStart ()
	{
		base.OnStart ();
		btn_StartDisplay.onClick.Add (new EventDelegate (btn_StartDisplay_OnClick));
		btn_SetupCameras.onClick.Add (new EventDelegate (btn_SetupCameras_OnClick));
		CameraManager.Instance.UpdateCameraList ();
	}

	public override void OnShow ()
	{
		base.OnShow ();
	}

	private void btn_StartDisplay_OnClick(){
		MenuManager.Instance.PushMenu ("MenuMainDisplay");

	}

	private void btn_SetupCameras_OnClick(){
		MenuManager.Instance.PushMenu ("MenuSetupCameras");
	}

	public override string ToString ()
	{
		return "MenuMain";
	}

	public override void OnBackButtonPressed ()
	{
		Application.Quit();
	}
}


