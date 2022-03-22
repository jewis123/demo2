namespace SuperCLine.ActionEngine
{
	public class BattleUnit:Unit
	{
		public override EUnitType UnitType { get; }
		public override string ModelName { get; }
		
		public bool HasControl;

		
		public override void InitProperty(string resID)
		{
		}

		public void SetBattleProperties(float sSearchRadius, float attackRadius, float chaseRadius)
		{
			PropertyContext.SetProperty(PropertyName.sSearchDist, sSearchRadius);
			PropertyContext.SetProperty(PropertyName.sAttackDist, attackRadius);
			PropertyContext.SetProperty(PropertyName.sChaseDist, chaseRadius);
		}
	}
}