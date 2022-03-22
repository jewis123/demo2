using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FrameGraph
{
    [Serializable]
    public class FrameGraph
    {
        [SerializeReference] private List<ResourceNode> m_ResNodes = new List<ResourceNode>();

        [SerializeReference] private List<PassNode> m_PassNodes = new List<PassNode>();

        public int resourceCount
        {
            get { return m_ResNodes.Count; }
        }

        public int passCount
        {
            get { return m_PassNodes.Count; }
        }

        public int RegisterResourceNode(string name)
        {
            var id = m_ResNodes.Count;
            var node = new ResourceNode() {id = id, name = name};
            m_ResNodes.Add(node);
            return id;
        }

        public int RegisterPassNode(string name, int[] inputReses, int[] outputReses)
        {
            int[] inputReferences = new int[inputReses.Length];
            int[] outputReferences = new int[outputReses.Length];
            for (int i = 0, cnt = inputReferences.Length; i < cnt; i++)
            {
                inputReferences[i] = PassNode.NO_PASS;
            }

            for (int i = 0, cnt = outputReferences.Length; i < cnt; i++)
            {
                outputReferences[i] = PassNode.NO_PASS;
            }

            var id = m_PassNodes.Count;
            var node = new PassNode()
            {
                id = id, name = name, inputReses = inputReses, outputReses = outputReses,
                inputResRefPasses = inputReferences, outputResRefPasses = outputReferences
            };
            m_PassNodes.Add(node);
            return id;
        }

        public bool RegisterResRefPassNode(int pass, int[] inputReferences, int[] outputReferences)
        {
            if (pass < m_PassNodes.Count)
            {
                var passNode = m_PassNodes[pass];
                if (passNode.inputResRefPasses.Length == inputReferences.Length &&
                    passNode.outputReses.Length == outputReferences.Length)
                {
                    m_PassNodes[pass].inputResRefPasses = inputReferences;
                    m_PassNodes[pass].outputResRefPasses = outputReferences;
                    return true;
                }
            }

            return false;
        }

        public List<PassNode>.Enumerator GetPassNodes()
        {
            return m_PassNodes.GetEnumerator();
        }

        public PassNode GetPassNode(int passId)
        {
            return passId < m_PassNodes.Count ? m_PassNodes[passId] : null;
        }

        public ResourceNode GetResourceNode(int resId)
        {
            return resId < m_ResNodes.Count ? m_ResNodes[resId] : null;
        }
    }
}