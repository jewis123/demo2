using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using Object = System.Object;

namespace FrameGraph
{
    public class VolumeFunctionManager
    {
        private static VolumeFunctionManager k_Instance;
        public static VolumeFunctionManager Instance
        {
            get
            {
                k_Instance = new VolumeFunctionManager();
                return k_Instance;
            }
        }

        
        /// <summary>
        /// 后处理VolumeComponent类型和执行逻辑的映射
        /// </summary>
        private Dictionary<Type, Object> m_FuncDic = new Dictionary<Type, Object>();
        
        private VolumeFunctionManager()
        {
            var types = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(a => 
                    a.GetTypes().Where(t => 
                            t.GetInterfaces().Any(x => 
                                x.IsGenericType ? x.GetGenericTypeDefinition() == typeof(IVolumePatchedFunction<>) : false
                                )
                            )
                    )
                .ToArray();

            foreach (var t in types)
            {
                var args = t.GetInterface("IVolumePatchedFunction`1").GetGenericArguments().Where(x => x.BaseType == typeof(VolumeComponent));
                foreach (var arg in args)
                {
                    m_FuncDic.Add(arg, Activator.CreateInstance(t));
                }
            }
        }

        public IVolumePatchedFunction<T> GetFunction<T>() where T : VolumeComponent
        {
            var t = typeof(T);
            if (m_FuncDic.ContainsKey(t))
            {
                return m_FuncDic[t] as IVolumePatchedFunction<T>;
            }
            else
            {
                return default(IVolumePatchedFunction<T>);
            }
        }
    }
}