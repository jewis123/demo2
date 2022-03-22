using System;
using UnityEngine;

namespace Battle
{
    [Serializable]
    public class TeamData
    {
        public Vector3 teamPositions;
        public CharacterData[] charaters;
        public float teamRadius;
        public float moveSpeed;
        [Range(0,1)]
        public float teamRotateSpeed;
    }
}