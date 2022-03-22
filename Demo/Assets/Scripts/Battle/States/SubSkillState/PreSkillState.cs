using UnityEngine;

namespace Battle.States.SubSkillState
{
    public class PreSkillState : FSMState<BattleCharacter>
    {
        private static readonly int SpeedHash = Animator.StringToHash("speed");
        private int stateHash;
        private float time;
        public override void EnterState()
        {

            time = 0;
            // fsm.target.animator.SetFloat(SpeedHash,fsm.target.battle.battleSpeed);

            stateHash = Animator.StringToHash("preSkill");
            fsm.target.animator.Play(stateHash,0,0);
            if (fsm.target.data.team == 1)
            {
                fsm.target.hud.SetSkillFlagVisible(true);
            }
        }

        public override void ExitState()
        {
        }

        public override void UpdateState()
        {
            if (time > fsm.target.data.carrySkill.preSkillTime)
            {
                fsm.ChangeState<ExecuteSkillState>();
            }
            else
            {
                //preSkillTime时间内处理逻辑
                time += Time.deltaTime;
            }
        }
    }
}