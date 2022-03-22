using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace FrameGraph
{
    /// <summary>
    /// URP对InvokeOnRenderObjectCallbackPass调用固定在绘制物体之后，现在需要开放出来单独可在FrameGraph内调整
    /// </summary>
    public class InvokeOnRenderObjectCallbackPassWrapper : FrameGraphScriptableRenderPass
    {
        public InvokeOnRenderObjectCallbackPassWrapper(URPContext context) : base()
        {
            base.profilingSampler = new ProfilingSampler("InvokeOnRenderObjectCallbackPass");
        }

        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            context.InvokeOnRenderObjectCallback();
        }

        protected override bool Setup(int[] inputs, int[] outputs, ref RenderingData renderingData)
        {
            return true;
        }

        public override void ClearUp()
        {
         
        }
    }
}