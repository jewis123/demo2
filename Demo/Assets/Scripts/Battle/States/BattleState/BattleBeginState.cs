namespace Battle.States.BattleState
{
    public class BattleBeginState: FSMState<BattleGamePlay>
    {
        public override void EnterState()
        {
            fsm.target.IsGameOver = false;
        }

        public override void ExitState()
        {
        }

        public override void UpdateState()
        {
        }
    }
}