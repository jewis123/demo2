using System;
using System.Collections.Generic;
using System.Linq;
using Battle.Config;
using Cinemachine;
using UnityEngine;

namespace Battle
{

    public class TeamManager
    {
        public int curTeamCnt;
        public BattleGamePlay battle;
        public Dictionary<int, Team> Teams = new Dictionary<int, Team>();

        private Team myTeam;


        public TeamManager(BattleGamePlay battle)
        {
            this.battle = battle;
        }
        
        public Team CreateTeam(Vector3 teamPos, int cnt, bool isMyTeam)
        {
            Team newTeam = new Team(teamPos,cnt, isMyTeam, curTeamCnt, battle, this);

            if (!isMyTeam)
            {
                Teams.Add(curTeamCnt, newTeam);
            }
            else
            {
                myTeam = newTeam;
            }
            curTeamCnt++;
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
            if (Teams.ContainsKey(index))
            {
                return Teams[index];
            }

            return null;
        }

        public bool IsMyTeamAllDead()
        {
            return myTeam.IsAllDead();
        }

        public bool IsEnermyAllDead()
        {
            foreach (var enermyTeam in Teams)
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
            foreach (var team in Teams)
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
            foreach (var enermyTeam in Teams)
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

        public bool SearchEnermy(ref Vector3 nearestEnermyPos)
        {
            int nearestTeamIdx = -1;
            float nearestTeamDis = Mathf.Infinity;
            float  newDis;
            Vector3 pos = Vector3.zero;
            for (int i = 0; i < Teams.Count; i++)
            {
                var team = Teams.Values.ElementAt(i);
                for (int j = 0; j < team.MemberPoses.Length; j++)
                {
                    newDis = Vector3.Distance(team.MemberPoses[j], myTeam.GetTeamCenter());
                    if ( newDis <= myTeam.AttentionRadius)
                    {
                        if (newDis < nearestTeamDis)
                        {
                            nearestTeamDis = newDis;
                            nearestTeamIdx = i;
                            pos = team.MemberPoses[j];
                        }
                    }
                }
            }

            if (nearestTeamIdx != -1)
            {
                nearestEnermyPos = pos;
            }
            return nearestTeamIdx != -1;
        }
    }
}