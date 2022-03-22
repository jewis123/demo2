using System;
using System.Collections.Generic;
using GraphProcessor;
using UnityEngine;

namespace FrameGraph
{
    public class FrameGraphBaseNode : BaseNode
    {
        /// <summary>
        /// 使用自定义值的字段
        /// </summary>
        [HideInInspector]
        public List<string> customFields = new List<string>();

        public override bool isRenamable => true;
    }
}