using System.Collections.Generic;
using GraphProcessor;
using FrameGraph;

[System.Serializable, NodeMenuItem("Passes/URP/MainLightShadowCasterPass")]
public class URPMainLightShadowCasterPassNode : BaseColorPassNode
{
    public override string name => "URPMainLightShadowCasterPass";

    public override string scriptableRenderPass => "FrameGraph.MainLightShadowCasterPassWrapper";
    
    [CustomPortBehavior(nameof(inputColor))]
    protected IEnumerable<PortData> GetInputColorPort(List<SerializableEdge> edges)
    {
        yield return NewPortData("inputColor", "ShadowMapTex", false);
    }
        
    [CustomPortBehavior(nameof(outputColor))]
    protected IEnumerable<PortData> GetOutputColorPort(List<SerializableEdge> edges)
    {
        yield return NewPortData("outputColor", "ShadowMapTex", true);
    }
}