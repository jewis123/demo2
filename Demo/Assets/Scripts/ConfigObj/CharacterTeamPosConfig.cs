using System;
using System.Collections.Generic;
using UnityEngine;

namespace Battle.Config
{
    [CreateAssetMenu(fileName = "CharacterTeamPosConfig", menuName = "BattleDemo/CharacterTeamPosConfig", order = 0)]
    public class CharacterTeamPosConfig : ScriptableObject
    {
        public List<int> characters;
        public int colCount;
        public float gapSize;
    }
}


