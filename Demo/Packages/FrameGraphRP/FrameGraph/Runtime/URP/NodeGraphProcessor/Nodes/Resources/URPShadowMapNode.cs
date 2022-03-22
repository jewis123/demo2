using GraphProcessor;
using UnityEngine;

namespace FrameGraph
{
    [System.Serializable, NodeMenuItem("RenderTarget/URP/ShadowMap")]
    public class URPShadowMapNode : BaseResourceNode
    {
        // [SerializeField]
        // public string shaderProperty = "_MainLightShadowmapTexture";
        public override string name => "_MainLightShadowmapTexture";
    }
}