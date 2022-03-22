using System;
using System.Collections;
using UnityEngine;

namespace Battle
{
    public class ResourcesLoadHelper
    {
        public static void Load<T>(MonoBehaviour helper, string path , Action<T> loadCallBack) where T: UnityEngine.Object
        {
            if (string.IsNullOrEmpty(path))
            {
                return;
            }
            helper.StartCoroutine(DelayCallBack<T>(path, loadCallBack));
        }
        
        static IEnumerator DelayCallBack<T>(string path , Action<T> callback) where T: UnityEngine.Object
        {
            var loadRequest = Resources.LoadAsync<T>(path);
            yield return loadRequest;
            if (callback != null)
            {
                callback(loadRequest.asset as T);
            }
        }
    }
}