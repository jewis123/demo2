using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace FrameGraph
{
    public interface IVolumePatchedFunction<T>  where T : VolumeComponent
    {
        public void OnVolumePatched(CommandBuffer cmd, RenderingData renderingData, Material material, T volumeComponent);
    }
}