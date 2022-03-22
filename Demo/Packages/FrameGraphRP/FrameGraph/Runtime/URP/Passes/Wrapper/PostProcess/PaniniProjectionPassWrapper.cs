using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace FrameGraph
{
    /// <summary>
    /// 从PostProcessPass中剥离出来的帕尼尼投影部分
    /// </summary>
    public class PaniniProjectionPassWrapper : BasePostProcessPass
    {
        private PaniniProjection m_PaniniProjection;
        
        public PaniniProjectionPassWrapper(URPContext context, BasePostProcessPass.Settings settings) : base(context, settings, "PaniniProjection")
        {

        }

        protected override void OnProcess(CommandBuffer cmd, ScriptableRenderContext context, ref RenderingData renderingData)
        {
            DoPaniniProjection(renderingData.cameraData.camera, cmd, source.id, destination.id);
        }

        protected override bool OnSetup(int[] inputs, int[] outputs, ref RenderingData renderingData)
        {
            var stack = VolumeManager.instance.stack;
            m_PaniniProjection = stack.GetComponent<PaniniProjection>();
            return renderingData.cameraData.postProcessEnabled && m_PaniniProjection.IsActive() && !renderingData.cameraData.isSceneViewCamera;
        }
        
        #region Panini Projection

        // Back-ported & adapted from the work of the Stockholm demo team - thanks Lasse!
        void DoPaniniProjection(Camera camera, CommandBuffer cmd, int source, int destination)
        {
            float distance = m_PaniniProjection.distance.value;
            var viewExtents = CalcViewExtents(camera);
            var cropExtents = CalcCropExtents(camera, distance);

            float scaleX = cropExtents.x / viewExtents.x;
            float scaleY = cropExtents.y / viewExtents.y;
            float scaleF = Mathf.Min(scaleX, scaleY);

            float paniniD = distance;
            float paniniS = Mathf.Lerp(1f, Mathf.Clamp01(scaleF), m_PaniniProjection.cropToFit.value);

            var material = context.postProcessMaterials.paniniProjection;
            material.SetVector(FrameGraphConstant.ShaderConstants._Params, new Vector4(viewExtents.x, viewExtents.y, paniniD, paniniS));
            material.EnableKeyword(
                1f - Mathf.Abs(paniniD) > float.Epsilon
                ? ShaderKeywordStrings.PaniniGeneric : ShaderKeywordStrings.PaniniUnitDistance
            );

            FrameGraphPostProcessUtils.Blit(cmd, source, FrameGraphPostProcessUtils.BlitDstDiscardContent(cmd, destination), material);
        }

        Vector2 CalcViewExtents(Camera camera)
        {
            float fovY = camera.fieldOfView * Mathf.Deg2Rad;
            float aspect = descriptor.width / (float)descriptor.height;

            float viewExtY = Mathf.Tan(0.5f * fovY);
            float viewExtX = aspect * viewExtY;

            return new Vector2(viewExtX, viewExtY);
        }

        Vector2 CalcCropExtents(Camera camera, float d)
        {
            // given
            //    S----------- E--X-------
            //    |    `  ~.  /,´
            //    |-- ---    Q
            //    |        ,/    `
            //  1 |      ,´/       `
            //    |    ,´ /         ´
            //    |  ,´  /           ´
            //    |,`   /             ,
            //    O    /
            //    |   /               ,
            //  d |  /
            //    | /                ,
            //    |/                .
            //    P
            //    |              ´
            //    |         , ´
            //    +-    ´
            //
            // have X
            // want to find E

            float viewDist = 1f + d;

            var projPos = CalcViewExtents(camera);
            var projHyp = Mathf.Sqrt(projPos.x * projPos.x + 1f);

            float cylDistMinusD = 1f / projHyp;
            float cylDist = cylDistMinusD + d;
            var cylPos = projPos * cylDistMinusD;

            return cylPos * (viewDist / cylDist);
        }

        #endregion
    }
}