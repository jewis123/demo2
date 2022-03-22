using UnityEngine;

namespace Battle.States
{
    public class ChangeStationState: FSMState<BattleCharacter>
    {
        private Vector3 targetPos;
        private static readonly int SpeedHash = Animator.StringToHash("speed");
        
        public override void EnterState()
        {
            if (fsm.target.IsDead)
            {
                return;
            }

            fsm.target.IsIdle = false;
            
            targetPos = fsm.target.StandPos;
            
            if (!fsm.target.IsDead)
            {
                fsm.target.animator.CrossFade("running",0.05f,0);
            }

            fsm.target.SetDestination(targetPos);
        }

        public override void ExitState()
        {
            fsm.target.animator.SetFloat(SpeedHash,fsm.target.battle.battleSpeed);  //还原移动速度影响
        }

        public override void UpdateState()
        {
            var character = fsm.target;
            
            if (character.IsDead)
            {
                character.battleActionQueue.DequeueChangeStation();
                return;
            }
            if (fsm.target.agent.remainingDistance <= fsm.target.agent.stoppingDistance)
            {
                character.battleActionQueue.DequeueChangeStation();
                fsm.ChangeState<IdleState>();
            }
        }
    }
}