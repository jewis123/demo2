using UnityEngine;

namespace Battle.States.SubSkillState
{
    public class AfterSkillState : FSMState<BattleCharacter>
    {
        private int stateHash;
        private float time;
        private static readonly int SpeedHash = Animator.StringToHash("speed");
        
        public override void EnterState()
        {
            // fsm.target.animator.SetFloat(SpeedHash,fsm.target.battle.battleSpeed);
            fsm.target.animator.CrossFade("afterSkill",0.1f,0);
            fsm.target.isIronBody = true;
            time =  0;
        }

        public override void ExitState()
        {
        }

        public override void UpdateState()
        {
            var stateInfo = fsm.target.animator.GetCurrentAnimatorStateInfo(0);

            if (stateInfo.IsName("afterSkill"))
            {
                if (stateInfo.normalizedTime > 1)
                {
                    fsm.target.isIronBody = false;

                    if (fsm.target.data.team == 0)
                    {
                        fsm.ChangeState<WaitComboState>();
                        return;
                    }
                    
                    fsm.target.hud.SetSkillFlagVisible(false);
                    fsm.ChangeState<Back2TeamState>();
                }
                else
                {
                    time += Time.deltaTime;
                }
            }
        }
    }
}