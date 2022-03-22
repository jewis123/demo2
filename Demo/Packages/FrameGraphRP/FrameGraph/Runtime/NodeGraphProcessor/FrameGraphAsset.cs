using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using GraphProcessor;
using UnityEngine;
using UnityEngine.Rendering;
using Object = System.Object;

namespace FrameGraph
{
    [System.Serializable]
    public class FrameGraphAsset : BaseGraph, IFrameGraphAssetHandle
    {
        private FrameGraphAsset()
        {
        }

        [SerializeReference] private string[] m_CameraUsedByPass;

        [SerializeReference] private BaseNode[] m_Pass2Node;

        [SerializeReference] private BaseNode[] m_Resource2Node;

        [SerializeReference] private FrameGraphCompiler.Result m_Result;

        [SerializeReference] private FrameGraph m_FrameGraph;

        public void Test()
        {
            Debug.Log("m_Pass2Node " + m_Pass2Node);
            foreach (var node in m_Pass2Node)
            {
                Debug.Log("node " + node);
            }

            Debug.Log("m_Resource2Node " + m_Resource2Node);
            foreach (var node in m_Resource2Node)
            {
                Debug.Log("node " + node);
            }


            Debug.Log("m_FrameGraph " + m_FrameGraph);
            var e = m_FrameGraph.GetPassNodes();
            while (e.MoveNext())
            {
                var current = e.Current;
                Debug.Log("=============================== " + current.id + " current pass " + current.name);
                foreach (var input in current.inputReses)
                {
                    if (input > -1)
                        Debug.Log(input + " input " + m_FrameGraph.GetResourceNode(input).name);
                }
                foreach (var output in current.outputReses)
                {
                    if (output > -1)
                        Debug.Log(output + " output " + m_FrameGraph.GetResourceNode(output).name);
                }
            }

            Debug.Log("m_Result " + m_Result);
            foreach (var pass in m_Result.sortedPasses)
            {
                Debug.Log("pass " + m_FrameGraph.GetPassNode(pass).name);
            }

            Debug.Log("m_RenderGraph " + m_FrameGraph);
            Debug.Log("m_RenderGraph.passCount " + m_FrameGraph.passCount);
            Debug.Log("m_RenderGraph.resourceCount " + m_FrameGraph.resourceCount);
        }

        public void Test2()
        {
            if (m_Result != null)
            {
                for (int i = 0; i < m_Result.res2Info.Length; i++)
                {
                    if (m_Result.res2Info[i] != null)
                    {
                        // 创建的RT才记录生命周期
                        var node = m_Resource2Node[i];
                        if (node is BaseResourceNode && !(node is CameraNode))
                        {
                            Debug.Log(string.Format("======================== 当前资源：{0}，最后的Pass是：{1} ==========", m_FrameGraph.GetResourceNode(i).name, m_FrameGraph.GetPassNode(m_Result.res2Info[i].lastPass).name));

                            foreach (var index in m_Result.res2Info[i].trimmedSortedList)
                            {
                                Debug.Log(m_FrameGraph.GetPassNode(index).name);
                            }
                        }
                    }
                }
            }
            
            // var last2Pass = GetRenderTarget2LastPass();
            // foreach (var kvp in last2Pass)
            // {
            //     Debug.Log(string.Format("{2} 资源{0}最后一次使用是在Pass {1}", m_FrameGraph.GetResourceNode(kvp.Key).name, m_FrameGraph.GetPassNode(kvp.Value).name, kvp.Key));
            // }
        }

        public override int GetHashCode()
        {
            var h1 = m_Pass2Node == null ? 0 : m_Pass2Node.GetHashCode();
            var h2 = m_Resource2Node == null ? 0 : m_Resource2Node.GetHashCode();
            var h3 = m_Result == null ? 0 : m_Result.GetHashCode();
            var h4 = m_FrameGraph == null ? 0 : m_FrameGraph.GetHashCode();
            var h5 = m_CameraUsedByPass == null ? 0 : m_CameraUsedByPass.GetHashCode();
            return h1 + h2 + h3 + h4 + h5;
        }

        public void Reset()
        {
            m_Pass2Node = null;
            m_Resource2Node = null;
            m_Result = null;
            m_FrameGraph = null;
            m_CameraUsedByPass = null;
        }

        public void InitPassSize(int size)
        {
            m_Pass2Node = new BaseNode[size];
            m_CameraUsedByPass = new string[size];
        }

        public void SetPass(int id, BaseNode node, string cameraTag)
        {
            m_Pass2Node[id] = node;
            m_CameraUsedByPass[id] = cameraTag;
        }

        public void InitResourceSize(int size)
        {
            m_Resource2Node = new BaseNode[size];
        }

        public void SetResource(int id, BaseNode node)
        {
            m_Resource2Node[id] = node;
        }

        public void Compile(FrameGraph fg)
        {
            m_FrameGraph = fg;
            FrameGraphCompiler compiler = new FrameGraphCompiler();
            m_Result = compiler.Compile(m_FrameGraph);
        }

        public List<int> GetSortedPasses()
        {
            if (m_Result != null)
            {
                return m_Result.sortedPasses;
            }
            else
            {
                return new List<int>();
            }
        }

        public List<int> GetRenderTargets()
        {
            List<int> reses = new List<int>();
            if (m_Result != null)
            {
                for (int i = 0; i < m_Result.res2Info.Length; i++)
                {
                    if (m_Result.res2Info[i] != null)
                        reses.Add(i);
                }
            }

            return reses;
        }

        public RenderTargetInfo GetRenderTargetInfo(int id)
        {
            RenderTargetInfo renderTargetInfo = null; 
            var node = m_Resource2Node[id];
            // 帧缓冲
            if (node is CameraNode)
            {
                renderTargetInfo = RenderTargetInfo.FrameBuffer;
            }
            // 根据URP配置生成的ShadowMap
            else if (node is URPShadowMapNode)
            {
                renderTargetInfo = RenderTargetInfo.CreateShadowMap(node.GetValidName());
            }
            // 根据URP配置生成的GradingLut
            else if (node is URPGradingLutNode)
            {
                renderTargetInfo = RenderTargetInfo.CreateGradingLut(node.GetValidName());
            }
            // 一般情况下的RT
            else if (node is CreateRenderTargetNode)
            {
                CreateRenderTargetNode createRenderTargetNode = node as CreateRenderTargetNode;
                RenderTargetInfo rtInfo = RenderTargetInfo.CreateRenderTarget(createRenderTargetNode.GetValidName());

                SetField(createRenderTargetNode, rtInfo, "width");
                SetField(createRenderTargetNode, rtInfo, "height");
                SetField(createRenderTargetNode, rtInfo, "downsampling");
                SetField(createRenderTargetNode, rtInfo, "useMipMap");
                SetField(createRenderTargetNode, rtInfo, "autoGenerateMips");
                SetField(createRenderTargetNode, rtInfo, "colorFormat");
                SetField(createRenderTargetNode, rtInfo, "graphicsFormat");
                SetField(createRenderTargetNode, rtInfo, "depthBufferBits");
                SetField(createRenderTargetNode, rtInfo, "filterMode");
                renderTargetInfo = rtInfo;
            }

            return renderTargetInfo;
        }

        private void SetField(CreateRenderTargetNode createRenderTargetNode, RenderTargetInfo rtInfo, string fieldName)
        {
            if (createRenderTargetNode.customFields.Contains(fieldName)) // 这里命名和createRenderTargetNode的变量名保持一致
            {
                // 用反射为了简化写法，因为只在初始化时执行一次
                var value = createRenderTargetNode.GetType().GetField(fieldName).GetValue(createRenderTargetNode);
                var property = rtInfo.GetType().GetProperty(fieldName);
                if (property != null)
                    property.SetValue(rtInfo, value);
                var field = rtInfo.GetType().GetField(fieldName);
                if (field != null)
                    field.SetValue(rtInfo, value);
            }
        }

        public PassInfo GetPassInfo(int id)
        {
            var node = m_FrameGraph.GetPassNode(id);
            var inputs = node.inputReses;
            var outputs = node.outputReses;
            var passNode = m_Pass2Node[id] as BasePassNode;
            var cameraTag = m_CameraUsedByPass[id];
            var passName = passNode.GetValidName();

            // 找到带有Context特性的字段作为构造函数的传参
            var fieldInfos = passNode.GetType().GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            List<Object> objects = new List<object>();
            foreach (var fieldInfo in fieldInfos)
            {
                var attribute = fieldInfo.GetCustomAttribute(typeof(ContextAttribute)) as ContextAttribute;
                if (attribute != null)
                {
                    // 这里插到队首是因为获取的字段是从子类到父类排列，且后定义的排在前面，插到队首是为了获得以下顺序：父类先于子类，先定义先于后定义的
                    objects.Insert(0, fieldInfo.GetValue(passNode));
                }
            }

            bool isPP = passNode is BasePostProcessPassNode;
            PassInfo pi;
            if (isPP)
            {
                pi = new PassInfo(inputs, outputs, ClearFlag.None, Color.black, passNode.actived, passName, cameraTag, isPP,objects.ToArray());
            }
            else
            {
                var colorPassNode = passNode as BaseColorPassNode;
                pi = new PassInfo(inputs, outputs, colorPassNode.clearFlag, colorPassNode.clearColor, passNode.actived, passName, cameraTag, isPP,objects.ToArray());
            }

            return pi;
        }

        public IFrameGraphRenderPass CreateFrameGraphRenderPass(int id, URPContext context, PassInfo pi)
        {
            var node = m_Pass2Node[id] as BasePassNode;
            if (pi.contexts.Length > 0)
            {
                Object[] args = new object[pi.contexts.Length + 1];
                Array.Copy(pi.contexts, 0, args, 1, pi.contexts.Length);
                args[0] = context;
                return Activator.CreateInstance(System.Type.GetType(node.scriptableRenderPass), args) as IFrameGraphRenderPass;
            }
            else
            {
                return Activator.CreateInstance(System.Type.GetType(node.scriptableRenderPass), context) as IFrameGraphRenderPass;
            }
        }

        public Dictionary<int, int> GetRenderTarget2LastPass()
        {
            Dictionary<int, int> dic = new Dictionary<int, int>();
            if (m_Result != null)
            {
                for (int i = 0; i < m_Result.res2Info.Length; i++)
                {
                    if (m_Result.res2Info[i] != null)
                    {
                        // 创建的RT才记录生命周期
                        var node = m_Resource2Node[i];
                        if (node is BaseResourceNode && !(node is CameraNode))
                        {
                            dic.Add(i, m_Result.res2Info[i].lastPass);
                        }
                    }
                }
            }

            return dic;
        }
    }
}