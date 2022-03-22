using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace FrameGraph.Example
{
    public class FrameGraphRendererTest : MonoBehaviour
    {
        IEnumerator Start()
        {
            yield return new WaitForSeconds(2);
            int colorAttachment = 1;
            int depthAttachment = 2;

            int testpass = 1;

            var rt1 = RenderTargetInfo.CreateRenderTarget("_CameraColorTexture");
            rt1.width = 1136;
            rt1.height = 640;
            rt1.colorFormat = RenderTextureFormat.Default;
            
            var rt2 = RenderTargetInfo.CreateRenderTarget("_CameraDepthAttachment");
            rt2.width = 1136;
            rt2.height = 640;
            rt2.colorFormat = RenderTextureFormat.Depth;
            
            FrameGraphConfig.Set(colorAttachment, rt1);
            FrameGraphConfig.Set(depthAttachment, rt2);

            var pi = new PassInfo(new int[] { }, new[] {colorAttachment, depthAttachment}, ClearFlag.All, Color.black, true);
            FrameGraphConfig.Set(testpass, pi);

            var rgr = UniversalRenderPipeline.asset.scriptableRenderer as FrameGraphRenderer;
            rgr.RegisterRenderPass(testpass, new TestPass());
        }

        private void OnGUI()
        {
            if (GUI.Button(new Rect(10, 10, 200, 80), "修改rt大小"))
            {
                var rtInfo = FrameGraphConfig.GetRenderTarget(1);
                rtInfo.width = 2272;
                rtInfo.height = 1280;
            }

            if (GUI.Button(new Rect(10, 110, 200, 80), "开关Pass"))
            {
                FrameGraphConfig.GetPass(1).actived = !FrameGraphConfig.GetPass(1).actived;
            }
        }
    }
   

    public class TestPass : FrameGraphScriptableRenderPass
    {
        private RenderTargetInfo colorInfo;
        private RenderTargetInfo depthInfo;

        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            Debug.Log("******************** 开始执行渲染逻辑 ********************");
            Debug.Log(string.Format("渲染目标{0}和{1}", colorInfo.shaderProperty, depthInfo.shaderProperty));
            Debug.Log(string.Format("{0}的宽：{1}，高：{2}", colorInfo.shaderProperty, colorInfo.width, colorInfo.height));
            Debug.Log(string.Format("{0}的宽：{1}，高：{2}", depthInfo.shaderProperty, depthInfo.width, depthInfo.height));
            Debug.Log("执行渲染逻辑");
            Debug.Log("******************** 渲染逻辑执行完毕 ********************");
        }

        protected override bool Setup(int[] inputs, int[] outputs, ref RenderingData renderingData)
        {
            colorInfo = FrameGraphConfig.GetRenderTarget(outputs[0]);
            depthInfo = FrameGraphConfig.GetRenderTarget(outputs[1]);
            return true;
        }

        public override void ClearUp()
        {
            
        }
    }
}