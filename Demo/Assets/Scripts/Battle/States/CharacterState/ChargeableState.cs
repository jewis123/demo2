using UnityEngine;

namespace Battle.States
{
    public class ChargeableState : FSMState<BattleCharacter>
    {
        private float enterTm = 0;
        
        public override void EnterState()
        {
            var lastProgress = fsm.GetFloat("coolDownProgress");
            if (lastProgress >=0 )
            {
                enterTm = Time.time - lastProgress * fsm.target.data.carrySkill.coolDown;
            }
            else
            {
                enterTm = Time.time;
            }
        }

        public override void ExitState()
        {
        }

        public override void UpdateState()
        {
            var character = fsm.target;
            float progress = 0;
            
            progress = (Time.time - enterTm) /  fsm.target.data.carrySkill.coolDown;
            fsm.target.hud.ChangeActionBar(progress);
            fsm.SetFloat("coolDownProgress",progress);

            if (Time.time > enterTm + character.data.carrySkill.coolDown)
            {
                if (progress >= 1)
                {
                    if (fsm.target.SetEnergy(1))
                    {
                        if (fsm.target.EnergyCount() < 3 && fsm.target.data.team == 0)
                        {
                            enterTm = Time.time;
                        }
                    }
                    Attack();
                }
            }
            else
            {
                if (fsm.target.EnergyCount() > 0)
                {
                    Attack();
                }
            }
        }
              
        private void Attack()
        {
            var character = fsm.target;

            if (character.data.team == 0)
            {
                if (character.attackTriggered)
                {
                    fsm.ChangeState<SkillState>();
                }
            }
            else
            {
                character.battleActionQueue.EnQueue(character.gameObject.name,character.data.id, fsm.target.data.team == 0);
                fsm.ChangeState<SkillState>();
            }
        }
    }
}