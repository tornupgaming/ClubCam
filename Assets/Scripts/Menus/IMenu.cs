using System;
using UnityEngine;
using System.Collections;

namespace ClickerEngine
{
	public class IMenu : MonoBehaviour
	{
		public bool StartupMenu = false;

		private void Start(){
			MenuManager.Instance.RegisterMenu (this);
		}

		public GameObject ParentObject;
		protected const float OFFSCREEN_X = 1000.0f;
		protected const float TWEEN_TIME = 0.3f;
		protected const float TWEEN_SCALE_TIME = 0.05f;
		protected bool m_Animating = false;

		public bool IsShowing(){
			return ParentObject.activeSelf;
		}

		public virtual void OnStart(){}
		public virtual void OnUpdate(){}
		public virtual void OnShow(){
			ParentObject.SetActive (true);
		}
		public virtual void OnHide(){
			ParentObject.SetActive (false);}
		public virtual IEnumerator OnAnimatedShow(){yield break;}
		public virtual IEnumerator OnAnimatedHide(Action OnFinish){yield break;}
		public virtual void OnBackButtonPressed(){}
	}
}

