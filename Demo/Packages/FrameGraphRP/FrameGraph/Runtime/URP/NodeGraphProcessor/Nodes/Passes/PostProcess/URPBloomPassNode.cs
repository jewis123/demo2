using System.Collections.Generic;
using GraphProcessor;

namespace FrameGraph
{
    [System.Serializable, NodeMenuItem("Passes/URP/PostProcess/BloomPass")]
    public class URPBloomPassNode : BasePostProcessPassNode
    {
        [Input("BloomMipDown0")] public int inputBloomMipDown0;

        [Output("BloomMipDown0", true)] public int outputBloomMipDown0;
        
        [Input("BloomMipUp1")] public int inputBloomMipUp1;

        [Output("BloomMipUp1", true)] public int outputBloomMipUp1;
        
        [Input("BloomMipDown1")] public int inputBloomMipDown1;

        [Output("BloomMipDown1", true)] public int outputBloomMipDown1;
        
        [Input("BloomMipUp2")] public int inputBloomMipUp2;

        [Output("BloomMipUp2", true)] public int outputBloomMipUp2;
        
        [Input("BloomMipDown2")] public int inputBloomMipDown2;

        [Output("BloomMipDown2", true)] public int outputBloomMipDown2;
        
        [Input("BloomMipUp3")] public int inputBloomMipUp3;

        [Output("BloomMipUp3", true)] public int outputBloomMipUp3;
        
        [Input("BloomMipDown3")] public int inputBloomMipDown3;

        [Output("BloomMipDown3", true)] public int outputBloomMipDown3;

        protected override string[] customInputFilters => new[] {nameof(inputDestination), nameof(inputBloomMipDown0), nameof(inputBloomMipUp1), nameof(inputBloomMipDown1), nameof(inputBloomMipUp2), nameof(inputBloomMipDown2), nameof(inputBloomMipUp3), nameof(inputBloomMipDown3)};
        
        protected override void Process()
        {
            base.Process();
            outputBloomMipDown0 = inputBloomMipDown0;
            outputBloomMipUp1 = inputBloomMipUp1;
            outputBloomMipDown1 = inputBloomMipDown1;
            outputBloomMipUp2 = inputBloomMipUp2;
            outputBloomMipDown2 = inputBloomMipDown2;
            outputBloomMipUp3 = inputBloomMipUp3;
            outputBloomMipDown3 = inputBloomMipDown3;
        }
        
        public override string name => "URPBloomPass";
        
        public override string scriptableRenderPass => "FrameGraph.BloomPassWrapper";
        
        [CustomPortBehavior(nameof(inputDestination))]
        protected IEnumerable<PortData> GetInputColorPort(List<SerializableEdge> edges)
        {
            yield return NewPortData("inputColor", "BloomMipUp0", false);
        }
        
        [CustomPortBehavior(nameof(outputDestination))]
        protected IEnumerable<PortData> GetOutputColorPort(List<SerializableEdge> edges)
        {
            yield return NewPortData("outputColor", "BloomMipUp0", true);
        }
    }
}