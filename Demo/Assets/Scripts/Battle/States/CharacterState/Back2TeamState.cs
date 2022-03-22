using UnityEngine;

namespace Battle.States
{
    public class Back2TeamState : FSMState<BattleCharacter>
    {
        private static readonly int SpeedHash = Animator.StringToHash("speed");
        public override void EnterState()
        {
            // fsm.target.animator.SetFloat(SpeedHash,fsm.target.battle.battleSpeed);
            fsm.target.animator.CrossFade("running",0,0);
        }

        public override void ExitState()
        {
            
        }

        public override void UpdateState()
        {
            fsm.target.SetDestination(fsm.target.StandPos);
            if (fsm.target.agent.remainingDistance <= fsm.target.agent.stoppingDistance)
            {
                bool isNeedChangeStation = fsm.target.battleActionQueue.Dequeue(fsm.target.data.id, fsm.target.data.team == 0);
                if (!isNeedChangeStation)
                {
                    fsm.ChangeState<IdleState>();
                }
            }
        }
    }
}