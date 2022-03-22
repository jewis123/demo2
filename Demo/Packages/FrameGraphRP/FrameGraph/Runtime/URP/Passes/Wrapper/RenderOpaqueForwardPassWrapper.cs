using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.Rendering.Universal.Internal;


namespace FrameGraph
{
    public class RenderOpaqueForwardPassWrapper : DrawObjectPassWrapper
    {
        public RenderOpaqueForwardPassWrapper(URPContext context)
            : base(context, "DrawOpaqueObjects", true, RenderQueueRange.opaque,
                context.data.opaqueLayerMask)
        {
        }
    }
}