using System.Collections.Generic;
using GraphProcessor;
using UnityEngine;
using UnityEngine.Experimental.Rendering;
using UnityEngine.Serialization;

namespace FrameGraph
{
    [System.Serializable, NodeMenuItem("RenderTarget/CreateRenderTarget")]
    public class CreateRenderTargetNode : BaseResourceNode
    {
        [Parameter(true), HideInInspector] 
        public int width;
        
        [Parameter(true), HideInInspector] 
        public int height;
        
        [Parameter(true), HideInInspector] 
        public Downsampling downsampling;
        
        [Parameter(true), HideInInspector] 
        public bool useMipMap;
        
        [Parameter(true), HideInInspector] 
        public bool autoGenerateMips;
        
        [Parameter(true), HideInInspector] 
        public RenderTextureFormat colorFormat;
        
        [Parameter(true), HideInInspector] 
        public GraphicsFormat graphicsFormat;
        
        [Parameter(true), HideInInspector] 
        public int depthBufferBits;
        
        [Parameter(false), HideInInspector] 
        public FilterMode filterMode;
        
        [SerializeField]
        public override string name => "RenderTarget";
    }
}