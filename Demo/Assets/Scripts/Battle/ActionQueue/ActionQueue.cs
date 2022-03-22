using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;


namespace Battle
{
    public class ActionQueue
    {

        public struct QueueData
        {
            public int characterID;
            public string attacker;
        }
        
        private BattleGamePlay battle;
        private List<QueueData> MyActionQueue;
        private List<QueueData> EnermyActionQueue;
        
        private Action<string> OnMyQueueChanged;
        private Action<string> OnEnermyQueueChanged;

        private Action OnChangeMyTeamStation;
        private int stationChangeCompleteCnt;

        public bool isMyTeamHasHitted;

        public ActionQueue(BattleGamePlay battle)
        {
            this.battle = battle;
            MyActionQueue = new List<QueueData>();
            EnermyActionQueue = new List<QueueData>();
        }

        /// <summary>
        /// 角色指令队列缓存
        /// </summary>
        /// <param name="AttackerName"></param>
        /// <param name="characterID"></param>
        /// <param name="isMyTeam"></param>
        /// <param name="callBack">换位逻辑</param>
        public void EnQueue(string AttackerName, int characterID, bool isMyTeam, Action callBack = null)
        {

            if (isMyTeam)
            {
                OnChangeMyTeamStation = callBack;
                if (callBack != null)
                {
                    if (stationChangeCompleteCnt == 0)
                    {
                        stationChangeCompleteCnt = battle.teamManager.GetMyTeam().MemberCnt;
                        MyActionQueue.Add(new QueueData(){characterID = characterID, attacker =  AttackerName});
                        if (MyActionQueue.Count  == 1 && !isMyTeamHasHitted)
                        {
                            OnChangeMyTeamStation?.Invoke();
                        }
                        OnMyQueueChanged?.Invoke(GetAttackerQueueStr(isMyTeam));
                    }
                }
                else
                {
                    MyActionQueue.Add(new QueueData(){characterID = characterID, attacker =  AttackerName});
                    OnMyQueueChanged?.Invoke(GetAttackerQueueStr(isMyTeam));
                }
            }
            else
            {
                EnermyActionQueue.Add(new QueueData(){characterID = characterID, attacker =  AttackerName});
                OnEnermyQueueChanged?.Invoke(GetAttackerQueueStr(isMyTeam));
            }
            
        }


        /// <summary>
        /// 角色指令执行
        /// </summary>
        /// <param name="characterID"></param>
        /// <param name="isMyTeam"></param>
        /// <returns>是否接上换位</returns>
        public bool Dequeue(int characterID, bool isMyTeam)
        {
            if (isMyTeam)
            {
                isMyTeamHasHitted = false;

                for (int i = 0; i < MyActionQueue.Count; i++)
                {
                    if (MyActionQueue[i].characterID == characterID)
                    {
                        MyActionQueue.RemoveAt(i);
                        OnMyQueueChanged?.Invoke(GetAttackerQueueStr(isMyTeam));
                        break;
                    }
                }
                
                if (MyActionQueue.Count > 0 && MyActionQueue[0].characterID == 0)
                {
                    OnChangeMyTeamStation?.Invoke();
                    return true;
                }
            }
            else
            {
                for (int i = 0; i < EnermyActionQueue.Count; i++)
                {
                    if (EnermyActionQueue[i].characterID == characterID)
                    {
                        EnermyActionQueue.RemoveAt(i);
                        OnEnermyQueueChanged?.Invoke(GetAttackerQueueStr(isMyTeam));
                        break;
                    }
                }
            }
            
            //剔除队列中残留的死亡角色数据
            for (int i = battle.characterList.Count -1; i >= 0; i--)
            {
                for (int j = MyActionQueue.Count - 1; j >=0 ; j--)
                {
                    if (battle.characterList[i].IsDead && MyActionQueue[j].characterID == battle.characterList[i].data.id)
                    {
                        MyActionQueue.RemoveAt(j);
                    }
                }
                
                for (int k = EnermyActionQueue.Count - 1; k >=0 ; k--)
                {
                    if (battle.characterList[i].IsDead && EnermyActionQueue[k].characterID == battle.characterList[i].data.id)
                    {
                        EnermyActionQueue.RemoveAt(k);
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// 完成更换位置
        /// </summary>
        public void DequeueChangeStation()
        {
            stationChangeCompleteCnt -= 1;

            stationChangeCompleteCnt = Mathf.Max(0, stationChangeCompleteCnt);

            //第一个是换位动作，执行换位
            if (MyActionQueue.Count > 0 && stationChangeCompleteCnt == 0)
            {
                MyActionQueue.RemoveAt(0);
                OnMyQueueChanged?.Invoke(GetAttackerQueueStr(true));
            }
        }

        /// <summary>
        /// 清空角色指令队列
        /// </summary>
        /// <param name="characterID"></param>
        public void CancelCmd(int characterID, bool isMyTeam)
        {
            bool isChanged = false;
            if (isMyTeam)
            {
                for (int i = MyActionQueue.Count - 1; i >=0 ; i--)
                {
                    if (MyActionQueue[i].characterID == characterID)
                    {
                        MyActionQueue.RemoveAt(i);
                        isChanged = true;
                    }
                }
            }
            else
            {
                for (int i = EnermyActionQueue.Count - 1; i >=0 ; i--)
                {
                    if (EnermyActionQueue[i].characterID == characterID)
                    {
                        EnermyActionQueue.RemoveAt(i);
                        isChanged = true;
                    }
                }
            }

            if (isChanged)
            {
                if (isMyTeam)
                {
                    OnMyQueueChanged?.Invoke(GetAttackerQueueStr(isMyTeam));
                }
                else
                {
                    OnEnermyQueueChanged?.Invoke(GetAttackerQueueStr(isMyTeam));
                }
            }
        }

        public void ClearCmd()
        {
            MyActionQueue.Clear();
            EnermyActionQueue.Clear();
            OnMyQueueChanged?.Invoke("");
            OnEnermyQueueChanged?.Invoke("");
        }
        
        public void RegisterChange(Action<string> myTeamcallback, Action<string> enermyCallback)
        {
            OnMyQueueChanged = myTeamcallback;
            OnEnermyQueueChanged = enermyCallback;
        }

        private string GetAttackerQueueStr(bool isMyTeam)
        {
            StringBuilder sb = new StringBuilder();
            if (isMyTeam)
            {
                for (int i = 0; i < MyActionQueue.Count; i++)
                {
                    sb.Append($"\n{MyActionQueue[i].attacker}");
                }
            }else
            {
                for (int i = 0; i < EnermyActionQueue.Count; i++)
                {
                    sb.Append($"\n{EnermyActionQueue[i].attacker}");
                }
            }
            

            return sb.ToString();
        }
        
    }
}