﻿using UnityEngine;

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
            Random.InitState((int) (fsm.target.data.id * Time.time));
            rangeTime = enterTm + Random.Range(fsm.target.WanderTime.x * 100,fsm.target.WanderTime.y * 100)/100;
            
            fsm.target.animator.CrossFade("idle",0.2f,0);
            fsm.target.hud.SetHUDVisible(true);
            fsm.target.SetDestination(fsm.target.StandPos);
            fsm.target.agent.speed = 0;
            fsm.target.transform.rotation = fsm.target.team.TeamRotation;
            
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
                    fsm.ChangeState<WanderState>();
                }
            }
        }
    }
}