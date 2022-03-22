using System;
using System.Collections.Generic;
using SuperCLine.ActionEngine;
using UnityEngine;

namespace Battle
{
    public class ActorAdapter:IDisposable
    {
        private BattleGamePlay battle;
        private SkillManager actorManager;

        public ActorAdapter(BattleGamePlay battle)
        {
            this.battle = battle;
            actorManager = SkillManager.Instance;
        }

        public void CreateCharacterUnit(Vector3 spawnPos, ref CharacterData data, ref BattleCharacter character)
        {
            Unit pUnit = UnitMgr.Instance.CreateUnit(EUnitType.EUT_Player, data.id.ToString(), spawnPos, 0f, ECampType.EFT_Friend);
            character = pUnit.UObject.gameObject.AddComponent<BattleCharacter>();
            character.Actor = pUnit;
            pUnit.OnHit = character.BeHit;
        }
        
        public void CreateMonsterUnit(Vector3 spawnPos, ref CharacterData data, ref BattleCharacter character)
        {
            Unit pUnit = UnitMgr.Instance.CreateUnit(EUnitType.EUT_Monster, data.id.ToString(), spawnPos, 0, ECampType.EFT_Enemy);
            character = pUnit.UObject.gameObject.AddComponent<BattleCharacter>();
            character.Actor = pUnit;
            pUnit.OnHit = character.BeHit;
        }
        
        public void SelectTarget(BattleUnit unit, Action<GameObject> searchCB)
        {
            GameObject obj = unit.Target == null ? null : unit.Target.UObject.gameObject;
            searchCB?.Invoke(obj);
        }
        
        public void Dispose()
        {
            GameObject.Destroy(actorManager);
        }
    }
}