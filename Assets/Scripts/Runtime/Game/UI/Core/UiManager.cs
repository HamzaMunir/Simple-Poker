using System.Collections;
using System.Collections.Generic;
using SimplePoker.UI.Data;
using UnityEngine;

namespace SimplePoker.Views
{
    public class UiManager : MonoBehaviour
    {
        public AudioClip ClickedSound;
        public AudioClip LostSound;
        public AudioClip WinSound;
        public PlayerUIView PlayerUi;
        public DisplayResultUIView ResultUi;

        private UiBaseView _activeView;

        public void ShowResultUI(ResultUIData data)
        {
            HideActiveView();
            ResultUi.SetData(data);
        }
        public void ShowPlayerUI(PlayerUIData data)
        {
            HideActiveView();
            PlayerUi.SetData(data);
            _activeView = PlayerUi;
        }
        
        
        public void HideMainUI()
        {
            PlayerUi.Hide();
        }

        private void HideActiveView()
        {
            if (_activeView != null)
            {
                _activeView.Hide();
            }
        }
    }
}