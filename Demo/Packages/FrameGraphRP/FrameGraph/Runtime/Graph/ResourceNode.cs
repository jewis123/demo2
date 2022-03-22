using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

namespace FrameGraph
{
    [Serializable]
    public class ResourceNode
    {
        public const int NO_RESOURCE = -1;
        
        /// <summary>
        /// 资源序号
        /// </summary>
        public int id;
        
        /// <summary>
        /// 资源名字
        /// </summary>
        public string name;
    }
}