using System;
using UnityEngine;

namespace Battle.Skill
{
    [Serializable]
    public struct SkillData
    {
        public int skillID;
        public float coolDown;
        public float skillArea;
        public float preSkillTime;
        public float preSkillIronTime;
        public float waitComboTime;
        public bool canCombo;
        public bool isAOE;
        public float bulletSpeed;
        public float skillDamage;
        public HitPoints[] comboHitPoints;
        public string[] comboSkillAudios;
        public string[] comboSkillHitAudios;
        public EffectBinding[] comboSkillEffect;
        public EffectBinding[] comboSkillHitEffect;
    }

    [Serializable]
    public struct HitPoints
    {
        [Range(0,1)]
        public float[] points;
    }
    
    [Serializable]
    public struct EffectBinding
    {
        public string effectPath;
        public string effectBonePath;
        public float duration;
    }
}