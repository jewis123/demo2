using UnityEngine;

namespace Battle.States
{
    public class IdleState : ChargeableState
    {
        private float enterTm = 0;
        private float curTime;
        private float rangeTime;
        
        public override void EnterState()
        {            
            base.EnterState();

            enterTm = Time.time;
            fsm.target.IsIdle = true;
            GetRangeTime();
            fsm.target.animator.CrossFade("idle",0.2f,0);
            fsm.target.hud.SetHUDVisible(true);
            fsm.target.SetDestination(fsm.target.StandPos);
            fsm.target.agent.speed = 0;
            fsm.target.transform.rotation = fsm.target.team.TeamRotation;
            
        }

        private void GetRangeTime()
        {
            Random.InitState((int) (fsm.target.data.id * Time.time));
            rangeTime = enterTm + Random.Range(fsm.target.WanderTime.x * 100,fsm.target.WanderTime.y * 100)/100;
        }

        public override void ExitState()
        {
            
        }

        public override void UpdateState()
        {
            base.UpdateState();

            if (fsm.target.team.InputTrigger)
            {
                fsm.target.SetDestination(fsm.target.StandPos);
            }

            if (fsm.target.IsAttention)
            {
                fsm.target.animator.CrossFade("defence",0.2f,0);
                fsm.target.transform.LookAt(fsm.target.AttentionPos);
            }
            else
            {
                fsm.target.animator.CrossFade("idle",0.2f,0);
            }
            
            bool onDest = fsm.target.agent.destination == fsm.target.StandPos;
            bool inDist = fsm.target.agent.remainingDistance < fsm.target.agent.stoppingDistance;
            bool hasSpeed = fsm.target.agent.speed > 0;
            
            if ( ! onDest && !inDist && !fsm.target.ForceStoped && hasSpeed)
            {
                fsm.ChangeState<FollowState>();
                return;
            }
            
            enterTm += Time.deltaTime;

            if (fsm.target.team.IsMyTeam)
            {
                if (rangeTime < enterTm)
                {
                    if (!fsm.target.IsAttention)
                    {
                        fsm.ChangeState<WanderState>();
                    }
                    else
                    {
                        GetRangeTime();
                    }
                }
            }
        }
    }
}