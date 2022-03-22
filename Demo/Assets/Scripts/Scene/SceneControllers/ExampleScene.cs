using StarterAssets;
using UI.Wnds;
using UnityEngine;

namespace Scene.SceneControllers
{
    public class ExampleScene : GameSceneController
    {

        public override void OnEnterBegin()
        {
            
        }

        public override void OnEnterEnd(UnityEngine.SceneManagement.Scene scene)
        {
            GameCore.singleton.uiManager.OpenUI("JoyStickUI");
            var joyStickUIWnd = GameCore.singleton.uiManager.GetUI<JoyStickUIWnd>("JoyStickUI");
            var rootGameObjects = scene.GetRootGameObjects();

            for (int i = 0; i < rootGameObjects.Length; i++)
            {
                if (rootGameObjects[i].name == "Characters")
                {
                    var player = rootGameObjects[i].transform.Find("Player");
                    joyStickUIWnd.displayObject.GetComponent<UICanvasControllerInput>().starterAssetsInputs = player.GetComponent<StarterAssetsInputs>();
                }
            }
            
            
        }

        public override void OnExitBegin()
        {
            GameCore.singleton.uiManager.CloseUI("JoyStickUI");
        }

        public override void OnExitEnd()
        {
            
        }
    }
}