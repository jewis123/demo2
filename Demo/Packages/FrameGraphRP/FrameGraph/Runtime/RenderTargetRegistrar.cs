using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace FrameGraph
{
    /// <summary>
    /// 管理FrameGraph的渲染目标
    /// </summary>
    internal static class RenderTargetRegistrar
    {
        private static Dictionary<string, RenderTargetHandle> m_RenderTargetHandleDic = new Dictionary<string, RenderTargetHandle>();

        private static Dictionary<int, string> m_Id2Property = new Dictionary<int, string>();
        private static HashSet<int> m_AllocatedSets = new HashSet<int>();
        
        public static void Reset()
        {
            m_Id2Property.Clear();
            m_AllocatedSets.Clear();
            m_RenderTargetHandleDic.Clear();
        }
        
        public static bool IsRenderTargetRegistered(int id)
        {
            return m_Id2Property.ContainsKey(id);
        }

        public static RenderTargetHandle GetRenderTarget(int id)
        {
            return m_RenderTargetHandleDic[m_Id2Property[id]];
        }

        public static void CancelRenderTarget(int id)
        {
            m_Id2Property.Remove(id);
        }
        
        public static void RegisterRenderTarget(int id, string shaderProperty)
        {
            m_Id2Property.Add(id, shaderProperty);
            if (!m_RenderTargetHandleDic.ContainsKey(shaderProperty))
            {
                RenderTargetHandle targetHandle = new RenderTargetHandle();
                targetHandle.Init(shaderProperty);
                m_RenderTargetHandleDic.Add(shaderProperty, targetHandle);
            }
        }
        
        public static void RegisterCameraRenderTarget(int id)
        {
            m_Id2Property.Add(id, string.Empty);
            if (!m_RenderTargetHandleDic.ContainsKey(string.Empty))
            {
                RenderTargetHandle targetHandle = RenderTargetHandle.CameraTarget;
                m_RenderTargetHandleDic.Add(string.Empty, targetHandle);
            }
            SetRenderTargetAllocated(id, true);
        }

        public static void SwapRenderTarget(int id1, int id2)
        {
            (m_Id2Property[id1], m_Id2Property[id2]) = (m_Id2Property[id2], m_Id2Property[id1]);
        }
        
        public static bool IsRenderTargetAllocated(int id)
        {
            return m_AllocatedSets.Contains(id);
        }

        public static void SetRenderTargetAllocated(int id, bool allocated)
        {
            if (allocated)
            {
                m_AllocatedSets.Add(id);
            }
            else
            {
                m_AllocatedSets.Remove(id);
            }
        }
    }
}