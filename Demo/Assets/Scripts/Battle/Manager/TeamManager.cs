using System;
using System.Collections.Generic;
using Battle.Config;
using Cinemachine;
using UnityEngine;

namespace Battle
{

    public class TeamManager
    {
        public int curTeamCnt;
        public BattleGamePlay battle;
        public Dictionary<int, Team> EnermyTeams = new Dictionary<int, Team>();
        private CameraTeamLook teamCameraLook;
        private CameraController camController;
        private Team myTeam;


        public TeamManager(BattleGamePlay battle)
        {
            this.battle = battle;
        }
        
        public Team CreateTeam(Vector3 teamPos, int cnt, float radius, float teamSpeed,float rotateSpeed,  bool isMyTeam, CharacterTeamPosConfig config=null ,float offsetRadius =0)
        {
            curTeamCnt++;
            Team newTeam = new Team(teamPos,cnt,radius, teamSpeed, rotateSpeed, isMyTeam, curTeamCnt, battle,offsetRadius);
            newTeam.InitTeamPos(isMyTeam, config);
            if (!isMyTeam)
            {
                EnermyTeams.Add(curTeamCnt, newTeam);
            }
            
            if (isMyTeam)
            {

                myTeam = newTeam;
                var teamCamera = GameObject.Find("TeamCamera");
                if (teamCamera != null)
                {
                    teamCameraLook = teamCamera.GetComponent<CameraTeamLook>();
                    camController = teamCamera.GetComponent<CameraController>();
                    teamCamera.GetComponent<CinemachineVirtualCamera>().Follow = newTeam.TargetGroup.transform;
                }
                else
                {
                    Debug.Log("缺少队伍相机");
                    // 手动创建过程省略。 。。 
                }

                if (teamCameraLook != null)
                {
                    teamCameraLook.lookTarget = newTeam.TargetGroup.transform;
                }

                if (camController!= null)
                {
                    camController.target = newTeam.TargetGroup.transform;
                }
            }
            
            return newTeam;
        }

        public Team GetMyTeam()
        {
            if (myTeam != null)
            {
                return myTeam;
            }

            return null;
        }

        public Team GetEnermyTeamByIndex(int index)
        {
            if (EnermyTeams.ContainsKey(index))
            {
                return EnermyTeams[index];
            }

            return null;
        }

        public bool IsMyTeamAllDead()
        {
            return myTeam.IsAllDead();
        }

        public bool IsEnermyAllDead()
        {
            foreach (var enermyTeam in EnermyTeams)
            {
                if (!enermyTeam.Value.IsAllDead())
                {
                    return false;
                }
            }

            return true;
        }
        
        public void UpdateTeamPos(int index, Vector3 pos)
        {
            Team team = GetEnermyTeamByIndex(index);
            if (team!=null)
            {
                team.TeamPos = pos;
            }
        }

        /// <summary>
        /// AI给输入
        /// </summary>
        /// <param name="index"></param>
        /// <param name="moveInput"></param>
        public void OnEnermyTeamMove(int index ,Vector2 moveInput)
        {
            Team team = GetEnermyTeamByIndex(index);
            if (team != null)
            {
                team.OnMove(moveInput);
            }
        }
        
        public void OnMyTeamMove(Vector2 moveInput)
        {
	        Team team = GetMyTeam();
	        if (team != null)
	        {
		        team.OnMove(moveInput);
	        }
        }
        
        public void Update()
        {
            foreach (var team in EnermyTeams)
            {
                team.Value.Update();
            }
            
            if (myTeam != null)
            {
                myTeam.Update();
            }
        }

        public void OnDestroy()
        {
            myTeam.OnDestroy();
            foreach (var enermyTeam in EnermyTeams)
            {
                enermyTeam.Value.OnDestroy();
            }
        }

        public void ChangeMemberPos()
        {
            // for (int i = 0; i < myTeam.Characters.Count; i++)
            // {
            //     myTeam.Characters[i].ChangeToIdle();
            // }
        }
    }
}