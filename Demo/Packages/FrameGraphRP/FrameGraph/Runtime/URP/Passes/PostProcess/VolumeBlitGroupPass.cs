using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace FrameGraph
{
    /// <summary>
    /// 配合BlitGroup使用
    /// </summary>
    public class VolumeBlitGroupPass : BasePostProcessPass
    {
        [Serializable]
        public class Settings
        {
            public string tag = "";
        }
        
        private BlitGroup m_BlitGroup;
        private List<MaterialDescription> m_Descs;
        private VolumeBlitGroupPass.Settings m_Settings;

        public VolumeBlitGroupPass(URPContext context, BasePostProcessPass.Settings settings, VolumeBlitGroupPass.Settings settings2) : base(context, settings, "BlitGroup")
        {
            m_Descs = new List<MaterialDescription>();
            m_Settings = settings2;
        }

        protected override void OnProcess(CommandBuffer cmd, ScriptableRenderContext context, ref RenderingData renderingData)
        {
            for (int i = 0, cnt = m_Descs.Count; i < cnt; i++)
            {
                var desc = m_Descs[i];
                var s = i % 2 == 0 ? source.Identifier() : destination.Identifier();
                var d = i % 2 == 0 ? destination.Identifier() : source.Identifier();
                if (string.IsNullOrEmpty(desc.sourceTexName))
                {
                    cmd.Blit(s, d, desc.material, desc.passIndex);
                }
                else
                {
                    FrameGraphPostProcessUtils.Blit(cmd, s, d, desc.material, desc.passIndex);
                }
            }
        }

        protected override bool OnSetup(int[] inputs, int[] outputs, ref RenderingData renderingData)
        {
            var stack = VolumeManager.instance.stack;
            m_BlitGroup = stack.GetComponent<BlitGroup>();
            if (renderingData.cameraData.postProcessEnabled && !renderingData.cameraData.isSceneViewCamera && m_BlitGroup.IsActive())
            {
                m_Descs.Clear();
                foreach (var blitDesc in m_BlitGroup.blitDescriptions.value)
                {
                    if (blitDesc.tag.Equals(m_Settings.tag))
                    {
                        foreach (var desc in blitDesc.materialDescriptions)
                        {
                            if (desc.material != null)
                            {
                                m_Descs.Add(desc);
                            }
                        }
                    }
                }
            
                return m_Descs.Count > 0;
            }
            else
            {
                return false;
            }
        }


        public override void ConfigureBeforeEnqueued()
        {
            if (settings.swap && m_Descs.Count % 2 == 1)
            {
                SwapRenderTarget();
            }
        }
    }
}