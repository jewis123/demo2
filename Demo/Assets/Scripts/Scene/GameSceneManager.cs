using System;
using System.Collections;
using System.Collections.Generic;
using Scene.SceneControllers;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Scene
{
    public class GameSceneManager : MonoBehaviour
    {
        private Dictionary<string, GameSceneController> currentSceneState = new Dictionary<string, GameSceneController>();

        private Light defaultMainLight;
        private Camera defaultMainCamera;

        
        private Dictionary<string,Camera> currentCameras = new Dictionary<string,Camera>();
        private Dictionary<string,Light> currentLights = new Dictionary<string,Light>();

        private void Awake()
        {
            defaultMainCamera = Camera.main;

            var gos = SceneManager.GetActiveScene().GetRootGameObjects();
            for (int i = 0; i < gos.Length; i++)
            {
                var light = gos[i].GetComponent<Light>();
                if (light!=null && light.type == LightType.Directional)
                {
                    defaultMainLight = light;
                    break;
                }
            }
        }

        public void LoadScene<T>(System.Action callback = null) where T : GameSceneController,new()
        {
            string sceneName = typeof(T).Name;
            if (!currentSceneState.ContainsKey(sceneName))
            {
                StartCoroutine(LoadSceneInternal<T>(sceneName,callback));
            }
        }
        
        public void UnLoadScene<T>(System.Action callback = null) where T : GameSceneController,new()
        {
            string sceneName = typeof(T).Name;
            if (currentSceneState.ContainsKey(sceneName))
            {
                StartCoroutine(UnLoadSceneInternal<T>(sceneName,callback));
            }
        }


        private IEnumerator LoadSceneInternal<T>(string sceneName,System.Action callback) where T : GameSceneController,new()
        {

            if (!currentSceneState.ContainsKey(sceneName))
                currentSceneState.Add(sceneName, new T());

            var scene = SceneManager.GetSceneByName(sceneName);

            if (scene.isLoaded)
            {
                yield break;
            }
            currentSceneState[sceneName].OnEnterBegin();

            var asyncOperation = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
            // asyncOperation.allowSceneActivation = false;

            while (!asyncOperation.isDone)
            {
                yield return null;
            }
            
            
            scene = SceneManager.GetSceneByName(sceneName);


            var rootGameObjects = scene.GetRootGameObjects();

            for (int i = 0; i < rootGameObjects.Length; i++)
            {
                if (rootGameObjects[i].name == "Main Camera")
                {
                    currentCameras.Add(sceneName,rootGameObjects[i].GetComponent<Camera>());
                }
                else if( rootGameObjects[i].name == "Directional Light")
                {
                    currentLights.Add(sceneName,rootGameObjects[i].GetComponent<Light>());
                }
            }
            
            
            defaultMainCamera.gameObject.SetActive(currentCameras.Count<=0);
            defaultMainLight.gameObject.SetActive(currentLights.Count<=0);
            currentSceneState[sceneName].OnEnterEnd(scene);
            callback?.Invoke();
        }

        private IEnumerator UnLoadSceneInternal<T>(string sceneName,System.Action callback = null) where T : GameSceneController,new()
        {
            if (currentSceneState.ContainsKey(sceneName))
            {
                currentSceneState[sceneName].OnExitBegin();
                if (currentCameras.ContainsKey(sceneName))
                {
                    currentCameras.Remove(sceneName);
                }

                if (currentLights.ContainsKey(sceneName))
                {
                    currentLights.Remove(sceneName);
                }
                
                defaultMainCamera.gameObject.SetActive(currentCameras.Count<=0);
                defaultMainLight.gameObject.SetActive(currentLights.Count<=0);

                var asyncOperation = SceneManager.UnloadSceneAsync(sceneName);
                yield return asyncOperation;
                var sceneHandler = currentSceneState[sceneName];
               
                currentSceneState.Remove(sceneName);
                sceneHandler.OnExitEnd();
                callback?.Invoke();
            }

          
        }
    }
}