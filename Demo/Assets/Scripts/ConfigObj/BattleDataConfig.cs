using UnityEngine;

namespace Battle.Config
{
    [CreateAssetMenu(fileName = "BattleData", menuName = "BattleDemo/BattleData", order = 0)]
    public class BattleDataConfig : ScriptableObject
    {
        public int id;
        public float myTeamRadius;
        public float myTeamMoveSpeed;
        public float myTeamRotateSpeed;
        public float teamAttentionRadius;
        public float teamBattleSpeed;
        public float teamWanderRadius;
        [Tooltip("x-y之间取一个随机数时间，开始游走")]
        public Vector2 startWanderTime;
        
        [Tooltip("成员落后多远开始追赶队伍")]
        public float teamDistance;
        
        public CharacterData[] characterData;
                
        public TeamData[] enermyTeamData;
    }
}