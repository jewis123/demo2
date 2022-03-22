using Battle.States.SubSkillState;
using UnityEngine;

namespace Battle.States
{
    public class WalkTowardState: FSMState<BattleCharacter>
    {
        private static readonly int SpeedHash = Animator.StringToHash("speed");
        
        private BattleCharacter target;

        
        public override void EnterState()
        {
            // fsm.target.animator.SetFloat(SpeedHash,fsm.target.battle.battleSpeed);
            fsm.target.animator.CrossFade("running",0,0);
            target = fsm.GetObject<BattleCharacter>("MoveTarget");
            fsm.target.SetDestination(fsm.target.StandPos);
        }

        public override void ExitState()
        {
            target = null;
            fsm.target.animator.SetFloat(SpeedHash,fsm.target.battle.battleSpeed);  //还原移动速度影响
        }

        public override void UpdateState()
        {
            var character = fsm.target;
            if (target != null)
            {
                character.agent.destination = target.transform.position;
                fsm.target.transform.forward = (target.transform.position - fsm.target.transform.position).normalized;
            }

            Vector3 myPos = character.transform.position;
            bool chaseInvaild = Vector2.Distance(new Vector2(myPos.x, myPos.z), new Vector2(character.StandPos.x, character.StandPos.z)) > fsm.target.data.chaseRadius;

            if (chaseInvaild)
            {
                fsm.ChangeState<Back2TeamState>();
                return;
            }

            if (Vector3.Distance(character.transform.position, target.transform.position) < character.data.attackRadius)
            {
                fsm.target.agent.isStopped = true;
                fsm.target.agent.ResetPath();
                
                fsm.ChangeState<PreSkillState>();
            }
        }
    }
}