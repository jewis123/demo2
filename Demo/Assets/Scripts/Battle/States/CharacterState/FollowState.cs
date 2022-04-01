using System;
using UnityEditor.UI;
using UnityEngine;

namespace Battle.States
{
    public class FollowState : ChargeableState,IAttentionable
    {
        private static readonly int SpeedHash = Animator.StringToHash("speed");
        private static readonly int NavSpeedHash = Animator.StringToHash("navSpeed");
        private float brakeSpeed;
        private float slowDownSpeed;
        private float offset;
        private bool isReached;
        private float enterTime;
        private float slowDownTime;
        
        public override void EnterState()
        {
            base.EnterState();
            
            offset = 0;
            brakeSpeed = slowDownSpeed = 0;
            isReached = false;
            var character = fsm.target;
            enterTime = slowDownTime = 0;
            
            character.IsIdle = true;
            // character.animator.SetFloat(SpeedHash,fsm.target.battle.battleSpeed);
            fsm.target.agent.speed = fsm.target.data.speed;
            fsm.target.animator.CrossFade("Move",0,0);

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

            if (fsm.target.IsAttention)
            {
                fsm.target.transform.LookAt(fsm.target.AttentionPos);
            }

            
            if (fsm.target.team.IsMyTeam)
            {
                bool backwardTeam = GetStandPosDistance() > fsm.target.team.BackwardDistance;
                if (fsm.target.team.InputTrigger || backwardTeam)
                {
                    fsm.target.SetDestination(fsm.target.StandPos);
                    // SpeedUp1();
                    SpeedUp2();
                }
                else
                {
                    // BrakeAfterReach();
                    BrakeInstancely();
                }
            }
            else
            {
                fsm.target.SetDestination(fsm.target.StandPos);
            }

            ChangeAnimation();
        }

        // private void BrakeAfterReach()
        // {
        //     if (!isReached)
        //     {
        //         fsm.target.SetDestination(fsm.target.StandPos);
        //     }
        //     
        //     if (isReached)
        //     {
        //         Braking();
        //     }
        //     
        //     if (!isReached && fsm.target.agent.remainingDistance < fsm.target.agent.stoppingDistance)
        //     {
        //         isReached = true;
        //     }
        // }

        private void BrakeInstancely()
        {
            BrakeWithRatio();
        }
        
        private void ChangeAnimation()
        {
            var agent = fsm.target.agent;
            var animator = fsm.target.animator;
            float percent = agent.speed / fsm.target.data.speed;

            animator.SetFloat(NavSpeedHash,percent);
        }

        private bool isOverShooting()
        {
            Vector3 destDir = (fsm.target.agent.destination - fsm.target.transform.position).normalized;
            Vector3 forword = fsm.target.agent.velocity.normalized;
            
            float angle = Vector3.Angle(forword, destDir);
            return angle > 90;
        }
        
        private void CheckOvershooting()
        {
            Vector3 destDir = (fsm.target.agent.destination - fsm.target.transform.position).normalized;
            if (isOverShooting())
            {
                fsm.target.agent.updateRotation = false;
                fsm.target.transform.forward = destDir;
                fsm.target.agent.velocity = destDir.normalized;
                fsm.target.agent.ResetPath();
                if (!fsm.target.IsAttention)
                {
                    fsm.target.agent.updateRotation = true;
                }
            }
        }

        private void StopNav()
        {
            if (fsm.target.team.InputTrigger)
            {
                return;
            }
            fsm.target.agent.isStopped = true;
            fsm.target.agent.velocity = Vector3.zero;
            fsm.target.agent.ResetPath();
            fsm.ChangeState<IdleState>();
        }

        // private void Braking()
        // {
        //     if (!fsm.target.team.IsMyTeam)
        //     {
        //         return;
        //     }
        //
        //     fsm.target.agent.SetDestination(fsm.target.transform.position + fsm.target.transform.forward
        //         * fsm.target.agent.speed) ;
        //     
        //     fsm.target.agent.speed -= fsm.target.data.brakeSpeed * Time.deltaTime;
        //     if (fsm.target.agent.speed <= 0)
        //     {
        //         StopNav();
        //     }
        // }
        
        private void BrakeWithRatio()
        {
            if (!fsm.target.team.IsMyTeam)
            {
                return;
            }

            float distance = GetStandPosDistance();
            
            if ( distance < fsm.target.data.brakeDistance)
            {
                if (brakeSpeed == 0)
                {
                    brakeSpeed = GetBrakeSpeed();
                }
                
                float ratio = distance / fsm.target.data.brakeDistance;
                float newSpeed = ratio * brakeSpeed;

                if (newSpeed < fsm.target.agent.speed)
                {
                    if (distance < fsm.target.data.slowDownDistance)
                    {
                        if (slowDownTime == 0)
                        {
                            slowDownTime = Time.time;
                        }

                        if (slowDownSpeed == 0)
                        {
                            slowDownSpeed = GetSlowDownSpeed();
                        }

                        float percent = fsm.target.data.slowDownDeltaSpeed * (Time.time - slowDownTime);
                        newSpeed = Mathf.Lerp(slowDownSpeed, fsm.target.data.slowDownSpeed, percent);
                    }

                    fsm.target.agent.speed = newSpeed;
                
                    // fsm.target.agent.SetDestination(fsm.target.transform.position + fsm.target.transform.forward
                        // * fsm.target.agent.speed) ;
                }else
                {
                    fsm.target.SetDestination(fsm.target.StandPos);
                }
            }else
            {
                fsm.target.SetDestination(fsm.target.StandPos);
            }
            
            // if (fsm.target.agent.speed <= .05f)
            // {
            //     StopNav();
            // }
            
            CheckDistanceStop();
        }

        private float GetBrakeSpeed()
        {
            float brakeSpeed;
            if (fsm.target.agent.speed == 0 && fsm.target.agent.remainingDistance > fsm.target.agent.stoppingDistance)
            {
                brakeSpeed = fsm.target.data.slowDownSpeed;
                fsm.target.SetDestination(fsm.target.StandPos);
                fsm.target.agent.speed = brakeSpeed;
            }
            else
            {
                brakeSpeed = fsm.target.agent.speed;
            }
            return brakeSpeed;
        }

        private float GetSlowDownSpeed()
        {
            float slowdownSpeed;
            if (fsm.target.agent.speed == 0 && fsm.target.agent.remainingDistance > fsm.target.agent.stoppingDistance)
            {
                slowdownSpeed = fsm.target.data.slowDownSpeed;
                fsm.target.SetDestination(fsm.target.StandPos);
                fsm.target.agent.speed = slowdownSpeed;
            }
            else
            {
                slowdownSpeed = fsm.target.agent.speed;
            }
            return slowdownSpeed;
        }
        
        /// <summary>
        /// 使用单体各自的速度和加速度
        /// </summary>
        private void SpeedUp1()
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

            CheckDistanceStop();

        }

        /// <summary>
        /// 启动前等待lagTime,使用单体加速度追赶TeamSpeed, 且最大速度不超过MaxSpeed;
        /// </summary>
        public void SpeedUp2()
        {
            if (!fsm.target.team.IsMyTeam)
            {
                return;
            }

            if (enterTime < fsm.target.data.lagtime)
            {
                enterTime += Time.deltaTime;
                fsm.target.agent.speed = 0;
                return;
            }

            bool continueSpeedUp = GetStandPosDistance() > fsm.target.team.BackwardDistance;
            bool lessMaxSpeed = fsm.target.agent.speed < fsm.target.data.speed;
            bool lessTeamSpeed = fsm.target.agent.speed < GetTeamSpeed();

            if (continueSpeedUp)
            {
                if (lessMaxSpeed)
                    fsm.target.agent.speed += fsm.target.data.accelerateSpeed * Time.deltaTime;
                else
                    fsm.target.agent.speed = fsm.target.data.speed;
            }
            else
            {
                if (lessTeamSpeed)
                    fsm.target.agent.speed += fsm.target.data.accelerateSpeed * Time.deltaTime;
                else
                    fsm.target.agent.speed = GetTeamSpeed();
            }

            CheckDistanceStop();
        }

        private void CheckDistanceStop()
        {
            Vector2 StandPosV2 = new Vector2(fsm.target.StandPos.x, fsm.target.StandPos.z);
            Vector2 NowPosV2 = new Vector2(fsm.target.transform.position.x, fsm.target.transform.position.z);
            float distance = Vector2.Distance(NowPosV2,StandPosV2);
            if (distance <= fsm.target.agent.stoppingDistance || fsm.target.agent.speed == 0 )
            {
                StopNav();
            }

            if (fsm.target.agent.speed != 0 && fsm.target.agent.velocity == Vector3.zero)
            {
                fsm.target.SetDestination(fsm.target.StandPos);
            }
        }

        public float GetStandPosDistance()
        {
            var nowPos = fsm.target.transform.position;
            var tarPos = fsm.target.StandPos;
            float distance = Vector2.Distance(new Vector2(nowPos.x, nowPos.z), new Vector2(tarPos.x, tarPos.z));
            return distance;
        }


        public float GetTeamSpeed()
        {
            if (fsm.target.IsAttention)
            {
                return fsm.target.team.TeamBattleSpeed;
            }

            return fsm.target.team.TeamSpeed;
        }

    }
}