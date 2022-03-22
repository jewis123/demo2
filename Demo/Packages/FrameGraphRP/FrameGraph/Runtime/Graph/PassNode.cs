using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FrameGraph
{

    [Serializable]
    public class PassNode
    {
        public const int NO_PASS = -1;

        /// <summary>
        /// Pass序号
        /// </summary>
        public int id;
        
        /// <summary>
        /// Pass名字
        /// </summary>
        public string name;
        
        /// <summary>
        /// 只读资源列表
        /// </summary>
        public int[] inputReses;
        
        /// <summary>
        /// 读写资源列表
        /// </summary>
        public int[] outputReses;
        
        /// <summary>
        /// 只读资源的前置关联Pass
        /// </summary>
        public int[] inputResRefPasses;
        
        /// <summary>
        /// 读写资源的前置关联Pass
        /// </summary>
        public int[] outputResRefPasses;
    }
}