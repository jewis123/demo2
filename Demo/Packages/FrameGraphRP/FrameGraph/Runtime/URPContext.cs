using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace FrameGraph
{
    public class MaterialLibrary
    {
        public readonly Material stopNaN;
        public readonly Material subpixelMorphologicalAntialiasing;
        public readonly Material gaussianDepthOfField;
        public readonly Material bokehDepthOfField;
        public readonly Material cameraMotionBlur;
        public readonly Material paniniProjection;
        public readonly Material bloom;
        public readonly Material uber;
        public readonly Material finalPass;

        public MaterialLibrary(PostProcessData data)
        {
            stopNaN = Load(data.shaders.stopNanPS);
            subpixelMorphologicalAntialiasing = Load(data.shaders.subpixelMorphologicalAntialiasingPS);
            gaussianDepthOfField = Load(data.shaders.gaussianDepthOfFieldPS);
            bokehDepthOfField = Load(data.shaders.bokehDepthOfFieldPS);
            cameraMotionBlur = Load(data.shaders.cameraMotionBlurPS);
            paniniProjection = Load(data.shaders.paniniProjectionPS);
            bloom = Load(data.shaders.bloomPS);
            uber = Load(data.shaders.uberPostPS);
            finalPass = Load(data.shaders.finalPostPassPS);
        }

        public Material Load(Shader shader)
        {
            if (shader == null)
            {
                Debug.LogErrorFormat($"Missing shader. {GetType().DeclaringType.Name} render pass will not execute. Check for missing reference in the renderer resources.");
                return null;
            }
            else if (!shader.isSupported)
            {
                return null;
            }

            return CoreUtils.CreateEngineMaterial(shader);
        }

        public void Cleanup()
        {
            CoreUtils.Destroy(stopNaN);
            CoreUtils.Destroy(subpixelMorphologicalAntialiasing);
            CoreUtils.Destroy(gaussianDepthOfField);
            CoreUtils.Destroy(bokehDepthOfField);
            CoreUtils.Destroy(cameraMotionBlur);
            CoreUtils.Destroy(paniniProjection);
            CoreUtils.Destroy(bloom);
            CoreUtils.Destroy(uber);
            CoreUtils.Destroy(finalPass);
        }
    }

    /// <summary>
    /// 因为所有的Pass通过反射进行构建。所以每个Pass的构造函数必须包含且仅包含该类作为参数。它用于兼容URP的Pass构造
    /// 所有初始方法和URP保持一致
    /// </summary>
    public class URPContext
    {
        public FrameGraphRendererData data;
        public StencilStateData stencilData;
        public StencilState defaultStencilState;

        public Material blitMaterial;
        public Material copyDepthMaterial;
        public Material samplingMaterial;

        // public Material screenspaceShadowsMaterial;

        // public Material tileDepthInfoMaterial;
        // public Material tileDeferredMaterial;
        public Material stencilDeferredMaterial;

        public MaterialLibrary postProcessMaterials;

        public URPContext(FrameGraphRendererData data)
        {
            // 这部分的逻辑来在于URP的ForwardRenderer的构造函数
            this.data = data;

            SetMaterial(ref blitMaterial, data.shaders.blitPS, nameof(data.shaders.blitPS));
            SetMaterial(ref copyDepthMaterial, data.shaders.blitPS, nameof(data.shaders.copyDepthPS));
            SetMaterial(ref samplingMaterial, data.shaders.blitPS, nameof(data.shaders.samplingPS));
            SetMaterial(ref stencilDeferredMaterial, data.shaders.blitPS, nameof(data.shaders.stencilDeferredPS));
            
            //screenspaceShadowsMaterial = CoreUtils.CreateEngineMaterial(data.shaders.screenSpaceShadowPS);
            //m_TileDepthInfoMaterial = CoreUtils.CreateEngineMaterial(data.shaders.tileDepthInfoPS);
            //m_TileDeferredMaterial = CoreUtils.CreateEngineMaterial(data.shaders.tileDeferredPS);

            stencilData = data.defaultStencilState;
            defaultStencilState = StencilState.defaultValue;
            defaultStencilState.enabled = stencilData.overrideStencilState;
            defaultStencilState.SetCompareFunction(stencilData.stencilCompareFunction);
            defaultStencilState.SetPassOperation(stencilData.passOperation);
            defaultStencilState.SetFailOperation(stencilData.failOperation);
            defaultStencilState.SetZFailOperation(stencilData.zFailOperation);

            postProcessMaterials = new MaterialLibrary(data.postProcessData);
        }

        public void ClearUp()
        {
            CoreUtils.Destroy(blitMaterial);
            CoreUtils.Destroy(copyDepthMaterial);
            CoreUtils.Destroy(samplingMaterial);
            // CoreUtils.Destroy(screenspaceShadowsMaterial);
            // CoreUtils.Destroy(tileDepthInfoMaterial);
            // CoreUtils.Destroy(tileDeferredMaterial);
            CoreUtils.Destroy(stencilDeferredMaterial);
            
            postProcessMaterials.Cleanup();
        }

        private void SetMaterial(ref Material material, Shader shader, string shaderName)
        {
            if (shader == null)
            {
                Debug.LogError(string.Format("{0} missing in FrameGraphRendererData", shaderName));
            }
            else
            {
                blitMaterial = CoreUtils.CreateEngineMaterial(data.shaders.blitPS);
            }
        }
    }
}