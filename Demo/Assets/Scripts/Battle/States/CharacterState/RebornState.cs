using UnityEngine;

namespace Battle.States
{
    public class RebornState: FSMState<BattleCharacter>
    {
        public override void EnterState()
        {
            fsm.target.animator.CrossFade("reborn",0.05f,0);
            fsm.target.hud.SetHUDVisible(true);
        }

        public override void ExitState()
        {
        }

        public override void UpdateState()
        {
            var stateInfo = fsm.target.animator.GetCurrentAnimatorStateInfo(0);
            if (stateInfo.normalizedTime > 1)
            {
                fsm.ChangeState<IdleState>();
            }
        }
    }
}