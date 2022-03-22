using System;
using System.Collections.Generic;
using GraphProcessor;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace FrameGraph
{
    [Serializable, VolumeComponentMenu("Post-processing/BlitGroup")]
    public class BlitGroup : VolumeComponent, IPostProcessComponent
    {
        public BlitDescriptionArrayParameter blitDescriptions = new BlitDescriptionArrayParameter(new BlitDescription[0], true);

        public bool IsActive()
        {
            return true;
        }

        public bool IsTileCompatible() => false;
    }

    [Serializable]
    public class BlitDescription
    {
        public string tag;
        public List<MaterialDescription> materialDescriptions;
    }

    [Serializable]
    public class MaterialDescription
    {
        public Material material;
        public int passIndex;
        public string sourceTexName;
    }
}