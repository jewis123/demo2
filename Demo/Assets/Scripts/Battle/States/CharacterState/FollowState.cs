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
        
        public override void EnterState()
        {
            offset = 0;
            curSpeed = 0;
            var character = fsm.target;
            
            character.IsIdle = true;
            // character.animator.SetFloat(SpeedHash,fsm.target.battle.battleSpeed);
            fsm.target.agent.speed = fsm.target.data.speed;
            fsm.target.animator.CrossFade("running",0,0);
            base.EnterState();
            if (fsm.target.name.StartsWith("character2"))
            {
                Debug.Log("FollowState");
            }
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
            
            if (fsm.target.Braking)
            {
                Braking();
            }
            else
            {
                SpeedUp();
            }

            fsm.target.SetDestination(fsm.target.StandPos);
            if (fsm.target.agent.remainingDistance < fsm.target.agent.stoppingDistance)
            {
                StopNav();
            }

            // ChangeAnimation();
        }

        private void ChangeAnimation()
        {
            var agent = fsm.target.agent;
            var animator = fsm.target.animator;
            float percent = agent.speed / fsm.target.data.speed;
            if (!animator.GetCurrentAnimatorStateInfo(0).IsName("Move"))
                animator.CrossFade("Move",0f,0);

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
            fsm.target.agent.ResetPath();
            fsm.ChangeState<IdleState>();
        }

        private void Braking()
        {
            if (!fsm.target.team.IsMyTeam)
            {
                return;
            }
            
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