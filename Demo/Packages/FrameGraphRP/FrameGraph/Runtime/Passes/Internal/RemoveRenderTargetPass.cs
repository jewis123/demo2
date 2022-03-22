using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace FrameGraph
{
    internal class RemoveRenderTargetPass : RenderTargetServicePass
    {
        // public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        // {
        //     CommandBuffer cmd = CommandBufferPool.Get();
        //
        //     CommandBufferPool.Release(cmd);
        // }

        public override void FrameCleanup(CommandBuffer cmd)
        {
            base.FrameCleanup(cmd);
            cmd.ReleaseTemporaryRT(renderTargetHandle.id);
        }
    }
}