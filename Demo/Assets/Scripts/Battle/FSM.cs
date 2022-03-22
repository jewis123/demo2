using System;
using System.Collections.Generic;
using UnityEngine;

namespace Battle
{


    public class FSM<K> 
    {
        public K target { get; private set; }
        
        public FSM(K t)
        {
            target = t;
            PrepareVariantMap();
        }


        //利用字典存储各种状态
        Dictionary<string, FSMState<K>> Dic = new Dictionary<string, FSMState<K>>();

        //当前状态
        FSMState<K> currentstate;

        public string currentstateName { get; private set; }

        //注册状态
        public void Register<T>() where T : FSMState<K>,new()
        {
            var statename = typeof(T).Name;
            var state = new T();
            state.Init(this);
            if (!Dic.ContainsKey(statename))
            {
                Dic.Add(statename,state);
            }
        }

        //设置默认状态
        public void SetDefault<T>() where T : FSMState<K>
        {
            var statename = typeof(T).Name;
            if (Dic.ContainsKey(statename))
            {
                currentstate = Dic[statename];
                currentstate.EnterState();
                currentstateName = statename;
            }
        }
        
        //改变状态
        public void ChangeState<T>()where T : FSMState<K>
        {
            if (currentstateName == typeof(T).Name)
            {
                return;   
            }
            var statename = typeof(T).Name;
            ChangeState(statename);
        }

        public void ChangeState(string statename)
        {
            if (Dic.ContainsKey(statename))
            {
                if (currentstate!=null)
                {
                    currentstate.ExitState();
                    currentstate = Dic[statename];
                    currentstate.EnterState();
                    currentstateName = statename;
                }
            }
        }

        //更新状态
        public void UpdateState()
        {
            if (currentstate!=null)
            {
                currentstate.UpdateState();
            }
        }
        
        
        
        
        
        public enum VariantType
        {
            Bool,
            Int,
            Float,
            String,
            Object,
        }

        [Serializable]
        public class Variant
        {
            [SerializeField] public string name;
            [SerializeField] public VariantType variantType;
            [SerializeField] public int valueInt;
            [SerializeField] public float valueFloat;
            [SerializeField] public string valueString;
            [SerializeField] public bool valueBool;
            [SerializeField] public object valueObject;

            public object GetValue()
            {
                switch (variantType)
                {
                    case VariantType.Bool:
                        return valueBool;
                    case VariantType.Int:
                        return valueInt;
                    case VariantType.Float:
                        return valueFloat;
                    case VariantType.String:
                        return valueString;
                    case VariantType.Object:
                        return valueObject;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }

            public bool GetBool()
            {
                return variantType == VariantType.Bool ? valueBool : default;
            }

            public int GetInt()
            {
                return variantType == VariantType.Int ? valueInt : default;
            }

            public float GetFloat()
            {
                return variantType == VariantType.Float ? valueFloat : default;
            }

            public string GetString()
            {
                return variantType == VariantType.String ? valueString : default;
            }
            
            public T GetObject<T>()
            {
                return variantType == VariantType.Object ? (T) valueObject : default;
            }
        }

        private Variant[] variantMap = Array.Empty<Variant>();

        public Dictionary<string, Variant> variantMapDic { get; private set; } = null;
        

        public void PrepareVariantMap()
        {
            if (!Application.isPlaying)
            {
                if (variantMapDic == null)
                    variantMapDic = new Dictionary<string, Variant>();
                else
                    variantMapDic.Clear();
                for (int i = 0; i < variantMap.Length; i++)
                {
                    variantMapDic.Add(variantMap[i].name, variantMap[i]);
                }
            }

            if (variantMapDic == null)
            {
                variantMapDic = new Dictionary<string, Variant>();
                for (int i = 0; i < variantMap.Length; i++)
                {
                    variantMapDic.Add(variantMap[i].name, variantMap[i]);
                }
            }
        }

        public bool GetBool(string key)
        {
            PrepareVariantMap();
            if (variantMapDic.ContainsKey(key))
                return variantMapDic[key].GetBool();
            return default;
        }

        public int GetInt(string key)
        {
            PrepareVariantMap();
            if (variantMapDic.ContainsKey(key))
                return variantMapDic[key].GetInt();
            return default;
        }

        public float GetFloat(string key)
        {
            PrepareVariantMap();
            if (variantMapDic.ContainsKey(key))
                return variantMapDic[key].GetFloat();
            return default;
        }

        public string GetString(string key)
        {
            PrepareVariantMap();
            if (variantMapDic.ContainsKey(key))
                return variantMapDic[key].GetString();
            return default;
        }

        public object GetValue(string key)
        {
            PrepareVariantMap();
            if (variantMapDic.ContainsKey(key))
            {
                return variantMapDic[key].GetValue();
            }

            return null;
        }
        
        
        public T GetObject<T>(string key)
        {
            PrepareVariantMap();
            if (variantMapDic.ContainsKey(key))
            {
                return variantMapDic[key].GetObject<T>();
            }
            return default;
        }

        public Variant GetVariant(string key)
        {
            PrepareVariantMap();
            if (variantMapDic.ContainsKey(key))
            {
                return variantMapDic[key];
            }

            return null;
        }


        public bool HasKey(string key)
        {
            PrepareVariantMap();
            return variantMapDic.ContainsKey(key);
        }
        

        public void SetBool(string key, bool value)
        {
            PrepareVariantMap();
            if (variantMapDic.ContainsKey(key) && variantMapDic[key].variantType == VariantType.Bool)
                variantMapDic[key].valueBool = value;
            else if (!variantMapDic.ContainsKey(key))
                variantMapDic.Add(key, new Variant()
                {
                    name = key,
                    variantType = VariantType.Bool,
                    valueBool = value
                });
        }

        public void SetInt(string key, int value)
        {
            PrepareVariantMap();
            if (variantMapDic.ContainsKey(key) && variantMapDic[key].variantType == VariantType.Int)
                variantMapDic[key].valueInt = value;
            else if (!variantMapDic.ContainsKey(key))
                variantMapDic.Add(key, new Variant()
                {
                    name = key,
                    variantType = VariantType.Int,
                    valueInt = value
                });
        }

        public void SetFloat(string key, float value)
        {
            PrepareVariantMap();
            if (variantMapDic.ContainsKey(key) && variantMapDic[key].variantType == VariantType.Float)
                variantMapDic[key].valueFloat = value;
            else if (!variantMapDic.ContainsKey(key))
                variantMapDic.Add(key, new Variant()
                {
                    name = key,
                    variantType = VariantType.Float,
                    valueFloat = value
                });
        }

        public void SetString(string key, string value)
        {
            PrepareVariantMap();
            if (variantMapDic.ContainsKey(key) && variantMapDic[key].variantType == VariantType.String)
                variantMapDic[key].valueString = value;
            else if (!variantMapDic.ContainsKey(key))
                variantMapDic.Add(key, new Variant()
                {
                    name = key,
                    variantType = VariantType.String,
                    valueString = value
                });
        }
        
        public void SetObject<T>(string key, T value)
        {
            PrepareVariantMap();
            if (variantMapDic.ContainsKey(key) && variantMapDic[key].variantType == VariantType.Object)
                variantMapDic[key].valueObject = value;
            else if (!variantMapDic.ContainsKey(key))
                variantMapDic.Add(key, new Variant()
                {
                    name = key,
                    variantType = VariantType.Object,
                    valueObject = value
                });
        }

    }
}