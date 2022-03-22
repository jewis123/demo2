﻿/*------------------------------------------------------------------------------
|
| COPYRIGHT (C) 2018 - 2026 All Right Reserved
|
| FILE NAME  : \Assets\CLineActionEditor\ActionEngine\Framework\Property\ActionAttackDef.cs
| AUTHOR     : https://supercline.com/
| PURPOSE    : 
|
| SPEC       : 
|
| MODIFICATION HISTORY
| 
| Ver	   Date			   By			   Details
| -----    -----------    -------------   ----------------------
| 1.0	   2019-4-12      SuperCLine           Created
|
+-----------------------------------------------------------------------------*/

namespace SuperCLine.ActionEngine
{
    using UnityEngine;
    using LitJson;
    using System;
    using System.Collections.Generic;

    public sealed class ActionAttackDef : IEventData, IProperty
    {
        [SerializeField] private float mDelay = 0f;
        [SerializeField] private bool mDeadActionChanged = false;

        // attack hit
        [SerializeField] private EAttackHitType mAttackHitType = EAttackHitType.EAHT_Normal;

        // emit
        [SerializeField] private EEmitType mEmitType = EEmitType.EET_None;
        [SerializeReference] private EmitProperty mEmitProperty = null;

        // attack entity
        [SerializeField] private EEntityType mEntityType = EEntityType.EET_None;
        [SerializeReference] private AttackEntityProperty mEntityProperty = null;

        // motion animator
        [SerializeField] private EMotionAnimatorType mMotionAnimatorType = EMotionAnimatorType.EMAT_None;
        [SerializeReference] private MotionAnimatorProperty mMotionAnimatorProperty = null;

        // feedback
        [SerializeReference] private List<IProperty> mHitFeedbackList = new List<IProperty>();

        #region property
        [EditorProperty("延迟时间发射", EditorPropertyType.EEPT_Float, Description = "攻击生成延迟时间生效")]
        public float Delay
        {
            get { return mDelay; }
            set { mDelay = value; }
        }
        [EditorProperty("Action切换是否立即死亡", EditorPropertyType.EEPT_Bool)]
        public bool DeadActionChanged
        {
            get { return mDeadActionChanged; }
            set { mDeadActionChanged = value; }
        }
        [EditorProperty("攻击定义组件", EditorPropertyType.EEPT_Enum)]
        public EAttackHitType AttackHitType
        {
            get { return mAttackHitType; }
            set { mAttackHitType = value; }
        }
        [EditorProperty("发射器类型", EditorPropertyType.EEPT_Enum)]
        public EEmitType EmitType
        {
            get { return mEmitType; }
            set
            {
                if (mEmitType != value)
                {
                    mEmitType = value;

                    switch (mEmitType)
                    {
                        case EEmitType.EET_Normal:
                            mEmitProperty = new NormalEmitProperty();
                            break;
                        case EEmitType.EET_ARC:
                            mEmitProperty = new ArcEmitProperty();
                            break;
                        default:
                            mEmitProperty = null;
                            break;
                    }
                }
            }
        }
        [EditorProperty("攻击体类型", EditorPropertyType.EEPT_Enum)]
        public EEntityType EntityType
        {
            get { return mEntityType; }
            set
            {
                if (mEntityType != value)
                {
                    mEntityType = value;
                    switch (mEntityType)
                    {
                        case EEntityType.EET_FrameCub:
                            mEntityProperty = new FrameEntityCubProperty();
                            break;
                        case EEntityType.EET_FrameCylinder:
                            mEntityProperty = new FrameEntityCylinderProperty();
                            break;
                        case EEntityType.EET_FrameFan:
                            mEntityProperty = new FrameEntityFanProperty();
                            break;
                        case EEntityType.EET_FrameRing:
                            mEntityProperty = new FrameEntityRingProperty();
                            break;
                        case EEntityType.EET_FrameSphere:
                            mEntityProperty = new FrameEntitySphereProperty();
                            break;
                        case EEntityType.EET_Physical:
                            mEntityProperty = new PhysicalEntityProperty();
                            break;
                        case EEntityType.EET_Bounce:
                            mEntityProperty = new BounceEntityProperty();
                            break;
                        case EEntityType.EET_Hook:
                            mEntityProperty = new HookEntityProperty();
                            break;
                        default:
                            mEntityProperty = null;
                            break;
                    }
                }

            }
        }
        [EditorProperty("运动组件类型", EditorPropertyType.EEPT_Enum)]
        public EMotionAnimatorType MotionAnimatorType
        {
            get { return mMotionAnimatorType; }
            set
            {
                if (mMotionAnimatorType != value)
                {
                    mMotionAnimatorType = value;
                    switch (mMotionAnimatorType)
                    {
                        case EMotionAnimatorType.EMAT_Line:
                            mMotionAnimatorProperty = new LineAnimatorProperty();
                            break;
                        case EMotionAnimatorType.EMAT_Curve:
                            mMotionAnimatorProperty = new CurveAnimatorProperty();
                            break;
                        case EMotionAnimatorType.EMAT_PingPong:
                            mMotionAnimatorProperty = new PingPongAnimatorProperty();
                            break;
                        default:
                            mMotionAnimatorProperty = null;
                            break;
                    }
                }

            }
        }
        #endregion

        public List<IProperty> HitFeedbackList
        {
            get { return mHitFeedbackList; }
            set { mHitFeedbackList = value; }
        }

        public string DebugName
        {
            get { return GetType().ToString(); }
        }

        public EmitProperty EmitProperty
        {
            get { return mEmitProperty; }
        }
        public AttackEntityProperty EntityProperty
        {
            get { return mEntityProperty; }
        }
        public MotionAnimatorProperty MotionAnimatorProperty
        {
            get { return mMotionAnimatorProperty; }
        }

        public void Deserialize(JsonData jd)
        {
            mDelay = JsonHelper.ReadFloat(jd["Delay"]);
            mDeadActionChanged = JsonHelper.ReadBool(jd["DeadActionChanged"]);
          
            //attack hit
            mAttackHitType = JsonHelper.ReadEnum<EAttackHitType>(jd["AttackHitType"]);

            //emit
            EmitType = JsonHelper.ReadEnum<EEmitType>(jd["EmitType"]);
            if (EmitType != EEmitType.EET_None)
                mEmitProperty.Deserialize(jd["EmitProperty"]);

            //attack entity
            EntityType = JsonHelper.ReadEnum<EEntityType>(jd["EntityType"]);
            if (EntityType != EEntityType.EET_None)
                mEntityProperty.Deserialize(jd["EntityProperty"]);

            //motion animator
            MotionAnimatorType = JsonHelper.ReadEnum<EMotionAnimatorType>(jd["MotionAnimatorType"]);
            if (MotionAnimatorType != EMotionAnimatorType.EMAT_None)
                mMotionAnimatorProperty.Deserialize(jd["MotionAnimatorProperty"]);

            //hit feedback
            JsonData jdFeedback = jd["FeedbackList"];
            for (int i = 0; i < jdFeedback.Count; ++i)
            {
                EHitFeedbackType eht = JsonHelper.ReadEnum<EHitFeedbackType>(jdFeedback[i]["FeedbackType"]);
                IProperty fb = Add(eht);
                fb.Deserialize(jdFeedback[i]);
            }
        }

        public JsonWriter Serialize(JsonWriter writer)
        {
            JsonHelper.WriteProperty(ref writer, "Delay", mDelay);
            JsonHelper.WriteProperty(ref writer, "DeadActionChanged", mDeadActionChanged);

            //attack hit
            JsonHelper.WriteProperty(ref writer, "AttackHitType", mAttackHitType.ToString());

            //emit
            JsonHelper.WriteProperty(ref writer, "EmitType", mEmitType.ToString());
            writer.WritePropertyName("EmitProperty");
            writer.WriteObjectStart();
            if (mEmitProperty != null)
                writer = mEmitProperty.Serialize(writer);
            writer.WriteObjectEnd();

            //attack entity
            JsonHelper.WriteProperty(ref writer, "EntityType", mEntityType.ToString());
            writer.WritePropertyName("EntityProperty");
            writer.WriteObjectStart();
            if (mEntityProperty != null)
                writer = mEntityProperty.Serialize(writer);
            writer.WriteObjectEnd();

            //motion animator
            JsonHelper.WriteProperty(ref writer, "MotionAnimatorType", mMotionAnimatorType.ToString());
            writer.WritePropertyName("MotionAnimatorProperty");
            writer.WriteObjectStart();
            if (mMotionAnimatorProperty != null)
                writer = mMotionAnimatorProperty.Serialize(writer);
            writer.WriteObjectEnd();

            //hit feedback
            writer.WritePropertyName("FeedbackList");
            writer.WriteArrayStart();
            using (List<IProperty>.Enumerator itr = mHitFeedbackList.GetEnumerator())
            {
                while (itr.MoveNext())
                {
                    writer.WriteObjectStart();
                    IHitFeedback fb = itr.Current as IHitFeedback;
                    JsonHelper.WriteProperty(ref writer, "FeedbackType", fb.FeedbackType.ToString());
                    writer = itr.Current.Serialize(writer);
                    writer.WriteObjectEnd();
                }
            }
            writer.WriteArrayEnd();

            return writer;
        }

        public IProperty Add(EHitFeedbackType t)
        {
            IProperty fb = null;
            switch (t)
            {
                case EHitFeedbackType.EHT_HitDamage:
                    fb = new HitDamage();
                    break;
                case EHitFeedbackType.EHT_HitSound:
                    fb = new HitSound();
                    break;
                case EHitFeedbackType.EHT_HitSoundRandom:
                    fb = new HitSoundRandom();
                    break;
                case EHitFeedbackType.EHT_HitEffect:
                    fb = new HitEffect();
                    break;
                case EHitFeedbackType.EHT_HitEffectRandom:
                    fb = new HitEffectRandom();
                    break;
                case EHitFeedbackType.EHT_HitModifyMaterial:
                    fb = new HitModifyMaterial();
                    break;
                case EHitFeedbackType.EHT_HitAddBuff:
                    fb = new HitAddBuff();
                    break;
                case EHitFeedbackType.EHT_HitAction:
                    fb = new HitAction();
                    break;
                case EHitFeedbackType.EHT_HitAttackerSpeed:
                    fb = new HitAttackerSpeed();
                    break;
            }

            mHitFeedbackList.Add(fb);

            return fb;
        }

        public void Del(IProperty fb)
        {
            mHitFeedbackList.Remove(fb);
        }

        public void DelAll()
        {
            mHitFeedbackList.Clear();
        }

        public EEventDataType EventType
        {
            get { return EEventDataType.EET_AttackDef; }
        }

        public void Enter(Unit unit)
        {

        }
        public void Update(Unit unit, int deltaTime)
        {

        }
        public void Exit(Unit unit)
        {

        }

        public void Execute(Unit unit)
        {
        }

        public IEventData Clone()
        {
            ActionAttackDef evt = new ActionAttackDef();
            evt.mDelay = this.mDelay;
            evt.mDeadActionChanged = this.mDeadActionChanged;
            evt.mAttackHitType = this.mAttackHitType;

            return evt;
        }
    }

}
