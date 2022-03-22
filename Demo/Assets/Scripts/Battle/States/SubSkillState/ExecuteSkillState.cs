using System.Collections.Generic;
using Battle.Skill;
using SuperCLine.ActionEngine;
using UnityEngine;

namespace Battle.States.SubSkillState
{
    public class ExecuteSkillState : FSMState<BattleCharacter>
    {
        private static readonly int SpeedHash = Animator.StringToHash("speed");
        private int stateHash;
        private int hitted = -1;
        private BattleUnit unit;
        
        public override void EnterState()
        {
            fsm.target.isIronBody = true;
            
            // fsm.target.animator.SetFloat(SpeedHash,fsm.target.battle.battleSpeed);
            fsm.target.Actor.ActionStatus.OnActionFinishCB += SkillFinished;
            fsm.target.Actor.ActionStatus.ChangeAction(GetAttackName());
            unit = fsm.target.Actor as BattleUnit;
            unit.HasControl = true;

        }

        public override void ExitState()
        {
            fsm.target.Actor.ActionStatus.OnActionFinishCB -= SkillFinished;
            unit.HasControl = false;
        }

        public override void UpdateState()
        {
        }

        public void SkillFinished()
        {
            if (fsm.target.EnergyCount() > 0)
            {
                fsm.ChangeState<WaitComboState>();
                return;
            }
            
            fsm.ChangeState<AfterSkillState>();
        }
        
        public string GetAttackName()
        {
            if (fsm.target.data.carrySkill.canCombo)
            {
                return $"attack{fsm.target.ComboIndex}";
            }

            return "attack1";
        }
    }
}