using System;
using Battle;
using Scene.SceneControllers;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Wnds
{
    public class BattleSceneUIWnd : UIController
    {
        private BattleGamePlay gamePlay;
        
        public Text speedText;
        public Text MyActionsText;
        public Text EnermyActionsText;
        public Button attacker1;
        public Button attacker2;
        public Button attacker3;
        public Button attacker4;
        public Button changeStand;
        public Button battleSpeed;
        public GameObject GameOver;

        private int lastSpeed;
        
        public override void OnOpen(object[] param)
        {
            gamePlay = GameObject.Find("Battle:1").GetComponent<BattleGamePlay>();
            gamePlay.actionqueue.RegisterChange(OnMyQueueChanged,OnEnermyQueueChanged);
            gamePlay.OnGameplayInited += BindInputEvent;
            gamePlay.OnGameOver += OnGameOver;
        }

        private void OnGameOver()
        {
            GameOver.SetActive(true);
        }

        public void OnMyQueueChanged(string str)
        {
            MyActionsText.text = str;
        }

        private void Update()
        {
            if (lastSpeed != gamePlay.battleSpeed)
            {
                lastSpeed = gamePlay.battleSpeed;
                speedText.text = $"倍数*{gamePlay.battleSpeed}（Q）";
            }
        }

        public void OnEnermyQueueChanged(string str)
        {
            EnermyActionsText.text = str;
        }

        public override void OnClose()
        {
            gamePlay.OnGameplayInited -= BindInputEvent;
        }
        
        public void OnBackButtonClk()
        {
            
            // GameCore.singleton.gameSceneManager.UnLoadScene<BattleScene>();
            GameCore.singleton.battleManager.ExitBattle(1);
            
        }

        private void BindInputEvent()
        {
            attacker1.onClick.AddListener(gamePlay.Input.OnAttack_1);
            attacker2.onClick.AddListener(gamePlay.Input.OnAttack_2);
            attacker3.onClick.AddListener(gamePlay.Input.OnAttack_3);
            attacker4.onClick.AddListener(gamePlay.Input.OnAttack_4);
            changeStand.onClick.AddListener(gamePlay.Input.OnChangeStation);
            battleSpeed.onClick.AddListener(gamePlay.Input.OnChangeBattleSpeed);
        }
    }
}