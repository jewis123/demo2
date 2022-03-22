using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;


namespace FrameGraph
{
    [CreateAssetMenu(menuName = "FrameGraph/RendererData")]
    public class FrameGraphRendererData : ForwardRendererData
    {
        [SerializeField]
        private FrameGraphAsset m_GraphAsset;

        public IFrameGraphAssetHandle GraphAsset
        {
            get
            {
                return m_GraphAsset;
            }
        }
    
        protected override ScriptableRenderer Create()
        {
            return new FrameGraphRenderer(this);
        }
    }

}

