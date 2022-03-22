namespace Scene
{
    public abstract class GameSceneController
    {
        public abstract void OnEnterBegin();
        public abstract void OnEnterEnd(UnityEngine.SceneManagement.Scene scene);

        public abstract void OnExitBegin();
        public abstract void OnExitEnd();
    }
}