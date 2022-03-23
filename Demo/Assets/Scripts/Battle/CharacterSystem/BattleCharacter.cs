using Battle.States;
using Battle.States.SubSkillState;
using SuperCLine.ActionEngine;
using UI.HUD;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

namespace Battle
{
    public class BattleCharacter : MonoBehaviour
    {
        public bool showGizmose;
        [SerializeField] public CharacterData data;
        [HideInInspector] public BattleGamePlay battle;
        [HideInInspector] public bool isIronBody;
        [HideInInspector] public int ComboIndex;
        [HideInInspector] public bool bHitBack;
        [HideInInspector] public HudBase hud;
        [HideInInspector] public NavMeshAgent agent;
        [HideInInspector] public bool inited;
        [HideInInspector] public Vector2 WanderTime;
        public Team team;
        public ActionQueue battleActionQueue;
        public Unit Actor;

        
        private Cinemachine.CinemachineImpulseSource MyInpulse;
        private CharacterDebugGizmos debugGizmos;
        private bool isDead;
        
        public bool AttackTriggered { get; set; }
        public bool InputTrigger { get; set; }
        public bool ForceStoped { get; set; }
        public bool IsIdle{ get; set; }
        public Vector3 StandPos
        {
            get => team.memberPoses[StandIndex];
        }

        private int standIndex;
        public int StandIndex
        {
            get => standIndex;
            set => standIndex = value;
        }

        public bool IsDead
        {
            get => isDead;
            set
            {
                isDead = value;
                if (isDead)
                {
                    agent.enabled = false;
                    fsm.ChangeState<DieState>();
                    team.MemberCnt = team.MemberCnt -1;
                    if (team.IsMyTeam)
                    {
                        team.RefreshFollowTarget();
                    }
                    else
                    {
                        team.RefreshCircleTeam();
                        team.StandIdxWithoutLineup();
                    }

                    team.DeadCnt++;
                }
                else
                {
                    fsm.ChangeState<RebornState>();
                    team.DeadCnt--;
                }
            }
        }
        
        public int EnergyCount()
        {
            return hud.EnergyCount;
        }

        public bool SetEnergy(int changeCnt)
        {
            if (data.team == 0)
            {
                var inValid = (hud.EnergyCount == 3 && changeCnt > 0) ||
                            (hud.EnergyCount <= 0 && changeCnt < 0);
                if (inValid)
                {
                    return false;
                }
            }

            if (data.team == 1)
            {
				var inValid = (hud.EnergyCount == 1 && changeCnt > 0) ||
                            (hud.EnergyCount <= 0 && changeCnt < 0);
				if(inValid)
					return false;
            }

            hud.EnergyCount += changeCnt;
            
            
            return true;
        }

        
        public Animator animator{ get; private set; }
        
        private FSM<BattleCharacter> fsm;


        private void OnDrawGizmos()
        {
            Gizmos.DrawLine(transform.position, agent.destination);
            Gizmos.DrawLine(transform.position, transform.position + agent.velocity);
        }

        private void Start()
        {
            debugGizmos = GetComponentInChildren<CharacterDebugGizmos>();
            
            hud = GetComponentInChildren<HudBase>();
            hud.InitData(data.HP, data.AP, this);
           
            animator = gameObject.GetComponent<Animator>();
            
            agent = gameObject.GetComponent<NavMeshAgent>();
            agent.speed = data.speed;
            agent.acceleration = team.IsMyTeam ? data.accelerateSpeed : 999;
            
            ComboIndex = 1;

            MyInpulse = GetComponent<Cinemachine.CinemachineImpulseSource>();

            fsm = new FSM<BattleCharacter>(this);
            fsm.Register<FollowState>();
            fsm.Register<WalkTowardState>();
            fsm.Register<WaitComboState>();
            fsm.Register<WanderState>();
            
            fsm.Register<SkillState>();
            fsm.Register<PreSkillState>();
            fsm.Register<ExecuteSkillState>();
            fsm.Register<AfterSkillState>();
            
            fsm.Register<BehitState>();
            fsm.Register<ChangeStationState>();
            fsm.Register<DieState>();
            fsm.Register<RebornState>();
            fsm.Register<IdleState>();
            fsm.Register<Back2TeamState>();
            fsm.SetDefault<IdleState>();
        }

        public void BeHit(GameObject attacker)
        {
            if (IsDead)
            {
                return;
            }


            AttackTriggered = false;

            BattleCharacter character = attacker.GetComponent<BattleCharacter>();
            float damageValue = CalDamage(character);
            FontJump(damageValue);

            MyInpulse.GenerateImpulse();

            
            if (IsIronBody())
            {
                return;
            }

            if(fsm.currentstateName!="BehitState")
                fsm.SetString("beforeBeHitState",fsm.currentstateName);
            
            if(fsm.currentstateName=="PreSkillState" || fsm.currentstateName=="WaitComboState")
                fsm.SetString("beforeBeHitState","Back2TeamState");
            
            fsm.ChangeState<BehitState>();
            
        }

        public void FontJump(float val)
        {
            var txt = Resources.Load("UI/Huds/DmgText");
            GameObject dmgText = (GameObject)Instantiate(txt, transform.Find("HUD"));
            Text text = dmgText.GetComponent<Text>();
            text.color = val <= 0 ? Color.red : Color.green;
            text.text = $"{val}";
            GameObject.Destroy(dmgText.gameObject, 0.5f);
            hud.ChangeBlood(val);
        }

        /// <summary>
        /// 受击前检测时候被击中
        /// </summary>
        /// <returns></returns>
        private bool IsIronBody()
        {
            return isIronBody || fsm.currentstateName == "ChangeStationState" || fsm.currentstateName == "DieState" || fsm.currentstateName == "RebornState";
        }

        public void ChangeStation()
        {
            StandIndex += 1;
            fsm.ChangeState<ChangeStationState>();
        }

        public void ForceStop()
        {
            if (ForceStoped)
            {
                return;
            }
            ForceStoped = true;
            agent.isStopped = true;
            agent.ResetPath();
            fsm.ChangeState<IdleState>();
        }

        public float GetSKillDamage()
        {
            return data.carrySkill.skillDamage + data.attribute.damage;
        }
        
        private float CalDamage(BattleCharacter attacker)
        {
            return data.attribute.defence - attacker.GetSKillDamage();
        }

        public void OnMove()
        {
            fsm.ChangeState<FollowState>();
        }
        
        private void Update()
        {
            if (!inited)
            {
                return;
            }

            if (showGizmose != debugGizmos.gameObject.activeSelf)
            {
                debugGizmos.gameObject.SetActive(showGizmose);
            }
            debugGizmos.Init(data.attackRadius, data.wanderRadius, data.searchRadius, this);

            fsm?.UpdateState();
            SetupActor();
        }

        public void SetupActor()
        {
            Actor.IsDead = isDead;
            Actor.IsGod = IsIronBody();
            Actor.TeamIdx = data.team;
            
            BattleUnit unit = Actor as BattleUnit;
            unit.SetBattleProperties(data.searchRadius,data.attackRadius,data.chaseRadius);
        }

        public void SetDestination(Vector3 dest)
        {
            if (agent.destination != dest)
            {
                agent.SetDestination(dest);
            }
        }
    }
}