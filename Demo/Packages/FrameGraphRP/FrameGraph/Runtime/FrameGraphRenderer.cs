using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.Rendering.Universal.Internal;


namespace FrameGraph
{
    public class FrameGraphRenderer : ScriptableRenderer
    {
        /// <summary>
        /// 为了复用创建RT的Pass和销毁RT的Pass而声明的池子
        /// </summary>
        /// <typeparam name="T"></typeparam>
        private class PassPool<T> where T : new()
        {
            private Stack<T> m_CachedStack = new Stack<T>();
            private Stack<T> m_UsedStack = new Stack<T>();

            public T GetPass()
            {
                T target;
                if (m_CachedStack.Count > 0)
                {
                    target = m_CachedStack.Pop();
                }
                else
                {
                    target = new T();
                }

                m_UsedStack.Push(target);
                return target;
            }

            public void RecycleAll()
            {
                while (m_UsedStack.Count > 0)
                {
                    m_CachedStack.Push(m_UsedStack.Pop());
                }
            }
        }

        /// <summary>
        /// 与URP保持一致的光照信息
        /// </summary>
        private ForwardLights m_ForwardLights;

        /// <summary>
        /// Pass序号列表
        /// </summary>
        private List<int> m_RegisteredPassIds = new List<int>();

        /// <summary>
        /// Pass对象列表
        /// </summary>
        private List<IFrameGraphRenderPass> m_RegisteredPasses = new List<IFrameGraphRenderPass>();

        /// <summary>
        /// 来自Asset文件的数据，这个形式和URP保持一致
        /// </summary>
        private FrameGraphRendererData m_RendererData;

        /// <summary>
        /// Asset文件操作句柄
        /// </summary>
        private IFrameGraphAssetHandle m_AssetHandle;

        /// <summary>
        /// Asset文件版本号，用于编辑器下动态修改
        /// </summary>
        private int m_AssetHashCode;

        /// <summary>
        /// 每个渲染目标对应使用它的最后的Pass
        /// </summary>
        private Dictionary<int, int> m_RT2LastPass = new Dictionary<int, int>();

        // 创建渲染目标和移除渲染目标的池子
        private PassPool<CreateRenderTargetPass> m_CreateRTPassPool = new PassPool<CreateRenderTargetPass>();
        private PassPool<RemoveRenderTargetPass> m_RemoveRTPassPool = new PassPool<RemoveRenderTargetPass>();

        /// <summary>
        /// 封装了来自URP参数的上下文，为了兼容URP的Pass
        /// </summary>
        private URPContext m_URPContext;

        // /// <summary>
        // /// 与URP保持一致的工具Pass
        // /// </summary>
        // private InvokeOnRenderObjectCallbackPassWrapper m_OnRenderObjectCallbackPass;

        public FrameGraphRenderer(FrameGraphRendererData data) : base(data)
        {
            m_RendererData = data;
            m_URPContext = new URPContext(m_RendererData);
            m_AssetHandle = m_RendererData.GraphAsset;
            m_ForwardLights = new ForwardLights();

            // m_OnRenderObjectCallbackPass = new InvokeOnRenderObjectCallbackPassWrapper();

            InitRenderer();

            supportedRenderingFeatures = new RenderingFeatures()
            {
                cameraStacking = true,
            };
        }

        public override void Setup(ScriptableRenderContext context, ref RenderingData renderingData)
        {
#if UNITY_EDITOR
            // 编辑器环境支持动态调整
            if (m_AssetHandle != null && m_AssetHandle.GetHashCode() != m_AssetHashCode)
            {
                ClearRenderPass();
                InitRenderer();
            }
#endif
            bool firstTimePassIsSteped = true;
            var cameraData = renderingData.cameraData;
            var camera = cameraData.camera;
            for (int i = 0, len = m_RegisteredPasses.Count; i < len; i++)
            {
                var passId = m_RegisteredPassIds[i];
                var pass = m_RegisteredPasses[i];
                var passInfo = FrameGraphConfig.GetPass(passId);


                // 取出Pass使用到的RT，检测RT是否被注册
                var inputs = passInfo.inputs;
                var outputs = passInfo.outputs;
                CheckRegisterRenderTarget(outputs);

                // 编辑器的相机全都执行（排查后发现编辑器Camera的enabled都为false，以后有误则修改此处）
                // Pass使用相机与当前相机一致时才执行Pass逻辑，为了确保后续逻辑的正确，RT依旧正常创建
                // Pass激活才会执行逻辑，为了确保后续逻辑的正确，RT依旧正常创建
                bool canExecute = passInfo.actived 
                                  && (!camera.enabled || string.IsNullOrEmpty(passInfo.cameraTag) || camera.CompareTag(passInfo.cameraTag)) 
                                  && pass.Setup(passId, ref renderingData);
             
                if (canExecute)
                {
                    var srpPass = pass.GetScriptableRenderPass();

                    if (passInfo.isPP)
                    {
                        // 后处理逻辑中设置RenderTarget的操作交由后处理自身
                        // 此外由于后处理设置渲染目标依靠个srp的ConfigureTarget。所以如果这里不重设下渲染目标会导致后续Pass没有识别到渲染目标有变化从而不重设渲染目标，这里重设下为了顺从SRP的框架
                        RenderTargetIdentifier color = outputs[0].ToRenderTargetHandle().Identifier();
                        srpPass.ConfigureTarget(color);
                    }
                    else
                    {
                        bool colorCreated = true;
                        bool depthCreated = true;
                        // 将RT设置为渲染目标
                        if (outputs.Length == 1) // 只有color
                        {
                            RenderTargetIdentifier color = outputs[0].ToRenderTargetHandle().Identifier();
                            if (firstTimePassIsSteped)
                            {
                                firstTimePassIsSteped = false;
                                ConfigureCameraTarget(color, RenderTargetHandle.CameraTarget.Identifier());
                            }
                            else
                            {
                                srpPass.ConfigureTarget(color);
                            }

                            colorCreated = RenderTargetRegistrar.IsRenderTargetAllocated(outputs[0]);
                            depthCreated = RenderTargetRegistrar.IsRenderTargetAllocated(outputs[0]);
                        }
                        else if (outputs.Length == 2) // 常态，一个是color，一个是depth
                        {
                            RenderTargetIdentifier color = outputs[0].ToRenderTargetHandle().Identifier();
                            RenderTargetIdentifier depth = outputs[1].ToRenderTargetHandle().Identifier();
                            if (firstTimePassIsSteped)
                            {
                                firstTimePassIsSteped = false;
                                ConfigureCameraTarget(color, depth);
                            }
                            else
                            {
                                srpPass.ConfigureTarget(color, depth);
                            }

                            colorCreated = RenderTargetRegistrar.IsRenderTargetAllocated(outputs[0]);
                            depthCreated = RenderTargetRegistrar.IsRenderTargetAllocated(outputs[1]);
                        }
                        else // TODO MRT
                        {
                        }
                        
                        // 配置清除标记
                        if (colorCreated && depthCreated)
                        {
                            srpPass.ConfigureClear(passInfo.clearFlag, passInfo.clearColor);
                        }
                        else
                        {
                            srpPass.ConfigureClear((colorCreated ? ClearFlag.None : ClearFlag.Color) | (depthCreated ? ClearFlag.None : ClearFlag.Depth), Color.clear);
                        }
                    }
                    

                    // 检测RT是否被创建。没有被创建则插入创建RT的Pass
                    CheckCreatePass(outputs);
                    // 添加到待执行队列
                    pass.ConfigureBeforeEnqueued();
                    EnqueuePass(srpPass);
                }

                

                if (renderingData.cameraData.resolveFinalTarget) // 防止stack的最后一个camera还没执行就把中间的rt给释放了
                {
                    // 如果Pass的输入输出RT不是FrameBuffer，且该RT后续没有再被使用过，插入回收RT的Pass
                    CheckRemovePass(inputs, passId, cameraData.resolveFinalTarget);
                    CheckRemovePass(outputs, passId, cameraData.resolveFinalTarget);
                }
            }

            // // 这里和URP保持一致，为了触发MonoBehaviour的绘制完成事件
            // // 执行顺序和URP有差别，URP在绘制完半透明物体后，执行后处理之前调用。此处是等所有Pass执行完毕才执行
            // EnqueuePass(m_OnRenderObjectCallbackPass);

            // 回收所有创建和移除RT的Pass到池子
            m_CreateRTPassPool.RecycleAll();
            m_RemoveRTPassPool.RecycleAll();
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            m_URPContext.ClearUp();
            ClearRenderPass();
        }

        /// <summary>
        /// 注册渲染Pass
        /// </summary>
        /// <param name="id"></param>
        /// <param name="pass"></param>
        public void RegisterRenderPass(int id, IFrameGraphRenderPass pass)
        {
            m_RegisteredPassIds.Add(id);
            m_RegisteredPasses.Add(pass);
        }

        public void ClearRenderPass()
        {
            m_RegisteredPassIds.Clear();
            m_RegisteredPasses.Clear();
            foreach (var pass in m_RegisteredPasses)
            {
                pass.ClearUp();
            }
        }

        public override void SetupLights(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            // 这里和URP的逻辑一致
            m_ForwardLights.Setup(context, ref renderingData);
        }

        public override void SetupCullingParameters(ref ScriptableCullingParameters cullingParameters, ref CameraData cameraData)
        {
            // 这里和URP的逻辑一致
            // TODO: PerObjectCulling also affect reflection probes. Enabling it for now.
            // if (asset.additionalLightsRenderingMode == LightRenderingMode.Disabled ||
            //     asset.maxAdditionalLightsCount == 0)
            // {
            //     cullingParameters.cullingOptions |= CullingOptions.DisablePerObjectCulling;
            // }

            // We disable shadow casters if both shadow casting modes are turned off
            // or the shadow distance has been turned down to zero
            bool isShadowCastingDisabled = !UniversalRenderPipeline.asset.supportsMainLightShadows &&
                                           !UniversalRenderPipeline.asset.supportsAdditionalLightShadows;
            bool isShadowDistanceZero = Mathf.Approximately(cameraData.maxShadowDistance, 0.0f);
            if (isShadowCastingDisabled || isShadowDistanceZero)
            {
                cullingParameters.cullingOptions &= ~CullingOptions.ShadowCasters;
            }

            // if (this.actualRenderingMode == RenderingMode.Deferred)
            //     cullingParameters.maximumVisibleLights = 0xFFFF;
            // else
            {
                // We set the number of maximum visible lights allowed and we add one for the mainlight...
                cullingParameters.maximumVisibleLights = UniversalRenderPipeline.maxVisibleAdditionalLights + 1;
            }
            cullingParameters.shadowDistance = cameraData.maxShadowDistance;
        }

        private void CheckRegisterRenderTarget(int[] rts)
        {
            foreach (var rt in rts)
            {
                if (rt.IsValidRenderTarget())
                {
                    var rtInfo = FrameGraphConfig.GetRenderTarget(rt);
                    if (!RenderTargetRegistrar.IsRenderTargetRegistered(rt))
                    {
                        switch (rtInfo.rtType)
                        {
                            case RenderTargetInfo.RenderTargetType.FrameBuffer:
                                // RT是FrameBuffer，直接关联到CameraRenderTarget
                                RenderTargetRegistrar.RegisterCameraRenderTarget(rt);
                                break;
                            case RenderTargetInfo.RenderTargetType.URPShadowMap:
                            case RenderTargetInfo.RenderTargetType.URPGradingLut:
                            case RenderTargetInfo.RenderTargetType.CustomRT:
                                // RT不是FrameBuffer，且RT池中没有被创建过，先插入创建RT的Pass
                                RenderTargetRegistrar.RegisterRenderTarget(rt, rtInfo.shaderProperty);
                                break;
                        }
                    }
                }
            }
        }

        /// <summary>
        ///  检测是否需要创建指定的RT
        /// </summary>
        /// <param name="rts"></param>
        private void CheckCreatePass(int[] rts)
        {
            foreach (var rt in rts)
            {
                if (rt.IsValidRenderTarget())
                {
                    var rtInfo = FrameGraphConfig.GetRenderTarget(rt);
                    if (!RenderTargetRegistrar.IsRenderTargetAllocated(rt))
                    {
                        switch (rtInfo.rtType)
                        {
                            case RenderTargetInfo.RenderTargetType.URPShadowMap:
                            case RenderTargetInfo.RenderTargetType.URPGradingLut:
                            case RenderTargetInfo.RenderTargetType.CustomRT:
                                var createRTPass = m_CreateRTPassPool.GetPass();
                                createRTPass.SetupRenderTarget(rt);
                                EnqueuePass(createRTPass);
                                RenderTargetRegistrar.SetRenderTargetAllocated(rt, true);
                                break;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 检测是否需要释放指定的rt
        /// </summary>
        /// <param name="rts"></param>
        /// <param name="passId"></param>
        private void CheckRemovePass(int[] rts, int passId, bool resolveFinalTarget)
        {
            foreach (var rt in rts)
            {
                if (rt.IsValidRenderTarget())
                {
                    var rtInfo = FrameGraphConfig.GetRenderTarget(rt);
                    int lastPassId;
                    if (rtInfo.rtType != RenderTargetInfo.RenderTargetType.FrameBuffer &&
                        m_RT2LastPass.TryGetValue(rt, out lastPassId))
                    {
                        // 当前Pass就是最后一次被使用的Pass，插入回收RT的Pass
                        if (lastPassId == passId)
                        {
                            if (RenderTargetRegistrar.IsRenderTargetAllocated(rt))
                            {
                                var removeRTPass = m_RemoveRTPassPool.GetPass();
                                removeRTPass.SetupRenderTarget(rt);
                                EnqueuePass(removeRTPass);
                                RenderTargetRegistrar.SetRenderTargetAllocated(rt, false);
                                
                                RenderTargetRegistrar.CancelRenderTarget(rt);
                            }

                            // if (RenderTargetRegistrar.IsRenderTargetRegistered(rt) && resolveFinalTarget)
                            // {
                            //     if (resolveFinalTarget) // 栈摄像机非最后一个不清注册信息，为了后续栈中的其他摄像机使用
                            //         RenderTargetRegistrar.CancelRenderTarget(rt);
                            // }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 初始化
        /// </summary>
        private void InitRenderer()
        {
            var asset = m_AssetHandle;
            if (asset != null)
            {
                RenderTargetRegistrar.Reset();

                m_AssetHashCode = asset.GetHashCode();
                // 遍历资源列表，取出资源的描述注册到全局配置
                var reses = asset.GetRenderTargets();
                foreach (var id in reses)
                {
                    // 获取渲染目标信息
                    var rtInfo = asset.GetRenderTargetInfo(id);

                    // 注册到全局
                    FrameGraphConfig.Set(id, rtInfo);
                }

                // 注册Pass和资源的关联，生成Pass类
                var passes = asset.GetSortedPasses();
                foreach (var id in passes)
                {
                    // 获取Pass信息
                    var pi = asset.GetPassInfo(id);

                    // 注册到全局
                    FrameGraphConfig.Set(id, pi);

                    // 创建兼容URP的可编程渲染Pass
                    var scriptableRenderPass = asset.CreateFrameGraphRenderPass(id, m_URPContext, pi);

                    // 注册渲染Pass
                    RegisterRenderPass(id, scriptableRenderPass);
                }

                m_RT2LastPass = asset.GetRenderTarget2LastPass();
            }
        }
    }
}