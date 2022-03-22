using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using GraphProcessor;
using UnityEngine;
using UnityEngine.Rendering;

namespace FrameGraph
{
    public abstract class BasePassNode : FrameGraphBaseNode
    {
        /// <summary>
        /// Pass是否被激活
        /// </summary>
        [Parameter, HideInInspector] public bool actived = true;
        
        protected List<string> inputFilters
        {
            get
            {
                return new List<string>(customInputFilters);
            }
        }


        protected abstract string[] customInputFilters { get; }

        /// <summary>
        /// 该Pass的Input参数
        /// </summary>
        public List<BaseNode> inputs
        {
            get
            {
                var array = Reverse(inputPorts);
                
                var list = new List<BaseNode>();
                // 获取除开inputColor和inputDepth以外的所有输入参数
                foreach (var port in array)
                {
                    // 过滤掉inputColor和inputDepth
                    var fieldName = port.fieldName;
                    if (!inputFilters.Contains(fieldName))
                    {
                        var node = GetResourceRecursuvely(this, fieldName);
                        list.Add(node);
                    }
                }
                
                
                return list;
            }
        }

        /// <summary>
        /// 该Pass的Input参数的来源
        /// </summary>
        public List<BaseNode> sourcesOfInput
        {
            get
            {
                var array = Reverse(inputPorts);
                
                var list = new List<BaseNode>();
                // 获取除开inputColor和inputDepth以外的所有输入参数
                foreach (var port in array)
                {
                    // 过滤掉inputColor和inputDepth
                    var fieldName = port.fieldName;
                    if (!inputFilters.Contains(fieldName))
                    {
                        if (port.GetEdges().Count == 0)
                        {
                            list.Add(null);
                        }
                        else
                        {
                            foreach (var edge in port.GetEdges())
                            {
                                list.Add(GetValidNode(edge.outputNode));
                            }
                        }
                    }
                }
                
                return list;
            }
        }

        /// <summary>
        /// 该Pass的RenderTarget
        /// </summary>
        public List<BaseNode> renderTargets
        {
            get
            {
                var list = new List<BaseNode>();
                foreach (var f in inputFilters)
                {
                    list.Add(GetResourceRecursuvely(this, f)); // 这里的字符串和变量名保持一致
                }
                return list;
            }
        }

        /// <summary>
        /// 该Pass的RenderTarget的来源
        /// </summary>
        public List<BaseNode> sourcesOfRenderTarget
        {
            get
            {
                var list = new List<BaseNode>();
                
                var array = Reverse(inputPorts);
                
                foreach (var port in array)
                {
                    if (inputFilters.Contains(port.fieldName))
                    {
                        if (port.GetEdges().Count == 0)
                        {
                            list.Add(null);
                        }
                        else
                        {
                            foreach (var edge in port.GetEdges())
                            {
                                list.Add(GetValidNode(edge.outputNode));
                            }
                        }
                    }
                }

                return list;
            }
        }

        private BaseNode GetValidNode(BaseNode node)
        {
            while (node is RelayNode)
            {
                node = (node as RelayNode).inputPorts[0].GetEdges()[0].outputNode;
            }

            return node;
        }
        
        /// <summary>
        /// 找到Pass的指定RenderTarget的资源来源
        /// </summary>
        /// <param name="passNode"></param>
        /// <param name="rtName"></param>
        /// <returns></returns>
        protected BaseNode GetResourceRecursuvely(BaseNode passNode, string rtName)
        {
            BaseNode result = null;
            foreach (var port in passNode.inputPorts)
            {
                if (result != null)
                    break;

                if (port.fieldName.Equals(rtName))
                {
                    // Pass的RenderTarget输入来源只会有一个，所以这里直接取下标为0的edge
                    var edges = port.GetEdges();
                    if (edges.Count > 0)
                    {
                        var edge = edges[0];
                        var sourceNode = edge.outputNode;
                        
                        while (sourceNode is RelayNode)
                        {
                            // 如果是连接用的节点，直接往前寻找前序
                            var prevNode = sourceNode as RelayNode;
                            edge = prevNode.inputPorts[0].GetEdges()[0];
                            sourceNode = edge.outputNode;
                        }
                        
                        // 如果是资源节点，则该资源节点就是来源
                        if (sourceNode is BaseResourceNode)
                        {
                            result = sourceNode;
                        }
                        // 如果是Pass节点，则继续找这个Pass的输入源
                        else if (sourceNode is BasePassNode)
                        {
                            var prevPassNode = sourceNode as BasePassNode;
                            // 找到连线的端口在这个节点内的序号
                            int index = 0;
                            for (int i = 0; i < prevPassNode.outputPorts.Count; i++)
                            {
                                if (prevPassNode.outputPorts[i].Equals(edge.outputPort))
                                {
                                    // 因为outportPorts得到的顺序是子类最后一个字段开始到父类第一个字段，需要用数量减当前这个倒序的序号获得正序的序号
                                    index = prevPassNode.outputPorts.Count - i;
                                    break;
                                }
                            }
                            result = GetResourceRecursuvely(prevPassNode, prevPassNode.inputPorts[prevPassNode.inputPorts.Count - index].fieldName);    // 用数量减序号依旧为了保持顺序一致
                        }
                    }
                }
            }

            return result;
        }
        
        public abstract string scriptableRenderPass { get; }

        private NodePort[] Reverse(NodeInputPortContainer container)
        {
            // 从inputPorts或outputPorts获取到的字段顺序是先来自子类后来自父类，同一个类里的字段按照定义时的从下往上排列。所以这里需要颠倒下顺序以保证先父类和子类，同一类里从上往下的顺序
            var array = container.ToArray();
            Array.Reverse(array);
            return array;
        }

        protected PortData NewPortData(string id, string name, bool allowMulti)
        {
            return new PortData()
            {
                identifier = id,
                displayName = name,
                displayType = typeof(int),
                acceptMultipleEdges = allowMulti
            };
        }
        

    }
}