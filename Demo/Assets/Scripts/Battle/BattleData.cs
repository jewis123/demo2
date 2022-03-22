using System;
using Battle.Skill;
using UnityEngine;

namespace Battle
{
    [Serializable]
    public struct CharacterData
    {
        public int id;
        public int team;
        public float HP;
        [Tooltip("行动格数")]
        public int AP;
        public float speed;
        [Range(0,10f)]
        public float accelerateSpeed;
        [Range(0,10f)]
        public float brakeSpeed;
        [Tooltip("攻击范围")]
        public float attackRadius;
        [Tooltip("索敌范围")]
        public float searchRadius;
        [Tooltip("追击范围")]
        public float chaseRadius;
        [Tooltip("待机游走半径")]
        public float wanderRadius;

        public bool moveable;
        
        [SerializeField] public CharacterAttribute attribute;
        
        [SerializeField] public SkillData carrySkill;
        
        public JObType job;

        public float hitBackOffset;
        
        [Range(0f,1f)]
        public float beHitRatio;
        public string beHitAudio;
    }

    public enum JObType
    {
        Warrior,
        ADC,
        Mage,
    }
    
    [Serializable]
    public struct CharacterAttribute
    {
        public float damage;
        public float defence;
    }
}