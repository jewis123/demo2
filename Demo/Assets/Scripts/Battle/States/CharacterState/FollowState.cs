using System;
using UnityEngine;

namespace Battle.States
{
    public class FollowState : ChargeableState
    {
        private static readonly int SpeedHash = Animator.StringToHash("speed");
        private static readonly int NavSpeedHash = Animator.StringToHash("navSpeed");
        private float curSpeed;
        private float offset;
        private bool isReached;
        
        public override void EnterState()
        {
            offset = 0;
            curSpeed = 0;
            isReached = false;
            var character = fsm.target;
            
            character.IsIdle = true;
            // character.animator.SetFloat(SpeedHash,fsm.target.battle.battleSpeed);
            fsm.target.agent.speed = fsm.target.data.speed;
            fsm.target.animator.CrossFade("Move",0,0);
            base.EnterState();
        }
        
        public override void ExitState()
        {
            float value = Mathf.Clamp01(fsm.GetFloat("coolDownProgress"));
            fsm.target.hud.ChangeActionBar(value);
            fsm.SetFloat("coolDownProgress",value);
        }
        
        public override void UpdateState()
        {
            base.UpdateState();

            if (fsm.target.ForceStoped)
            {
                StopNav();
                return;
            }
            
            CheckOvershooting();

            if (fsm.target.InputTrigger)
            {
                SpeedUp();
            }

            if (!isReached)
            {
                fsm.target.SetDestination(fsm.target.StandPos);
            }
            
            if (!fsm.target.InputTrigger && isReached)
            {
                Braking();
            }
            
            if (!isReached && fsm.target.agent.remainingDistance < fsm.target.agent.stoppingDistance)
            {
                isReached = true;
            }

            ChangeAnimation();
        }

        private void ChangeAnimation()
        {
            var agent = fsm.target.agent;
            var animator = fsm.target.animator;
            float percent = agent.speed / fsm.target.data.speed;

            animator.SetFloat(NavSpeedHash,percent);

        }

        private void CheckOvershooting()
        {
            Vector3 destDir = (fsm.target.agent.destination - fsm.target.transform.position).normalized;
            Vector3 forword = fsm.target.agent.velocity.normalized;
            
            float angle = Vector3.Angle(forword, destDir);

            if (angle > 90)
            {
                fsm.target.agent.updateRotation = false;
                fsm.target.transform.forward = destDir;
                fsm.target.agent.ResetPath();
                fsm.target.agent.updateRotation = true;
            }
        }

        private void StopNav()
        {
            fsm.target.agent.isStopped = true;
            fsm.target.agent.velocity = Vector3.zero;
            fsm.target.agent.ResetPath();
            fsm.ChangeState<IdleState>();
        }

        private void Braking()
        {
            if (!fsm.target.team.IsMyTeam)
            {
                return;
            }

            fsm.target.agent.SetDestination(fsm.target.transform.position + fsm.target.transform.forward
                * fsm.target.agent.speed) ;
            
            fsm.target.agent.speed -= fsm.target.data.brakeSpeed * Time.deltaTime;
            if (fsm.target.agent.speed <= 0)
            {
                StopNav();
            }
        }
        
        private void SpeedUp()
        {
            if (!fsm.target.team.IsMyTeam)
            {
                return;
            }
            
            fsm.target.agent.speed += fsm.target.data.accelerateSpeed * Time.deltaTime;
            
            if (fsm.target.agent.speed > fsm.target.data.speed)
            {
                fsm.target.agent.speed = fsm.target.data.speed;
            }
            
            if (fsm.target.agent.remainingDistance <= fsm.target.agent.stoppingDistance)
            {
                fsm.ChangeState<IdleState>();
            }
        }
    }
}