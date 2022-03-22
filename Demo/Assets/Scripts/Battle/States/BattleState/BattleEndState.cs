namespace Battle.States.BattleState
{
    public class BattleEndState: FSMState<BattleGamePlay>
    {
        public override void EnterState()
        {
            fsm.target.OnGameOver?.Invoke();
        }

        public override void ExitState()
        {
        }

        public override void UpdateState()
        {
        }
    }
}