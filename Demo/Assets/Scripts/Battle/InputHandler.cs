using UnityEngine;
using UnityEngine.InputSystem;

namespace Battle
{
    public class InputHandler:MonoBehaviour
    {
        public BattleGamePlay gamePlay;

        public void OnEnable()
        {
            gamePlay = GetComponent<BattleGamePlay>();
        }

        private void SetInputTrigger(int characterIdx)
        {
            if (gamePlay.characterList == null || gamePlay.IsGameOver)
            {
                return;
            }
            
            for (int i = 0; i < gamePlay.characterList.Count; i++)
            {
                if (gamePlay.characterList[i].data.id == characterIdx)
                {
                    if (gamePlay.characterList[i].EnergyCount() > 0)
                    {
                        gamePlay.characterList[i].attackTriggered = true;
                    }
                    return;
                }
            }
        }
        
        public void OnAttack_1()
        {
            SetInputTrigger(1);
        }

        public void OnAttack_2()
        {
            SetInputTrigger(2);
        }

        public void OnAttack_3()
        {
            SetInputTrigger(3);
        }
        
        public void OnAttack_4()
        {
            SetInputTrigger(4);
        }

        public void OnChangeStation()
        {
            //轮换站位
            if (gamePlay.characterList == null || gamePlay.IsGameOver)
            {
                return;
            }
            
            gamePlay.actionqueue.EnQueue("更换站位", 0, true, () =>
            {
                for (int i = 0; i < gamePlay.characterList.Count; i++)
                {
                    if (gamePlay.characterList[i].data.team == 0)
                    {
                        gamePlay.characterList[i].ChangeStation();
                    }
                }
            });
        }
        
        public void OnChangeBattleSpeed()
        {
            //轮换站位
            if (gamePlay.IsGameOver)
            {
                return;
            }
            gamePlay.battleSpeed += 1;
            gamePlay.battleSpeed %= 3;
            gamePlay.battleSpeed = gamePlay.battleSpeed == 0 ? 1 : gamePlay.battleSpeed;
            gamePlay.battleSpeed = gamePlay.battleSpeed;
        }

        public void OnMove(InputValue value)
        {
            if (gamePlay.teamManager != null)
            {
                gamePlay.teamManager.OnMyTeamMove(value.Get<Vector2>());
            }
        }
    }
}