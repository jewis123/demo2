using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using GraphProcessor;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;
using Button = UnityEngine.UIElements.Button;
using Group = UnityEditor.Experimental.GraphView.Group;
using Image = UnityEngine.UI.Image;
using Toggle = UnityEngine.UIElements.Toggle;

namespace FrameGraph
{
    public class FrameGraphInspectorView : PinnedElementView
    {
        private const string PanelTitle = "Inspector";

        // private BaseNodeView m_ShownNodeView;
        private Button m_AddButton;

        private BaseGraphView m_GraphView;
        private int m_NodeViewIndex;
        private int m_SerializableObjectIndex;

        protected override void Initialize(BaseGraphView graphView)
        {
            m_GraphView = graphView;
            title = PanelTitle;
            m_AddButton = new Button(OnAddClicked)
            {
                text = "+"
            };

            graphView.RegisterCallback<MouseUpEvent>(evt =>
            {
                // 每次鼠标放开时（包含点选和框选），扫描被选中的节点
                // 由于没有方法可以监听到哪些节点被选中，所以只能使用这种方式来实现
                bool findSelected = false;
                m_NodeViewIndex = 0;
                m_SerializableObjectIndex = 0;
                foreach (var nodeView in graphView.nodeViews)
                {
                    if (nodeView.selected)
                    {
                        findSelected = true;
                        break;
                    }

                    // 由于删除节点后nodeViews没有更新，所以为了和序列化数据保持一致，这里仅当节点存在父节点时（表示没有被删掉）计数+1
                    if (nodeView.parent != null)
                    {
                        m_SerializableObjectIndex++;
                    }

                    m_NodeViewIndex++;
                }

                if (!findSelected)
                {
                    m_NodeViewIndex = -1;
                    m_SerializableObjectIndex = -1;
                }
                ShowNodeParameters(m_NodeViewIndex, m_SerializableObjectIndex);
            });
        }

        private void ShowNodeParameters(int nodeViewIndex, int serializableObjectIndex)
        {
            if (serializableObjectIndex < 0)
            {
                // 没有任何节点被选中
                base.title = PanelTitle;
                HideAddButton();
                HideContent();
            }
            else
            {
                var node = m_GraphView.nodeViews[nodeViewIndex].nodeTarget;
                title = node.name;

                // 检测是否需要添加"+"按钮
                if (HasParametersAttribute(node))
                {
                    ShowAddButton();
                }
                else
                {
                    HideAddButton();
                }

                ShowContent(nodeViewIndex, serializableObjectIndex);
            }
        }

        private bool HasParametersAttribute(BaseNode node)
        {
            var fields = node.GetType().GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

            foreach (var field in fields)
            {
                var attribute = field.GetCustomAttribute(typeof(ParameterAttribute)) as ParameterAttribute;
                if (attribute != null && attribute.wait2Add)
                {
                    return true;
                }
            }

            return false;
        }

        private void ShowAddButton()
        {
            if (!header.Contains(m_AddButton))
                header.Add(m_AddButton);
        }

        private void HideAddButton()
        {
            if (header.Contains(m_AddButton))
                header.Remove(m_AddButton);
        }

        private void ShowContent(int nodeViewIndex, int serializableObjectIndex)
        {
            content.Clear();
            var node = m_GraphView.nodeViews[nodeViewIndex].nodeTarget as FrameGraphBaseNode;
            if (node != null)
            {
                var nodes = m_GraphView.serializedGraph.FindProperty("nodes");
                var nodeProperty = nodes.GetArrayElementAtIndex(serializableObjectIndex);
                var fieldInfos = node.GetType().GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
                foreach (var fieldInfo in fieldInfos)
                {
                    var attribute = fieldInfo.GetCustomAttribute(typeof(ParameterAttribute)) as ParameterAttribute;
                    if (attribute != null)
                    {
                        bool showField = true;
                        // 如果是等待添加类型，当添加时才会显示字段。否则就直接显示字段
                        if (attribute.wait2Add)
                        {
                            showField = node.customFields.Contains(fieldInfo.Name);
                        }
                    
                        if (showField)
                        {
                            var child = nodeProperty.FindPropertyRelative(fieldInfo.Name);
                            var field = new PropertyField(child);
                            field.Bind(m_GraphView.serializedGraph);
                            field.RegisterValueChangeCallback(e =>
                            {
                                (m_GraphView.nodeViewsPerNode[node] as FrameGraphBaseNodeView).OnFieldChange(fieldInfo.Name);
                            });
                            content.Add(field);
                        }
                    }
                }
            }
        }

        private void HideContent()
        {
            content.Clear();
        }

        private void OnAddClicked()
        {
            if (m_NodeViewIndex >= 0)
            {
                var node = m_GraphView.nodeViews[m_NodeViewIndex].nodeTarget as FrameGraphBaseNode;
                if (node != null)
                {
                    var parameterType = new GenericMenu();
            
                    var fields = node.GetType().GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            
                    foreach (var field in fields)
                    {
                        var attribute = field.GetCustomAttribute(typeof(ParameterAttribute)) as ParameterAttribute;
                        if (attribute != null && attribute.wait2Add)
                        {
                            bool isOn = node.customFields.Contains(field.Name);
                            parameterType.AddItem(new GUIContent(field.Name), isOn, () =>
                            {
                                if (isOn)
                                {
                                    node.customFields.Remove(field.Name);
                                }
                                else
                                {
                                    node.customFields.Add(field.Name);
                                }
            
                                // 刷新面板
                                ShowNodeParameters(m_NodeViewIndex, m_SerializableObjectIndex);
                            });
                        }
                    }
            
                    parameterType.ShowAsContext();
                }
            }
        }
    }
}