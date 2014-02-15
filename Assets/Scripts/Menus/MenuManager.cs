using System;
using UnityEngine;
using System.Collections.Generic;
using System.Collections;

namespace ClickerEngine
{
	public class MenuManager : MonoBehaviour
	{
		public static MenuManager Instance;
		private void Awake(){Instance = this;}

		// Menus
		private Stack<IMenu> m_MenuStack = new Stack<IMenu> ();	
		private Dictionary<string, IMenu> m_MenuMap = new Dictionary<string, IMenu> ();

		#region Unity Functions

		private void Update(){
			if(m_MenuStack != null && m_MenuStack.Count>0){
				m_MenuStack.Peek().OnUpdate();
				if(Input.GetKeyDown(KeyCode.Escape)){
					m_MenuStack.Peek().OnBackButtonPressed();
				}
			}
		}

		#endregion

		#region Helper Functions

		#endregion

		#region Core Menu Functionality

		public void RegisterMenu(IMenu menu){
			if (m_MenuMap.ContainsKey (menu.ToString ())) {
				Debug.LogError ("Menu Already exists!!! " + menu.ToString());
			} else {
				m_MenuMap.Add (menu.ToString (), menu);
				menu.OnStart ();
				if (menu.StartupMenu) {
					PushMenu (menu);
				} else {
					menu.OnHide ();
				}
			}
		}

		public void PushMenu (string menuName){
			if (m_MenuMap.ContainsKey (menuName)) {
				PushMenu (m_MenuMap [menuName]);
			} else {
				Debug.LogError ("Menu name doesn't exist! " + menuName);
			}
		}

		private void PushMenu(IMenu menu){
			if (m_MenuStack.Count > 0) {
				IMenu currentMenu = m_MenuStack.Peek ();
				if (currentMenu != menu) {
					if (currentMenu != null) {
						currentMenu.OnHide ();
					}
					m_MenuStack.Push (menu);
					menu.OnShow ();
				}
			} else {
				m_MenuStack.Push (menu);
				menu.OnShow ();
			}
		}

		public void PopMenu(){
			PopMenu (true);
		}

		public void PopMenu(bool showNextMenu){
			IMenu currentMenu = m_MenuStack.Pop();
			if (currentMenu != null) {
				currentMenu.OnHide ();
			}
			if (showNextMenu) {
				if(m_MenuStack.Count > 0){
					m_MenuStack.Peek().OnShow();
				} else {
					Application.Quit();
				}
			}
		}

		#endregion
	}
}

