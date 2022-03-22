using System.Collections.Generic;
using GraphProcessor;
using UnityEngine;

namespace FrameGraph
{
    public abstract class BasePostProcessPassNode : BasePassNode
    {
        [Input("Destination")] public int inputDestination;

        [Output("Destination", true)] public int outputDestination;
        
        [Input("SourceTex")] public int inputSource;

        [Output("SourceTex", true)] public int outputSource;
        
        [SerializeField, HideInInspector, Parameter, Context]
        public BasePostProcessPass.Settings settings;
        
        protected override string[] customInputFilters => new[] {nameof(inputDestination)};
        
        protected override void Process()
        {
            base.Process();
            outputDestination = inputDestination;
            outputSource = inputSource;
        }
    }
}