using System;
using UnityEngine.Rendering;

namespace FrameGraph
{
    [Serializable]
    public class BlitDescriptionArrayParameter : VolumeParameter<BlitDescription[]>
    {
        public BlitDescriptionArrayParameter(BlitDescription[] value, bool overrideState = false)
            : base(value, overrideState)
        {
        }
    }
        
    [Serializable]
    public class StringParameter : VolumeParameter<string>
    {
        public StringParameter(string value, bool overrideState = false)
            : base(value, overrideState)
        {
        }
    }
}