using UnityEngine;

namespace SimplePoker.Views
{
    public abstract class UiBaseView : MonoBehaviour
    {
        private UiBaseData _data;
        protected UiBaseData Data => _data;

        public GameObject Content;
        public virtual void SetData(UiBaseData data)
        {
            _data = data;
            RefreshViewInternal();
        }

        private void RefreshViewInternal()
        {
            Show();
            RefreshView();
        }
        protected abstract void RefreshView();
        public void Show()
        {
            Content.SetActive(true);
        }

        public void Hide()
        {
            ResetView();
            Content.SetActive(false);
        }

        protected abstract void ResetView();
    }
    
    
}