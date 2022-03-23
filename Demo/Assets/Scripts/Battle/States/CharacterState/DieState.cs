using UnityEngine;

namespace Battle.States
{
    public class DieState: FSMState<BattleCharacter>
    {
        private static readonly int SpeedHash = Animator.StringToHash("speed");
        private bool isChecked = false;
        public override void EnterState()
        {
            isChecked = false;
            fsm.target.IsIdle = true;
            // fsm.target.animator.SetFloat(SpeedHash,fsm.target.battle.battleSpeed);
            fsm.target.animator.CrossFade("dying",0.5f,0);
            fsm.target.transform.localPosition = Vector3.zero; //兼容临时动画结束后动作没回到原点问题
            fsm.target.battleActionQueue.DequeueChangeStation();  //兼容走位途中死亡的情况
            fsm.target.hud.ChangeActionBar(0);
        }

        public override void ExitState()
        {
        }

        public override void UpdateState()
        {
            if (isChecked)
            {
                return;
            }
            var stateInfo = fsm.target.animator.GetCurrentAnimatorStateInfo(0);
            if (stateInfo.normalizedTime > 1)
            {
                fsm.target.hud.SetHUDVisible(false);
                fsm.target.battle.CheckGameOver(fsm.target.data.team);
                isChecked = true;
            }
        }
    }
}