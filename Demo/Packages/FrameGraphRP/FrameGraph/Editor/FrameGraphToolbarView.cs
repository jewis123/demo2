using System.Collections.Generic;
using GraphProcessor;
using NUnit.Framework;
using FrameGraph;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using Status = UnityEngine.UIElements.DropdownMenuAction.Status;

namespace FrameGraph
{
    public class FrameGraphToolbarView : ToolbarView
    {
        ToolbarButtonData showParameters;

        public FrameGraphToolbarView(BaseGraphView graphView) : base(graphView)
        {

        }

        protected override void AddButtons()
        {
            AddButton("Test", () =>
            {
                var asset = graphView.graph as FrameGraphAsset;
                asset.Test();
            });
            AddButton("Test2", () =>
            {
                var asset = graphView.graph as FrameGraphAsset;
                asset.Test2();
            });
            AddButton("Compile", () =>
            {
                var asset = graphView.graph as FrameGraphAsset;
                asset.Reset();

                // 取出所有graph中的Node
                var nodes = graphView.graph.nodes;
                var node2Group = new Dictionary<BaseNode, string>();
                var nameSets = new HashSet<string>();
                
                foreach (var node in nodes)
                {
                    node2Group.Add(node, string.Empty);
                    var n = node.GetValidName();
                    if (!nameSets.Add(n))
                    {
                        Debug.LogWarning(string.Format("存在相同的节点名字：{0}，这会影响运行时通过名字索引节点的结果", n));
                    }
                }
                
                // 找到每个Node所在的Group
                foreach (var group in graphView.graph.groups)
                {
                    foreach (var nodeGUID in group.innerNodeGUIDs)
                    {
                        foreach (var node in nodes)
                        {
                            if (node.GUID.Equals(nodeGUID))
                            {
                                node2Group[node] = group.title;
                                break;
                            }
                        }
                    }
                }

                // 从Node中筛选出Pass, Resource
                List<BasePassNode> passNodes = new List<BasePassNode>();
                List<BaseNode> resourceNodes = new List<BaseNode>();
                foreach (var node in nodes)
                {
                    if (node is BasePassNode)
                    {
                        passNodes.Add(node as BasePassNode);
                    }
                    else if (node is BaseResourceNode)
                    {
                        resourceNodes.Add(node);
                    }
                }

                // 将所有Pass和Resource注册为ID，并建立Pass和Resource与ID的映射
                Dictionary<BaseNode, int> node2Pass = new Dictionary<BaseNode, int>();
                Dictionary<BaseNode, int> node2Resource = new Dictionary<BaseNode, int>();
                FrameGraph fg = new FrameGraph();
                // 建立Resource的映射
                foreach (var node in resourceNodes)
                {
                    var id = fg.RegisterResourceNode(node.GetValidName());
                    node2Resource.Add(node, id);
                }

                asset.InitResourceSize(fg.resourceCount);
                foreach (var kvp in node2Resource)
                {
                    var id = kvp.Value;
                    var node = kvp.Key;
                    asset.SetResource(id, node);
                }

                // 建立Pass的映射
                foreach (var node in passNodes)
                {
                    var inputs = new List<int>();
                    foreach (var inputNode in node.inputs)
                    {
                        inputs.Add(inputNode == null ? ResourceNode.NO_RESOURCE : node2Resource[inputNode]);
                    }
                    
                    var rts = new List<int>();
                    foreach (var rtNode in node.renderTargets)
                    {
                        rts.Add(rtNode == null ? ResourceNode.NO_RESOURCE : node2Resource[rtNode]);
                    }

                    // 当Pass存在输出的目标(RT)时才需要被记录
                    if (rts.Count > 0)
                    {
                        var id = fg.RegisterPassNode(node.name, inputs.ToArray(), rts.ToArray());
                        node2Pass.Add(node, id);
                    }
                }

                asset.InitPassSize(fg.passCount);
                foreach (var kvp in node2Pass)
                {
                    var id = kvp.Value;
                    var node = kvp.Key;
                    asset.SetPass(id, node, node2Group[node]);
                }

                // 记录每个Pass输入输出的来源（它的上一个Pass）
                foreach (var node in passNodes)
                {
                    if (node2Pass.ContainsKey(node))
                    {
                        var inputs = new List<int>();
                        foreach (var inputNode in node.sourcesOfInput)
                        {
                            // 检测每个输入源是否来源与Pass阶段，是Pass表明两个Pass之间有先后关系
                            if (inputNode != null && node2Pass.ContainsKey(inputNode))
                            {
                                inputs.Add(node2Pass[inputNode]);
                            }
                            else
                            {
                                inputs.Add(PassNode.NO_PASS);
                            }
                        }
                        
                        var rts = new List<int>();
                        foreach (var inputNode in node.sourcesOfRenderTarget)
                        {
                            // 检测每个输入源是否来源与Pass阶段，是Pass表明两个Pass之间有先后关系
                            if (inputNode != null && node2Pass.ContainsKey(inputNode))
                            {
                                rts.Add(node2Pass[inputNode]);
                            }
                            else
                            {
                                rts.Add(PassNode.NO_PASS);
                            }
                        }

                        fg.RegisterResRefPassNode(node2Pass[node], inputs.ToArray(), rts.ToArray());
                    }
                }

                // 编译结果并保存
                asset.Compile(fg);
                EditorUtility.SetDirty(asset);
                AssetDatabase.SaveAssets();
            });

            AddButton("Center", graphView.ResetPositionAndZoom);

            graphView.OpenPinned<FrameGraphInspectorView>();
            // bool exposedParamsVisible = graphView.GetPinnedElementStatus<RGNodeParameterView>() != Status.Hidden;
            // showParameters = AddToggle("Show Parameters", exposedParamsVisible,
            //     (v) => graphView.ToggleView<RGNodeParameterView>());
        }

        public override void UpdateButtonStatus()
        {
            if (showParameters != null)
                showParameters.value = graphView.GetPinnedElementStatus<FrameGraphInspectorView>() != Status.Hidden;
        }
    }
}