using SuperCLine.ActionEngine;
using UnityEngine;

namespace Battle.States
{
    public class BehitState: FSMState<BattleCharacter>
    {
        private static readonly int SpeedHash = Animator.StringToHash("speed");
        private int stateHash;
        private float offset;
        private BattleUnit unit;
        
        public override void EnterState()
        {
            if (fsm.target.IsDead)
            {
                return;
            }
            
            
            offset = 0;
            Random.InitState((int) (fsm.target.data.id * Time.time));
            float ratio = UnityEngine.Random.value;
            if ( ratio < fsm.target.data.beHitRatio)
            {
                fsm.target.bHitBack = true;
            }

            // fsm.target.animator.SetFloat(SpeedHash,fsm.target.battle.battleSpeed);
            
            //调用技能系统播放
            fsm.target.Actor.ActionStatus.OnActionFinishCB += ActionFinished;
            fsm.target.Actor.ActionStatus.ChangeAction("normalHit",0, false,0.5f);
            unit = fsm.target.Actor as BattleUnit;
            unit.HasControl= true;
            
            
            //打断后续连招
            fsm.target.battleActionQueue.CancelCmd(fsm.target.data.id, fsm.target.data.team == 0);
            fsm.target.battleActionQueue.isMyTeamHasHitted = true;
        }

        public override void ExitState()
        {
            fsm.target.Actor.ActionStatus.OnActionFinishCB -= ActionFinished;
            unit.HasControl = false;
        }

        public override void UpdateState()
        {
            HitBack();
        }

        public void ActionFinished()
        {
            ChangeState();
        }

        public void HitBack()
        {
            if (fsm.target.bHitBack)
            {
                offset += Time.deltaTime * 10;

                if (offset < fsm.target.data.hitBackOffset)
                {
                    fsm.target.transform.position += -1 * fsm.target.transform.forward * Time.deltaTime * 2;
                }
            }
        }

        private void ChangeState()
        {
            bool isNeedChangeStation = fsm.target.battleActionQueue.Dequeue(fsm.target.data.id, fsm.target.data.team == 0);  
            if (!isNeedChangeStation)
            {
                var stateName = fsm.GetString("beforeBeHitState");
                fsm.ChangeState(stateName);
            }
        }
    }
}