using System;
using System.Collections;
using System.Collections.Generic;
using UI;
using UnityEngine;

namespace UI
{
    public class UIManager : MonoBehaviour
    {
        private Dictionary<string, GameObject> uiObjects = new Dictionary<string, GameObject>();


        private void Awake()
        {
            uiObjects = new Dictionary<string, GameObject>();
            DontDestroyOnLoad(gameObject);
        }

        public void OpenUI(string uiName,params object[] param)
        {
            if (IsOpen(uiName)) return;
            var uiprefab = Resources.Load<GameObject>($"UI/{uiName}");
            var ui = GameObject.Instantiate(uiprefab, transform, false);
            ui.name = uiName;
            var controller = ui.GetComponent<UIController>();
            controller.Open(param);
            controller.uiName = uiName;
            controller.displayObject = ui;
            uiObjects.Add(uiName, ui);
        }


        public void CloseUI(string uiName)
        {
            if (IsOpen(uiName))
            {
                var ui = uiObjects[uiName];
                uiObjects.Remove(uiName);
                var controller = ui.GetComponent<UIController>();
                controller.OnClose();
                
                Destroy(ui);
            }
        }
        
        public bool IsOpen(string uiName)
        {
            return uiObjects.ContainsKey(uiName);
        }
        
        public UIController GetUI<T>(string uiName) where T : UIController
        {
            var ui = uiObjects[uiName];
            var controller = ui.GetComponent<UIController>();
            
            if(controller.uiName == uiName)
                return controller as T;
            return null;
        }
        
    }
}
