using System;
using System.Collections.Generic;

namespace FrameGraph
{
    public class ParameterAttribute : Attribute
    {
        /// <summary>
        /// 是否通过"+"按钮来添加字段
        /// </summary>
        public bool wait2Add;
        
        public ParameterAttribute(bool wait2Add = false)
        {
            this.wait2Add = wait2Add;
        }
    }
}