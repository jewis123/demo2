using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using Object = System.Object;

namespace FrameGraph
{
    /// <summary>
    /// 降采样枚举
    /// </summary>
    public enum Downsampling
    {
        _1x = 0,
        _2x = 1,
        _4x = 2,
        _8x = 3,
        _16x = 4,
        _32x = 5,
        _64x = 6
    }
    
    /// <summary>
    /// Pass信息类
    /// </summary>
    public class PassInfo
    {
        /// <summary>
        /// 无效的资源
        /// </summary>
        public const int INVALID_RESOURCE = ResourceNode.NO_RESOURCE;
        
        /// <summary>
        /// 输入资源
        /// </summary>
        public int[] inputs { get; private set; }
        /// <summary>
        /// 输出资源
        /// </summary>
        public int[] outputs { get; private set; }
        /// <summary>
        /// 清除标识
        /// </summary>
        public ClearFlag clearFlag { get; private set; }
        /// <summary>
        /// 清除颜色
        /// </summary>
        public Color clearColor { get; private set; }
        /// <summary>
        /// Pass名字
        /// </summary>
        public string passName { get; private set; }
        /// <summary>
        /// 在哪个Tag的摄像机下运行，默认为空，表示任何摄像机下都能运行
        /// </summary>
        public string cameraTag { get; private set; }

        /// <summary>
        /// Pass构造函数的传参
        /// </summary>
        public Object[] contexts { get; private set; }

        /// <summary>
        /// 是否激活
        /// </summary>
        public bool actived { get; set; }

        public bool isPP { get; private set; }

        public PassInfo(int[] inputs, int[] outputs, ClearFlag clearFlag, Color clearColor, bool actived, string passName = "", string cameraTag = "", bool isPP = false, params Object[] contexts)
        {
            this.inputs = inputs;
            this.outputs = outputs;
            this.clearFlag = clearFlag;
            this.clearColor = clearColor;
            this.actived = actived;
            this.passName = passName;
            this.cameraTag = cameraTag;
            this.contexts = contexts;
            this.isPP = isPP;
        }
    }

    /// <summary>
    /// 渲染目标信息类
    /// </summary>
    public class RenderTargetInfo
    {
        /// <summary>
        /// 渲染目标类型
        /// </summary>
        public enum RenderTargetType
        {
            FrameBuffer,
            URPShadowMap,
            CustomRT,
            URPGradingLut
        }
        
        private static RenderTargetInfo k_FrameBuffer;
        
        /// <summary>
        /// 帧缓冲
        /// </summary>
        public static RenderTargetInfo FrameBuffer
        {
            get
            {
                if (k_FrameBuffer == null)
                {
                    k_FrameBuffer = new RenderTargetInfo("");
                    k_FrameBuffer.rtType = RenderTargetType.FrameBuffer;
                }

                return k_FrameBuffer;
            }
        }
        
        // private static RGRenderTargetInfo k_URPShadowMap;
        // public static RGRenderTargetInfo URPShadowMap
        // {
        //     get
        //     {
        //         if (k_FrameBuffer == null)
        //         {
        //             k_FrameBuffer = new RGRenderTargetInfo("");
        //             k_FrameBuffer.rtType = RTType.FrameBuffer;
        //         }
        //
        //         return k_FrameBuffer;
        //     }
        // }

        /// <summary>
        /// 以URP的配置创建ShadowMap的信息
        /// </summary>
        /// <param name="shaderProperty"></param>
        /// <returns></returns>
        public static RenderTargetInfo CreateShadowMap(string shaderProperty)
        {
            var rt = new RenderTargetInfo(shaderProperty);
            rt.rtType = RenderTargetType.URPShadowMap;
            return rt;
        }
        
        /// <summary>
        /// 以URP的配置创建GradingLut的信息
        /// </summary>
        /// <param name="shaderProperty"></param>
        /// <returns></returns>
        public static RenderTargetInfo CreateGradingLut(string shaderProperty)
        {
            var rt = new RenderTargetInfo(shaderProperty);
            rt.rtType = RenderTargetType.URPGradingLut;
            return rt;
        }
        
        /// <summary>
        /// 创建渲染目标信息
        /// </summary>
        /// <param name="shaderProperty"></param>
        /// <returns></returns>
        public static RenderTargetInfo CreateRenderTarget(string shaderProperty)
        {
            var rt = new RenderTargetInfo(shaderProperty);
            rt.rtType = RenderTargetType.CustomRT;
            return rt;
        }
        
        /// <summary>
        /// shader变量名
        /// </summary>
        public string shaderProperty { get; private set; }

        /// <summary>
        /// 渲染目标类型
        /// </summary>
        public RenderTargetType rtType { get; private set; }

        /// <summary>
        /// 降采样
        /// </summary>
        public Downsampling downsampling = Downsampling._1x;
        
        /// <summary>
        /// 像素宽度
        /// </summary>
        public int width
        {
            get
            {
                return m_Width;
            }
            set
            {
                m_Width = value;
                defaultWidth = false;
            }
        }
        
        /// <summary>
        /// 像素高度
        /// </summary>
        public int height
        {
            get
            {
                return m_Height;
            }
            set
            {
                m_Height = value;
                defaultHeight = false;
            }
        }
        
        /// <summary>
        /// 是否使用MipMap
        /// </summary>
        public bool useMipMap
        {
            get
            {
                return m_UseMipMap;
            }
            set
            {
                m_UseMipMap = value;
                defaultUseMipmap = false;
            }
        }
        
        /// <summary>
        /// 是否自动生成MipMap
        /// </summary>
        public bool autoGenerateMips
        {
            get
            {
                return m_AutoGenerateMips;
            }
            set
            {
                m_AutoGenerateMips = value;
                defaultAutoGenerateMips = false;
            }
        }
        
        /// <summary>
        /// 纹理格式
        /// </summary>
        public RenderTextureFormat colorFormat
        {
            get
            {
                return m_ColorFormat;
            }
            set
            {
                m_ColorFormat = value;
                defaultColorFormat = false;
            }
        }
        
        /// <summary>
        /// 图像格式
        /// </summary>
        public GraphicsFormat graphicsFormat
        {
            get
            {
                return m_GraphicsFormat;
            }
            set
            {
                m_GraphicsFormat = value;
                defaultGraphicsFormat = false;
            }
        }
        
        /// <summary>
        /// 深度缓冲位数
        /// </summary>
        public int depthBufferBits
        {
            get
            {
                return m_DepthBufferBits;
            }
            set
            {
                m_DepthBufferBits = value;
                defaultDepthBufferBits = false;
            }
        }

        /// <summary>
        /// 过滤模式
        /// </summary>
        public FilterMode filterMode;

        // 是否默认启用Camera的参数
        public bool defaultWidth { get; private set; }
        public bool defaultHeight{ get; private set; }
        public bool defaultUseMipmap{ get; private set; }
        public bool defaultAutoGenerateMips{ get; private set; }
        public bool defaultColorFormat{ get; private set; }
        
        public bool defaultGraphicsFormat{ get; private set; }
        public bool defaultDepthBufferBits{ get; private set; }
        
        private int m_Width;
        private int m_Height;
        private bool m_UseMipMap;
        private bool m_AutoGenerateMips;
        private RenderTextureFormat m_ColorFormat;
        private GraphicsFormat m_GraphicsFormat;
        private int m_DepthBufferBits;
        
        private RenderTargetInfo(string shaderProperty)
        {
            this.shaderProperty = shaderProperty;
            defaultWidth = true;
            defaultHeight = true;
            defaultUseMipmap = true;
            defaultAutoGenerateMips = true;
            defaultColorFormat = true;
            defaultDepthBufferBits = true;
            defaultGraphicsFormat = true;
            rtType = RenderTargetType.CustomRT;
        }
    }

    /// <summary>
    /// FrameGraph配置
    /// </summary>
    public class FrameGraphConfig
    {
        /// <summary>
        /// Pass名字对应ID
        /// </summary>
        private static readonly Dictionary<string, int> k_PassName2Id = new Dictionary<string, int>();

        /// <summary>
        /// Pass信息集合
        /// </summary>
        private static readonly Dictionary<int, PassInfo> k_Id2PassInfo = new Dictionary<int, PassInfo>();

        /// <summary>
        /// 渲染目标名字对应ID
        /// </summary>
        private static readonly Dictionary<string, int> k_RenderTargetProperty2Id = new Dictionary<string, int>();
        /// <summary>
        /// 渲染目标信息集合
        /// </summary>
        private static readonly Dictionary<int, RenderTargetInfo> k_Id2RenderTargetInfo = new Dictionary<int, RenderTargetInfo>();

        /// <summary>
        /// 注册指定RT，如果该RT存在则更新RT的信息
        /// </summary>
        /// <param name="id"></param>
        /// <param name="value"></param>
        public static void Set(int id, RenderTargetInfo value)
        {
            Set(id, k_Id2RenderTargetInfo, value);
            if (!string.IsNullOrEmpty(value.shaderProperty))
            {
                Set(value.shaderProperty, k_RenderTargetProperty2Id, id);
            }
        }

        /// <summary>
        /// 获取指定RT信息
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static RenderTargetInfo GetRenderTarget(int id)
        {
            return k_Id2RenderTargetInfo[id];
        }
        

        public static RenderTargetInfo GetRenderTarget(string renderTargetProperty)
        {
            int id;
            if (k_RenderTargetProperty2Id.TryGetValue(renderTargetProperty, out id))
            {
                return GetRenderTarget(id);
            }
            else
            {
                return null;
            }
        }

        public static void Set(int id, PassInfo pi)
        {
            Set(id, k_Id2PassInfo, pi);

            if (!string.IsNullOrEmpty(pi.passName))
            {
                Set(pi.passName, k_PassName2Id, id);
            }
        }
        
        public static PassInfo GetPass(int id)
        {
            return k_Id2PassInfo[id];
        }

        public static PassInfo GetPass(string passName)
        {
            int id;
            if (k_PassName2Id.TryGetValue(passName, out id))
            {
                return GetPass(id);
            }
            else
            {
                return null;
            }
        }

        private static void Set<T1, T2>(T1 id, Dictionary<T1, T2> dic, T2 value)
        {
            if (dic.ContainsKey(id))
            {
                dic[id] = value;
            }
            else
            {
                dic.Add(id, value);
            }
        }
    }
}