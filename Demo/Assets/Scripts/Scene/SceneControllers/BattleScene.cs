namespace Scene.SceneControllers
{
    public class BattleScene : GameSceneController
    {
        public override void OnEnterBegin()
        {
            
        }

        public override void OnEnterEnd(UnityEngine.SceneManagement.Scene scene)
        {
            GameCore.singleton.uiManager.OpenUI("BattleUI");
        }

        public override void OnExitBegin()
        {
            GameCore.singleton.uiManager.CloseUI("BattleUI");
        }

        public override void OnExitEnd()
        {
            
        }
    }
}