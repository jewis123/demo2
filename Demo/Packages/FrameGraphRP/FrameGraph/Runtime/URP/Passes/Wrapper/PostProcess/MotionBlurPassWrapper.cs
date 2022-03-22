using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace FrameGraph
{
    /// <summary>
    /// 从PostProcessPass中剥离出来的动态模糊部分
    /// </summary>
    public class MotionBlurPassWrapper : BasePostProcessPass
    {
        private MotionBlur m_MotionBlur;
        
        private Matrix4x4[] m_PrevViewProjM = new Matrix4x4[2];
        private bool m_ResetHistory;
        
        private RenderTargetHandle m_DepthTex;
        
        public MotionBlurPassWrapper(URPContext context, BasePostProcessPass.Settings settings) : base(context, settings, "MotionBlur")
        {
            m_ResetHistory = true;
        }

        protected override void OnProcess(CommandBuffer cmd, ScriptableRenderContext context, ref RenderingData renderingData)
        {
            DoMotionBlur(renderingData.cameraData, cmd, source.id, destination.id);
        }

        protected override void OnProcessFinish()
        {
            m_ResetHistory = false;
        }

        protected override bool OnSetup(int[] inputs, int[] outputs, ref RenderingData renderingData)
        {
            var stack = VolumeManager.instance.stack;
            m_MotionBlur = stack.GetComponent<MotionBlur>();
            m_DepthTex = inputs[1].ToRenderTargetHandle();
            return renderingData.cameraData.postProcessEnabled && m_MotionBlur.IsActive() && !renderingData.cameraData.isSceneViewCamera;
        }

        #region Motion Blur

#if ENABLE_VR && ENABLE_XR_MODULE
        // Hold the stereo matrices to avoid allocating arrays every frame
        internal static readonly Matrix4x4[] viewProjMatrixStereo = new Matrix4x4[2];
#endif
        void DoMotionBlur(CameraData cameraData, CommandBuffer cmd, int source, int destination)
        {
            var material = context.postProcessMaterials.cameraMotionBlur;

#if ENABLE_VR && ENABLE_XR_MODULE
            if (cameraData.xr.enabled && cameraData.xr.singlePassEnabled)
            {
                var viewProj0 = GL.GetGPUProjectionMatrix(cameraData.GetProjectionMatrix(0), true) * cameraData.GetViewMatrix(0);
                var viewProj1 = GL.GetGPUProjectionMatrix(cameraData.GetProjectionMatrix(1), true) * cameraData.GetViewMatrix(1);
                if (m_ResetHistory)
                {
                    viewProjMatrixStereo[0] = viewProj0;
                    viewProjMatrixStereo[1] = viewProj1;
                    material.SetMatrixArray("_PrevViewProjMStereo", viewProjMatrixStereo);
                }
                else
                    material.SetMatrixArray("_PrevViewProjMStereo", m_PrevViewProjM);

                m_PrevViewProjM[0] = viewProj0;
                m_PrevViewProjM[1] = viewProj1;
            }
            else
#endif
            {
                int prevViewProjMIdx = 0;
#if ENABLE_VR && ENABLE_XR_MODULE
                if (cameraData.xr.enabled)
                    prevViewProjMIdx = cameraData.xr.multipassId;
#endif
                // This is needed because Blit will reset viewproj matrices to identity and UniversalRP currently
                // relies on SetupCameraProperties instead of handling its own matrices.
                // TODO: We need get rid of SetupCameraProperties and setup camera matrices in Universal
                var proj = cameraData.GetProjectionMatrix();
                var view = cameraData.GetViewMatrix();
                var viewProj = proj * view;

                material.SetMatrix("_ViewProjM", viewProj);

                if (m_ResetHistory)
                    material.SetMatrix("_PrevViewProjM", viewProj);
                else
                    material.SetMatrix("_PrevViewProjM", m_PrevViewProjM[prevViewProjMIdx]);

                m_PrevViewProjM[prevViewProjMIdx] = viewProj;
            }

            cmd.SetGlobalTexture(FrameGraphConstant.ShaderConstants._CameraDepthTexture, m_DepthTex.id);
            material.SetFloat("_Intensity", m_MotionBlur.intensity.value);
            material.SetFloat("_Clamp", m_MotionBlur.clamp.value);

            FrameGraphPostProcessUtils.SetSourceSize(cmd, descriptor);

            FrameGraphPostProcessUtils.Blit(cmd, source, FrameGraphPostProcessUtils.BlitDstDiscardContent(cmd, destination), material, (int) m_MotionBlur.quality.value);
        }

        #endregion
    }
}