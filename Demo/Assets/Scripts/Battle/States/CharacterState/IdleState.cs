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
            if (fsm.target.name.StartsWith("character2"))
            {
                Debug.Log("fffffffffffffffffffff");
            }
        }

        public override void ExitState()
        {
            
        }

        public override void UpdateState()
        {
            base.UpdateState();

            if (fsm.target.InputTrigger)
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