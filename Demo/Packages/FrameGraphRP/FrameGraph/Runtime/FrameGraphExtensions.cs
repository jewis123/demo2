using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace FrameGraph
{
    public static class FrameGraphExtensions
    {
        public static bool IsValidRenderTarget(this int id)
        {
            return id != PassInfo.INVALID_RESOURCE;
        }

        public static bool IsRenderTargetAllocated(this int id)
        {
            return id != PassInfo.INVALID_RESOURCE && RenderTargetRegistrar.IsRenderTargetAllocated(id);
        }
        
        public static RenderTargetHandle ToRenderTargetHandle(this int id)
        {
            return id == PassInfo.INVALID_RESOURCE ? RenderTargetHandle.CameraTarget : RenderTargetRegistrar.GetRenderTarget(id);
        }
    }
}