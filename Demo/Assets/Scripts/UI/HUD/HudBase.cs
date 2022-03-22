using System;
using Battle;
using UnityEngine;
using UnityEngine.UI;

namespace UI.HUD
{
    public class HudBase:MonoBehaviour
    {
        public Slider ActionSlider;
        public Slider HpSlider;
        public GameObject SkillFlag;
        
        private float MaxHp;
        private int MaxActionCount;


        private int curActionCnt;
        private float curHP;
        private BattleCharacter _character;

        public int EnergyCount
        {
            get
            {
                return curActionCnt;
            }
            set
            {
                curActionCnt = value;
               
                // ActionSlider.value = (float)curActionCnt / MaxActionCount;    等换成环形UI解封
                
                if (_character.data.team == 0)
                {
                    for (int i = 1; i <= 3; i++)
                    {
                        string name = $"Slider/energy{i}";
                        var energy = transform.Find(name).gameObject;
                        energy.SetActive(i <= curActionCnt);
                    }
                }
            }
        }

        private void OnEnable()
        {
            ActionSlider = transform.Find("Slider").GetComponent<Slider>();
            HpSlider = transform.Find("Blood").GetComponent<Slider>();

            OnInit();
        }

        protected virtual void OnInit()
        {
        }

        public void InitData(float hp, int actionCount, BattleCharacter character)
        {
            MaxHp = hp;
            MaxActionCount = actionCount;
            _character = character;
            curHP = MaxHp;
        }

        public void SetActionHUDVisible(bool bShow)
        {
            if (ActionSlider)
            {
                ActionSlider.gameObject.SetActive(bShow);
            }
        }

        public void SetHUDVisible(bool bShow)
        {
            if (gameObject.activeSelf == bShow)
            {
                return;
            }
            gameObject.SetActive(bShow);
        }

        public void ChangeActionBar(float value)
        {
            if (ActionSlider)
            {
                ActionSlider.value = value;
            }
        }

        public void ChangeBlood(float value)
        {
            if (curHP <= 0 && value >= MaxHp)
            {
                OnReborn();
                curHP = MaxHp;
            }
            
            curHP = Mathf.Min(curHP + value , MaxHp);
            
            if (curHP <= 0)
            {
                OnBloodEmpty();
            }
            HpSlider.value = (float)curHP / MaxHp;
        }

        public void OnBloodEmpty()
        {
            _character.IsDead = true;
        }

        public void OnReborn()
        {
            HpSlider.value = 1;
            _character.IsDead = false;
        }

        public void SetSkillFlagVisible(bool bShow)
        {
            SkillFlag.SetActive(bShow);
        }
    }
}