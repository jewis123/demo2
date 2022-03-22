/*------------------------------------------------------------------------------
|
| COPYRIGHT (C) 2018 - 2026 All Right Reserved
|
| FILE NAME  : \Assets\CLineActionEditor\ActionEngine\Res\ResourceMgr.cs
| AUTHOR     : https://supercline.com/
| PURPOSE    : 
|
| SPEC       : resource name format[/gamedata/xx.prefab], and case-insensitive
|
| MODIFICATION HISTORY
| 
| Ver	   Date			   By			   Details
| -----    -----------    -------------   ----------------------
| 1.0	   2018-9-10        SuperCLine           Created
|
+-----------------------------------------------------------------------------*/


using System.Collections.Generic;
using System.IO;

namespace SuperCLine.ActionEngine
{
    using System;
    using System.Collections;
    using UnityEngine;

    public class ResConfig
    {
        public static readonly string PathPrefix = Application.dataPath + @"/Resources/";
        public static readonly string RootDataPath = PathPrefix + "CLineActionEditor/GameData/";
        public static readonly string RootDataResPath = "CLineActionEditor/GameData/";
        public static readonly string ActionDataPath = "Action/";
        public static readonly string ActionInterruptDataPath = "ActionInterrupt/";
        public static readonly string AIDataPath = "AI/";
        public static readonly string BuffDataPath = "Buff/";
        public static readonly string UnitDataPath = "Unit/";
        
        public static readonly string PlayerPropertyFile = RootDataResPath + "Unit/Player.json";
        public static readonly string MonsterPropertyFile = RootDataResPath + "Unit/Monster.json";
        public static readonly string WeaponPropertyFile = RootDataResPath + "Unit/Weapon.json";
        public static readonly string AISwitchFile = RootDataResPath + "Unit/AISwitch.json";
        public static readonly string BuffFactoryPropertyFile = RootDataResPath + "Buff/Buff.json";
        public static readonly string BuffFile = "Buff/Buff.json";
    }
    
    public sealed class ResourceMgr : MonoSingleton<ResourceMgr>
    {
        private string CorrectPath(string path)
        {
            var idx = path.IndexOf('.');
            if (idx != -1)
            {
                var endLength = path.Length - idx;
                path = path.Substring(0, path.Length - endLength);
            }

            if (path.StartsWith(ResConfig.PathPrefix))
            {
                path = path.Substring(ResConfig.PathPrefix.Length);
            }

            return path;
        }
        
        public void LoadObject<T>(string path, Action<T> callback = null)where T : UnityEngine.Object
        {
            path = CorrectPath(path);
            StartCoroutine(DelayCallBack<T>(path , callback));
        }

        public T LoadObject<T>(string path) where T : UnityEngine.Object
        {
            path = CorrectPath(path);
            
            return Resources.Load<T>(path);
        }


        IEnumerator DelayCallBack<T>(string name , Action<T> callback) where T : UnityEngine.Object
        {
            var loadRequest = Resources.LoadAsync<T>(name);
            yield return loadRequest;
            if (callback != null)
            {
                callback(loadRequest.asset as T);
            }
        }
        
        /// <summary>
        /// 掐头去尾， 保留资源路径
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public string FormatResourceName(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                return "";
            }
            var startIdx = name.IndexOf("Resources/");
            var lastIdx = name.LastIndexOf(".");
            var lastLength = name.Length - lastIdx;
            var startLength = "Resources/".Length;
            var nameLength = lastIdx - startIdx - startLength;
            var rst = name.Substring(startIdx + startLength, nameLength);
            return rst;
        }

        public string[] GetFilePaths(string rootPath)
        {
            return Directory.GetFiles(rootPath);
        }

    }

}
