using System;
using System.Collections.Generic;
using SuperCLine.ActionEngine;
using UnityEngine;

namespace Battle
{
	public class CharacterDebugGizmos: MonoBehaviour
	{
		public List<UDrawTool> _drawTools = new List<UDrawTool>();
		public float attackRadius;
		public float searchRadius;
		public float wanderRadius;
		public BattleCharacter Character;

		private void OnValidate()
		{
			Update();
		}

		public void Update()
		{

			_drawTools[0].DrawCircle(_drawTools[0].transform, transform.position, attackRadius, Color.red);
			_drawTools[1].DrawCircle(_drawTools[1].transform, transform.position, searchRadius, Color.blue);
			
			if (Character)
			{
				_drawTools[2].DrawCircle(_drawTools[2].transform, Character.StandPos, wanderRadius, Color.yellow);
			}
		}

		public void Init(float dataAttackRadius, float dataWanderRadius, float dataSearchRadius, BattleCharacter character)
		{
			attackRadius = dataAttackRadius;
			wanderRadius = dataWanderRadius;
			searchRadius = dataSearchRadius;
			Character = character;
		}
	}
}