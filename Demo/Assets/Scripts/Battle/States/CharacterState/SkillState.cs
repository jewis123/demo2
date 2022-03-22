using Battle.States.SubSkillState;
using SuperCLine.ActionEngine;
using UnityEngine;

namespace Battle.States
{
    public class SkillState: FSMState<BattleCharacter>
    {

        public override void EnterState()
        {
            fsm.target.IsIdle = false;

            if (fsm.target.ComboIndex > fsm.target.data.AP)
            {
                fsm.ChangeState<Back2TeamState>();
                return;
            }


            if (fsm.target.Actor.Target == null )
            {
                fsm.ChangeState<IdleState>();
                return;
            }
            
            SelectTargetBySkillSystem();
        }

        public override void ExitState()
        {
            
        }

        public override void UpdateState()
        {
            // SelectEnermy();
        }

        public void SelectEnermy()
        {
            var character = fsm.target;
            
            var list = fsm.target.battle.characterList;
            var minDistance = float.MaxValue;
            BattleCharacter target = null;
            
            //选前排
            for (int j = 0; j < list.Count; j++)
            {
                if (list[j].data.team != character.data.team)
                {
                    if (list[j].IsDead)
                    {
                        continue;
                    }
                    var dir = list[j].transform.position
                              - character.transform.position;
                    var distance = dir.sqrMagnitude;
                    if (distance < minDistance)
                    {
                        target = list[j];
                        minDistance = distance;
                    }
                }
            }

            if (minDistance > fsm.target.data.attackRadius)
            {
                if (fsm.target.data.team == 0)
                {
                    fsm.target.attackTriggered = false;
                    fsm.ChangeState<IdleState>();
                    
                    return;
                    
                }
                return;
            }


            if (target != null)
            {
                AttackTarget(target.gameObject);
            }
        }

        public void SelectTargetBySkillSystem()
        {
            BattleUnit unit = fsm.target.Actor as BattleUnit;
            fsm.target.battle.ActorAdapter.SelectTarget(unit, AttackTarget);
        }

        private void AttackTarget(GameObject target)
        {
            if (target == null)
            {
                fsm.ChangeState<IdleState>();
                return;
            }
            BattleCharacter attackee = target.GetComponent<BattleCharacter>();
            if (attackee == null)
            {
                fsm.ChangeState<IdleState>();
                return;
            }
            
            fsm.target.battleActionQueue.EnQueue(fsm.target.name,fsm.target.data.id, fsm.target.data.team == 0);

            fsm.target.SetEnergy(-1);
            fsm.SetFloat("coolDownProgress", -1);

            if (fsm.target.data.team == 0)
            {
                fsm.target.attackTriggered = false;
            }
            
            var attacker = fsm.target;
            if (attacker.data.moveable)
            {
                fsm.SetObject("MoveTarget", attackee);
                fsm.ChangeState<WalkTowardState>();
                return;
            }
            
            fsm.SetObject("MoveTarget", attackee);
            fsm.ChangeState<PreSkillState>();
        }
    }
}