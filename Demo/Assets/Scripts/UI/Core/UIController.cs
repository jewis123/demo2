using UnityEngine;

namespace UI
{
    public abstract class UIController : MonoBehaviour
    {

        public string uiName { get; set; }
        public GameObject displayObject { get; set; }

        public virtual void Open(object[] param)
        {
            OnOpen(param);
        }

        public abstract void OnOpen(object[] param);

        public abstract void OnClose();
        public void Close()
        {
            GameCore.singleton.uiManager.CloseUI(uiName);
        }
    }
}