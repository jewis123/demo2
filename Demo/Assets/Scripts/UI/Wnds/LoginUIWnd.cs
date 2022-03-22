using Scene.SceneControllers;

namespace UI.Wnds
{
    public class LoginUIWnd : UIController
    {
        public override void OnOpen(object[] param)
        {
            
        }

        public override void OnClose()
        {
            
        }


        public void OnStartGameButtonClk()
        {
           GameCore.singleton.gameSceneManager.LoadScene<ExampleScene>();
           Close();
        }
    }
}