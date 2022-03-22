/*------------------------------------------------------------------------------
|
| COPYRIGHT (C) 2018 - 2026 All Right Reserved
|
| FILE NAME  : \Assets\CLineActionEditor\ActionEngine\Framework\GameManager.cs
| AUTHOR     : https://supercline.com/
| PURPOSE    : 
|
| SPEC       : 
|
| MODIFICATION HISTORY
| 
| Ver	   Date			   By			   Details
| -----    -----------    -------------   ----------------------
| 1.0	   2018-9-11        SuperCLine           Created
|
+-----------------------------------------------------------------------------*/


namespace SuperCLine.ActionEngine
{
    using UnityEngine;

    public class SkillManager : MonoSingleton<SkillManager>
    {
        void Awake()
        {
            base.Awake();
            GameConfig.Instance.Init();
            LogMgr.Instance.Init();
            FrameCore.Instance.Init();
            ObjectPoolMgr.Instance.Init();
            PropertyMgr.Instance.Init();
            AttackHitMgr.Instance.Init();
            MessageMgr.Instance.Init();
            TimerMgr.Instance.Init();
            AudioMgr.Instance.Init();
            CameraMgr.Instance.Init();
            UnitMgr.Instance.Init();
            EffectMgr.Instance.Init();
        }

        void OnDestroy()
        {
            UnitMgr.Instance.Destroy();
            CameraMgr.Instance.Destroy();
            AudioMgr.Instance.Destroy();
            TimerMgr.Instance.Destroy();
            MessageMgr.Instance.Destroy();
            AttackHitMgr.Instance.Destroy();
            PropertyMgr.Instance.Destroy();
            ObjectPoolMgr.Instance.Destroy();
            FrameCore.Instance.Destroy();
            LogMgr.Instance.Destroy();
            GameConfig.Instance.Destroy();
            EffectMgr.Instance.Destroy();
        }

        void FixedUpdate()
        {
            UnitMgr.Instance.FixedUpdate(Time.fixedDeltaTime);
            AttackHitMgr.Instance.FixedUpdate(Time.fixedDeltaTime);
        }

        void Update()
        {
            PreUpdate(Time.deltaTime);
            UnitMgr.Instance.Update(Time.deltaTime);
            AttackHitMgr.Instance.Update(Time.deltaTime);
            MessageMgr.Instance.Update(Time.deltaTime);
            TimerMgr.Instance.Update(Time.deltaTime);
            AudioMgr.Instance.Update(Time.deltaTime);
            PostUpdate(Time.deltaTime);
        }

        void PreUpdate(float fTick)
        {

        }

        void PostUpdate(float fTick)
        {
        }

        void LateUpdate()
        {
            CameraMgr.Instance.LateUpdate(Time.deltaTime);
        }

        void OnGUI()
        {
            LogMgr.Instance.OnGUI();
        }

        void OnApplicationQuit()
        {
        }

        void OnApplicationPause(bool pause)
        {

        }

        void OnApplicationFocus(bool focus)
        {

        }
    }
}

