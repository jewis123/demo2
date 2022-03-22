using System.Collections.Generic;
using GraphProcessor;

namespace FrameGraph
{
    /// <summary>
    /// 非最后一个执行的栈相机使用的后处理节点
    /// </summary>
    [System.Serializable, NodeMenuItem("Passes/URP/PostProcess/StackedPostProcessPass")]
    public class URPStackedPostProcessPassNode : BasePostProcessPassNode
    {
        [Input("LutTex")] public int inputLut;
        
        [Input("BloomTex")] public int inputBloom;

        public override string name => "URPStackedPostProcessPass";
        
        public override string scriptableRenderPass => "FrameGraph.StackedPostProcessPassWrapper";
    }
}