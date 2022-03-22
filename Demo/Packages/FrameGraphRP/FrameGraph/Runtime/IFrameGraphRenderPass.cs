using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace FrameGraph
{
    /// <summary>
    /// FrameGraph渲染Pass的接口
    /// </summary>
    public interface IFrameGraphRenderPass
    {
        /// <summary>
        /// 用于设置指定Pass的参数
        /// </summary>
        /// <param name="pass"></param>
        /// <param name="renderingData"></param>
        /// <returns></returns>
        bool Setup(int pass, ref RenderingData renderingData);
        
        /// <summary>
        /// 返回可编程渲染Pass
        /// </summary>
        /// <returns></returns>
        ScriptableRenderPass GetScriptableRenderPass();

        /// <summary>
        /// 清除
        /// </summary>
        void ClearUp();

        /// <summary>
        /// 被加到待处理列表前调用
        /// </summary>
        void ConfigureBeforeEnqueued();
    }
}

