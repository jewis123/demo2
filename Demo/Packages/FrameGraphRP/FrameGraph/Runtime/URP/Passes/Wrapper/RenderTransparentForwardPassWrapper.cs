using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.Rendering.Universal.Internal;

namespace FrameGraph
{
    public class RenderTransparentForwardPassWrapper : DrawObjectPassWrapper
    {
        private bool m_shouldReceiveShadows;

        public RenderTransparentForwardPassWrapper(URPContext context) : base(context, "DrawTransparentObjects", false, RenderQueueRange.transparent, context.data.transparentLayerMask)
        {
            m_shouldReceiveShadows = context.data.shadowTransparentReceive;
        }

        protected override bool needShadow
        {
            get { return m_shouldReceiveShadows && hasShadowMap; }
        }
    }
}