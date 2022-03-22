using System;
using GraphProcessor;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;

namespace FrameGraph
{
    [System.Serializable, NodeMenuItem("Passes/URP/RenderObjectsPass")]
    public class URPRenderObjectsPassNode : BaseColorDepthPassNode
    {
        [SerializeField, HideInInspector, Parameter, Context]
        private RenderObjectsPassWrapper.Settings settings;
        
        public override string name => "URPRenderObjectsPass";
        
        public override string scriptableRenderPass => "FrameGraph.RenderObjectsPassWrapper";
    }
}