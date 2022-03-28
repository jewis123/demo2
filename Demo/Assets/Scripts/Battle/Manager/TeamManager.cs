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

        private Team myTeam;


        public TeamManager(BattleGamePlay battle)
        {
            this.battle = battle;
        }
        
        public Team CreateTeam(Vector3 teamPos, int cnt, bool isMyTeam)
        {
            curTeamCnt++;
            Team newTeam = new Team(teamPos,cnt, isMyTeam, curTeamCnt, battle);

            if (!isMyTeam)
            {
                EnermyTeams.Add(curTeamCnt, newTeam);
            }
            else
            {
                myTeam = newTeam;
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