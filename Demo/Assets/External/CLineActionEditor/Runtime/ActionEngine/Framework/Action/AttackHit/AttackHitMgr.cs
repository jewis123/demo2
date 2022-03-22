/*------------------------------------------------------------------------------
|
| COPYRIGHT (C) 2018 - 2026 All Right Reserved
|
| FILE NAME  : \Assets\CLineActionEditor\ActionEngine\Framework\Action\AttackHit\AttackHitMgr.cs
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
    using System.Collections.Generic;

    public sealed class AttackHitMgr : Singleton<AttackHitMgr>
    {
        private List<IAttackHit> mAttackHitList = new List<IAttackHit>();
        private List<IAttackHit> mHandleList = new List<IAttackHit>();

        public override void Init()
        {

        }

        public override void Destroy()
        {
            using (var itr = mAttackHitList.GetEnumerator())
            {
                while (itr.MoveNext())
                {
                    itr.Current.Dispose();
                }
            }

            mAttackHitList.Clear();
            mHandleList.Clear();
        }

        public AttackHit Create(Unit owner, ActionAttackDef data, params object[] param)
        {
            AttackHit ah = null;
            switch (data.AttackHitType)
            {
                case EAttackHitType.EAHT_Normal:
                    ah = new AttackHit(param) {Owner = owner, Data = data};
                    break;
                default:
                    break;
            }

            if (ah != null)
            {
                ah.Init();
                mAttackHitList.Add(ah);
            }

            return ah;
        }

        public void Update(float fTick)
        {
            for (int i = 0; i < mAttackHitList.Count; i++)
            {
                if (mAttackHitList[i].IsDead)
                    mHandleList.Add(mAttackHitList[i]);
                else
                    mAttackHitList[i].Update(fTick);
            }


            for (int i = 0; i < mHandleList.Count; i++)
            {
                mHandleList[i].Dispose();
                mAttackHitList.Remove(mHandleList[i]);
            }

            mHandleList.Clear();
        }

        public void FixedUpdate(float fTick)
        {
            for (int i = 0; i < mAttackHitList.Count; i++)
            {
                mAttackHitList[i].FixedUpdate(fTick);
            }
        }
    }
}
