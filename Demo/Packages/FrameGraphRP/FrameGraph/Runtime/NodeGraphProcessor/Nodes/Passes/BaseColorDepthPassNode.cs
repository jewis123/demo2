using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using GraphProcessor;
using UnityEngine;
using UnityEngine.Rendering;

namespace FrameGraph
{
    public abstract class BaseColorDepthPassNode : BaseColorPassNode
    {
        [Input("Depth Attachment")] public int inputDepth;

        [Output("Depth Attachment", true)] public int outputDepth;

        protected override string[] customInputFilters => new[] {nameof(inputColor), nameof(inputDepth)};

        protected override void Process()
        {
            base.Process();
            outputDepth = inputDepth;
        }
    }
}