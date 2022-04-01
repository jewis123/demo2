using UnityEngine;

namespace Battle.States.SubSkillState
{
    public class WaitComboState : FSMState<BattleCharacter>
    {
        private float time;
        public override void EnterState()
        {
            time =  0;
            
            fsm.target.IsAttackTriggered = false;

            fsm.target.animator.CrossFade("idle",0,0);
        }

        public override void ExitState()
        {
            
        }

        public override void UpdateState()
        {
            if (fsm.target.EnergyCount() == 0)
            {
                FinishAction();
                return;
            }
            
            if (time > fsm.target.data.carrySkill.waitComboTime)
            {
                FinishAction();
            }
            else
            {
                time += Time.deltaTime;
            }
             
            if (fsm.target.IsAttackTriggered)
            {
                if (fsm.target.EnergyCount() > 0)
                {
                    fsm.target.IsAttackTriggered = false;
                    EnQueueCmd();
                }
            }
        }

        public void FinishAction()
        {
            fsm.ChangeState<Back2TeamState>();
        }
           
        public void EnQueueCmd()
        {
            var character = fsm.target;
            
            if (fsm != null)
            {
                fsm.target.ComboIndex += 1;
                fsm.ChangeState<SkillState>();
                return;
            }
            
            if (fsm.target.ComboIndex == 1)
            {
                character.battleActionQueue.EnQueue(character.gameObject.name,character.data.id, fsm.target.data.team == 0);
            }
        }
    }
}