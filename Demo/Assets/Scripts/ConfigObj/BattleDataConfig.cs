using UnityEngine;

namespace Battle.Config
{
    [CreateAssetMenu(fileName = "BattleData", menuName = "BattleDemo/BattleData", order = 0)]
    public class BattleDataConfig : ScriptableObject
    {
        public int id;
        public float myTeamRadius;
        public float myTeamMoveSpeed;
        [Range(0,1)]
        public float myTeamRotateSpeed;
        [Range(0, 5)]
        public float teamPosOffsetRadius;

        public Vector2 startWanderTime;

        public CharacterData[] characterData;
                
        public TeamData[] enermyTeamData;
    }
}