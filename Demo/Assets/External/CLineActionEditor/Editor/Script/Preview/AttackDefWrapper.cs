/*------------------------------------------------------------------------------
|
| COPYRIGHT (C) 2021 - 2029 All Right Reserved
|
| FILE NAME  : \CLineActionEditor\Editor\Script\Preview\AttackDefWrapper.cs
| AUTHOR     : https://supercline.com/
| PURPOSE    :
|
| SPEC       :
|
| MODIFICATION HISTORY
|
| Ver      Date            By              Details
| -----    -----------    -------------   ----------------------
| 1.0      2021-11-8      SuperCLine           Created
|
+-----------------------------------------------------------------------------*/

namespace SuperCLine.ActionEngine
{
    using UnityEngine;

    public class AttackDefWrapper : XObject
    {
        private GameObject mRec = null;
        private GameObject mArea = null;
        private ActionAttackDef mAttack = null;
        private float mSimulateTime = 0f;
        private PhysicalEnityWrapper _PhXwrapper;
        protected float mEmitTimer = 0f;
        protected bool mFirstTime = true;
        private int mCurNum;
        private Mesh attackMesh;

        public AttackDefWrapper(ActionAttackDef aad)
        {
            mAttack = aad;
        }


        void NormalEmit()
        {
            if (mAttack.EmitProperty is NormalEmitProperty)
            {
                Vector3 StartPosition = CalcStartPositionForNormal();
                Vector3 StartDirection = CalcStartRotationForNormal();

                mRec = new GameObject("temp");

                UDrawTool tool;
                switch (mAttack.EntityType)
                {
                    case EEntityType.EET_FrameCub:
                        break;
                    case EEntityType.EET_FrameCylinder:
                        {
                            FrameEntityCylinderProperty p = mAttack.EntityProperty as FrameEntityCylinderProperty;
                            mRec.transform.localScale = Vector3.one;
                            mRec.transform.localPosition = StartPosition;
                            mRec.transform.localRotation = Quaternion.LookRotation(Vector3.forward);
                            tool = mRec.AddComponent<UDrawTool>();
                            tool.DrawCircleSolid(mRec.transform, StartPosition, p.Radius, Color.red, ref attackMesh);
                            mArea = tool.go;
                            mCurNum++;
                        }
                        break;
                    case EEntityType.EET_FrameFan:
                        {
                            FrameEntityFanProperty p = mAttack.EntityProperty as FrameEntityFanProperty;
                            mRec.transform.localScale = Vector3.one;
                            mRec.transform.localPosition = StartPosition;
                            mRec.transform.localRotation = Quaternion.LookRotation(StartDirection);
                            tool = mRec.AddComponent<UDrawTool>();
                            tool.DrawSectorSolid(mRec.transform, StartPosition, p.Degree, p.Radius, Color.red, ref attackMesh);
                            mArea = tool.go;
                            mCurNum++;
                        }
                        break;
                    case EEntityType.EET_Physical:
                        {
                            PhysicalEntityProperty p = mAttack.EntityProperty as PhysicalEntityProperty;
                            _PhXwrapper = new PhysicalEnityWrapper();
                            var ah = new AttackHit("1111");
                            ah.Data = mAttack;
                            _PhXwrapper.AttackEntity = new PhysicalEntity(ah, p, StartPosition, StartDirection);
                            _PhXwrapper.SetAnimator(mAttack.MotionAnimatorType, mAttack.MotionAnimatorProperty,ah);
                            mCurNum++;
                            break;

                        }
                    default:
                        LogMgr.Instance.Logf(ELogType.ELT_WARNING, "Core", "current attack def can not preview --{0}", mAttack.DebugName);
                        break;
                }

            }
        }

        Vector3 CalcStartPositionForNormal()
        {
            Vector3 pos = Helper.Vec3Zero;
            Vector3 fward = Helper.Vec3Zero;
            NormalEmitProperty mProperty = mAttack.EmitProperty as NormalEmitProperty;
            Transform unitTransform = UnitWrapper.Instance.UnitWrapperUnit.transform;
            Transform bindBone = null;
            if (!string.IsNullOrEmpty(mProperty.Dummy))
            {
                bindBone = GameObject.Find(mProperty.Dummy).transform;
            }
            switch (mProperty.PosType)
            {
                case EEmitterPosType.EEPT_AttackerCurrentPosAndDir:
                    {
                        pos = bindBone != null ? bindBone.position : unitTransform.position;
                        fward = bindBone != null ? bindBone.forward : unitTransform.forward;
                    }
                    break;
            }


            Vector2 offsetXZ = new Vector2(mProperty.EmitOffset.x, mProperty.EmitOffset.z);
            Vector3 directionXZ = new Vector2(fward.x, fward.z);
            offsetXZ = offsetXZ.magnitude * directionXZ;
            pos += new Vector3(offsetXZ.x, mProperty.EmitOffset.y, offsetXZ.y);

            return pos;
        }

        Vector3 CalcStartRotationForNormal()
        {
            Vector3 direction;
            NormalEmitProperty mProperty = mAttack.EmitProperty as NormalEmitProperty;
            Transform unitTransform = UnitWrapper.Instance.UnitWrapperUnit.transform;
            switch (mProperty.PosType)
            {
                case EEmitterPosType.EEPT_AttackerCurrentPosAndDir:
                default:
                    direction = unitTransform.forward;
                    break;
            }

            if (mProperty.EmitRotation != Helper.Vec3Zero)
            {
                Quaternion qat = Quaternion.LookRotation(direction);
                Vector3 euler = mProperty.EmitRotation + qat.eulerAngles;
                direction = Quaternion.Euler(euler) * Vector3.forward;
            }

            return direction;
        }

        public void Tick(float fTick)
        {
            mSimulateTime += fTick;
            SimulateEmit(fTick);
            _PhXwrapper?.Update(fTick);
        }

        private void SimulateEmit(float fTick)
        {
            if (mCurNum >= mAttack.EmitProperty.Num)
            {
                return;
            }
            switch (mAttack.EmitType)
            {
                case EEmitType.EET_Normal:
                {
                    break;
                }
                default:
                    LogMgr.Instance.Logf(ELogType.ELT_WARNING, "Core", "current attack def can not preview --{0}", mAttack.DebugName);
                    return;
            }
            
            if (mFirstTime || mEmitTimer >= mAttack.EmitProperty.Interval)
            {
                NormalEmit();

                if (mFirstTime)
                    mFirstTime = false;
                else
                    mEmitTimer -= mAttack.EmitProperty.Interval;
            }
            else
            {
                mEmitTimer += fTick;
            }
        }


        protected override void OnDispose()
        {
            if (mRec)
                GameObject.DestroyImmediate(mRec);
            if (mArea)
                GameObject.DestroyImmediate(mArea);
            
            if (_PhXwrapper != null)
            {
                _PhXwrapper.Dispose();
            }
        }
    }
}