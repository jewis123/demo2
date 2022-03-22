using System.Collections.Generic;
using Battle.Config;
using UnityEngine;

namespace Battle
{
    public class BattleManager : MonoBehaviour
    {
        private Dictionary<int, BattleGamePlay> battles = new Dictionary<int, BattleGamePlay>();


        public void StartBattle(BattleDataConfig battleData)
        {
            GameObject battleGameplay = new GameObject($"Battle:{battleData.id}");
            var battleGamePlay = battleGameplay.AddComponent<BattleGamePlay>();
            battleGamePlay.Init(battleData);
            battles.Add(battleData.id,battleGamePlay);
            battleGamePlay.OnStart();
        }

        public void ExitBattle(int id)
        {
            if (battles.ContainsKey(id))
            {
                var battle = battles[id];
                battles.Remove(id);
                battle.OnExit();
                GameObject.Destroy(battle.gameObject);
                
            }
        }

    }
}