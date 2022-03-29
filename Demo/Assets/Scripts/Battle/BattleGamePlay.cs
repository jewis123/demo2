using System;
using System.Collections.Generic;
using Battle.Config;
using Battle.States.BattleState;
using Scene.SceneControllers;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Battle
{
    [RequireComponent(typeof(InputHandler))]
    public class BattleGamePlay : MonoBehaviour
    {
        public TeamManager teamManager;

        [SerializeField]public BattleDataConfig battleData;
        [SerializeField] public CharacterTeamPosConfig config;
        [HideInInspector]public InputHandler Input;

        public Dictionary<int, BattleCharacter> characters { get; private set; }
        public List<BattleCharacter> characterList { get; private set; }

        public FSM<BattleGamePlay> fsm;

        public ActionQueue actionqueue;

        public int battleSpeed = 1;
        [Tooltip("队伍朝向使用增量变化")]
        public bool useRoatateSpeed;
        [Tooltip("队伍中心为原点/队伍一号位为原点")]
        public bool useTeamCenterMove;
        [Tooltip("无输入立即停止")]
        public bool useImmdiateStop;

        public bool drawTeamGizmose;

        // [Tooltip("使用后队变前队")]
        // public bool useAutoChangeLeader;
        private bool curCenterMode;
        private bool curStopMode;

        public Action OnGameplayInited;
        public Action OnGameOver;
        private bool isGameOver;
        public ActorAdapter ActorAdapter;

        public bool IsGameOver
        {
            get => isGameOver;
            set => isGameOver = value;
        }

        private void Awake()
        {
            actionqueue = new ActionQueue(this);
            ActorAdapter = new ActorAdapter(this);

            if (FindObjectsOfType<BattleGamePlay>().Length > 1)
            {
                Destroy(gameObject);
            }
            
            if (GameCore.singleton == null)
            {
                OnGamePlayStart();
            }
        }


        private void Update()
        {
            fsm?.UpdateState();
            teamManager?.Update();
            // if (curCenterMode != useTeamCenterMove)
            // {
            //     curCenterMode = useTeamCenterMove;
            //     teamManager?.GetMyTeam().SetCenterEffectParent();
            // }

            if (curStopMode != useImmdiateStop)
            {
                curStopMode = useImmdiateStop;
                teamManager?.GetMyTeam().SetStopMode(curStopMode);
            }
        }

        private void OnDestroy()
        {
            if (GameCore.singleton == null)
                OnGamePlayEnd();
        }

        public void Init(BattleDataConfig data)
        {
            battleData = data;
            var playerInput = gameObject.AddComponent<PlayerInput>();
            playerInput.actions = Resources.Load<InputActionAsset>("BattleInput");
            playerInput.currentActionMap = playerInput.actions.actionMaps[0];
        }
        
        public void OnStart()
        {

            fsm = new FSM<BattleGamePlay>(this);
            fsm.Register<BattleBeginState>();
            fsm.Register<BattleEndState>();
            fsm.SetDefault<BattleBeginState>();
            
            EnterMap(() =>
            {
                OnGamePlayStart();
            });

        }

        public void OnExit()
        {
            ExitMap(() =>
            {
                OnGamePlayEnd();
                GameCore.singleton.gameSceneManager.LoadScene<ExampleScene>();
            });
        }


        private void OnGamePlayStart()
        {
            ResourcesLoadHelper.Load<CharacterTeamPosConfig>(this, "ConfigObjs/CharacterTeamPosConfig", characterAssetConfig =>
            {
                config = characterAssetConfig;
                gameObject.SetActive(true);
                CreateCharacters();
            
                //todo otherLogic
                Input = GetComponent<InputHandler>();
                Input.gamePlay = this;
                OnGameplayInited?.Invoke();
            });
        }

        private void OnGamePlayEnd()
        {
            //todo otherLogic
            teamManager.OnDestroy();
        }

        public void CheckGameOver(int team)
        {
            if (teamManager.IsEnermyAllDead() || teamManager.IsMyTeamAllDead())
            {
                IsGameOver = true;
                if (fsm != null)
                {
                    fsm.ChangeState<BattleEndState>();

                }
            }
        }

        private void EnterMap(System.Action callback)
        {
            GameCore.singleton.gameSceneManager.LoadScene<BattleScene>(callback);
        }

        private void ExitMap(System.Action callback)
        {
            GameCore.singleton.gameSceneManager.UnLoadScene<BattleScene>(callback);
        }

        private void CreateCharacters()
        {
            teamManager = new TeamManager(this);

            characters = new Dictionary<int, BattleCharacter>();
            characterList = new List<BattleCharacter>();
            var characterRoot = GameObject.Find("Characters");
            if (characterRoot == null)
            {
                characterRoot = new GameObject("Characters");
            }

            //加入配置角色
            Team team;
            team = teamManager.CreateTeam(Vector3.zero, config.characters.Count, true );
            team.InitMyTeamProps();
            team.SetParent(characterRoot.transform);
            
            
            for ( int i = 0; i < battleData.characterData.Length; i++)
            {
                var Char = battleData.characterData[i];
                int teamIdx = team.GetTeamIdxByID(Char.id);
                LoadCharacter( teamIdx, team, Char);
            }

            //创建怪物
            TeamData teamData;
            for (int i = 0; i < battleData.enermyTeamData.Length; i++)
            {
                teamData = battleData.enermyTeamData[i];
                team = teamManager.CreateTeam(teamData.teamPositions, teamData.charaters.Length, false);
                team.InitEnemryTeamProps(teamData.teamRadius, teamData.moveSpeed, teamData.teamRotateSpeed);
                team.SetParent(characterRoot.transform);

                for (int j = 0; j < teamData.charaters.Length; j++)
                {
                    LoadCharacter( j , team,  teamData.charaters[j]);
                }
            }
        }

        private void LoadCharacter( int teamIdx, Team team, CharacterData data)
        {
            if (!characters.ContainsKey(data.id))
            {
                BattleCharacter character = null;

                if (team.IsMyTeam)
                {
                    ActorAdapter.CreateCharacterUnit(team.memberPoses[teamIdx], ref data, ref character);
                }
                else
                {
                    ActorAdapter.CreateMonsterUnit(team.memberPoses[teamIdx], ref data, ref character);
                }
                SetData(character, teamIdx, team, data);
                team.AddMember(character);
            }
        }

        private void SetData(BattleCharacter character, int teamIdx, Team team, CharacterData data)
        {
            character.team = team;
            character.data = data;
            character.battle = this;
            character.battleActionQueue = actionqueue;
            character.StandIndex = teamIdx;
            character.inited = true;
            character.WanderTime = battleData.startWanderTime;
            character.transform.position = character.StandPos;
            
            characters.Add(data.id, character);
            characterList.Add(character);
        }
    }
}