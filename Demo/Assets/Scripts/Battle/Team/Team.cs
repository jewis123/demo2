using System;
using System.Collections.Generic;
using Battle.Config;
using Cinemachine;
using SuperCLine.ActionEngine;
using UnityEngine;
using Quaternion = UnityEngine.Quaternion;
using Random = UnityEngine.Random;
using Vector3 = UnityEngine.Vector3;

namespace Battle
{
    public class Team
    {
        private bool isMyTeam;
        private float teamSpeed;
        private float backwardDistance;
        private float offsetRadius;
        private float rotateSpeed;
        private Vector3 centerPos;
        private int _memberCnt;
        private float teamRadius;
        private int teamIdx;
        private int colCount;
        private float gapSize;
        private List<int> lineup;

        private Vector3 curVelocity;
        private float curRotation;

        private GameObject teamCenterObj;
        private CinemachineTargetGroup _targetGroup;
        private GameObject teamHolder;
        private Vector3 lastInputDir;
        private GameObject[] memberPosObj;
        private Mesh[] _meshes;
        private bool dontChangePoses;
        private GameObject Pointer;
        private GameObject Effect;
        private GameObject FollowPoint;
        private CameraTeamLook teamCameraLook;
        private CameraController camController;
        private int deadCnt;
        private bool moving;
        private Quaternion teamRotation;
        private bool inited = false;

        public Quaternion TeamRotation
        {
            get => teamRotation;
            set
            {
                teamRotation = value;
                if (Pointer != null)
                {
                    Pointer.transform.rotation = teamRotation;
                }
            }
        }

        public Vector3[] memberPoses;
        public BattleGamePlay battle;
        public List<BattleCharacter> Characters = new List<BattleCharacter>();
        

        public CinemachineTargetGroup TargetGroup
        {
            get => _targetGroup;
        }
        public bool IsMyTeam
        {
            get => isMyTeam;
        }
        public float TeamSpeed => teamSpeed;
        public bool InputTrigger
        {
            get { return curVelocity != Vector3.zero; }
        }
        public float BackwardDistance => backwardDistance;
        public int DeadCnt
        {
            get => deadCnt;
            set
            {
                deadCnt = Mathf.Max(0, deadCnt);
            }

        }
        public int MemberCnt
        {
            get => _memberCnt;
            set => _memberCnt = value;
        }
        public Vector3 TeamPos
        {
            get => centerPos;
            set
            {
                centerPos = value;
                if (!isMyTeam)
                {
                    RefreshCircleTeam();
                }
            }
        }
        
        public Team(Vector3 teamPos, int teamCnt, bool isMyTeam, int idx, BattleGamePlay battle)
        {

            this.battle = battle;
            centerPos = teamPos;
            _memberCnt = teamCnt;
            teamIdx = idx;
            this.isMyTeam = isMyTeam;
        }

        public void InitMyTeamProps()
        {
            backwardDistance = battle.battleData.teamDistance;
            offsetRadius = battle.battleData.teamPosOffsetRadius;
            teamRadius = battle.battleData.myTeamRadius;
            teamSpeed = battle.battleData.myTeamMoveSpeed;
            rotateSpeed = battle.battleData.myTeamRotateSpeed;
            
            InitTeamPos(true);
            SetCenterEffectParent();
        }

        public void InitEnemryTeamProps(float dataRadis, float dataSpeed, float dataRatateSpeed)
        {
            teamRadius = dataRadis;
            teamSpeed = dataSpeed;
            rotateSpeed = dataRatateSpeed;
            
            InitTeamPos(false);
        }

        private void InitTeamPos(bool withLineup)
        {
            teamHolder = new GameObject($"TeamCenter_{teamIdx}");
            teamHolder.transform.position = centerPos;
            TeamRotation = Quaternion.Euler(new Vector3(0,90,0));

            teamCenterObj = new GameObject($"MoveableTeamCenter_{teamIdx}");
            teamCenterObj.transform.position = centerPos;

            if (_memberCnt > 0)
            {
                memberPoses = new Vector3[_memberCnt];
                memberPosObj = new GameObject[_memberCnt];
                _meshes = new Mesh[_memberCnt];
                for (int i = 0; i < _memberCnt; i++)
                {
                    var newObj = new GameObject($"team_pos_{i}");
                    memberPosObj[i] = newObj;
                    memberPosObj[i].transform.SetParent(teamCenterObj.transform);
                }
            }

            if (isMyTeam)
            {
                GameObject prefab;
                prefab = Resources.Load<GameObject>("Prefabs/ArrayIndicator");
                Pointer = GameObject.Instantiate(prefab);
                prefab = Resources.Load<GameObject>("Prefabs/MissonPoint");
                Effect = GameObject.Instantiate(prefab);
                SetCenterEffectParent();
            }

            if (withLineup)
            {
                gapSize = battle.config.gapSize;
                colCount = battle.config.colCount;
                lineup = battle.config.characters;
                WithLineup(gapSize, colCount);
            }
            else
            {
                Withoutlineup();
            }
            
            inited = true;
            InitFollowCam();
        }

        private void InitFollowCam()
        {
            if (isMyTeam)
            {
                if (_targetGroup == null)
                {
                    FollowPoint = new GameObject("FollowPoint");
                    _targetGroup = FollowPoint.AddComponent<CinemachineTargetGroup>();
                }
                
                
                var teamCamera = GameObject.Find("TeamCamera");
                if (teamCamera != null)
                {
                    teamCameraLook = teamCamera.GetComponent<CameraTeamLook>();
                    camController = teamCamera.GetComponent<CameraController>();
                    teamCamera.GetComponent<CinemachineVirtualCamera>().Follow = TargetGroup.transform;
                }
                else
                {
                    Debug.Log("缺少队伍相机");
                    // 手动创建过程省略。 。。 
                }

                if (teamCameraLook != null)
                {
                    teamCameraLook.lookTarget = TargetGroup.transform;
                }

                if (camController!= null)
                {
                    camController.target = TargetGroup.transform;
                }
            }
        }

        public void Withoutlineup()
        {
            RefreshCircleTeam();
        }

        public void WithLineup(float gapSize, int colCount)
        {
            RefreshLineupTeam(gapSize, colCount);
        }

        public bool IsAllDead()
        {
            return DeadCnt >= _memberCnt;
        }
        
        public void RefreshCircleTeam()
        {
            float perDegree = (float)360 / _memberCnt;

            // if (!battle.useTeamRotationOnMove)
            // {
            //     
            // }
            for (int i = 0; i < _memberCnt; i++)
            {
                memberPoses[i] = Quaternion.Euler(0,  perDegree * i ,0) * Vector3.forward * teamRadius  + centerPos;
                memberPosObj[i].transform.position = memberPoses[i];
            }
            // else
            // {
            //     for (int i = 0; i < _memberCnt; i++)
            //     {
            //         memberPoses[i] = Quaternion.Euler(0,  perDegree * i + curRotation ,0) * Vector3.forward * teamRadius  + centerPos;
            //         memberPosObj[i].transform.position = memberPoses[i];
            //     }
            // }
        }

        public void RefreshLineupTeam(float gapSize, int colCount)
        {
            var midIdx = Mathf.Ceil(colCount / 2);

            for (int i = 0; i < memberPosObj.Length; i++)
            {
                var offset = new Vector3((i % colCount) - midIdx, 0, Mathf.Floor(i / colCount) - midIdx) * (2*gapSize);
                var teamPos = centerPos + offset;
                // if (battle.useRoatateSpeed)
                // {
                //     float rotateX = teamPos.x * Mathf.Cos(curRotation) - teamPos.y * Mathf.Sin(curRotation);
                //     float rotateZ = teamPos.x * Mathf.Sin(curRotation) + teamPos.y * Mathf.Cos(curRotation);
                //     teamPos = new Vector3(rotateX, 0, rotateZ);
                // }

                memberPosObj[i].transform.position = teamPos;
                memberPoses[i] = teamPos;
            }
        }

        public void StandIdxWithoutLineup()
        {
            int idx = 0;
            for (int i = 0; i < Characters.Count; i++)
            {
                if (Characters[i].IsDead)
                {
                    continue;
                }

                idx++;
                Characters[i].StandIndex = idx;
            }
        }

        private void StandIdxWithLineup(BattleCharacter character)
        {
            for (int j = 0; j < lineup.Count; j++)
            {
                if (character.data.id == lineup[j])
                {
                    character.StandIndex = j;
                    break;
                }
            }
        }

        public void SetParent(Transform parent)
        {
            teamHolder.transform.SetParent(parent);
        }

        public void SetCenterEffectParent()
        {
            if (!inited)
            {
                return;
            }
            GameObject parent;
            if (battle.useTeamCenterMove)
            {
                parent = teamCenterObj;
            }
            else
            {
                parent = memberPosObj[0];
            }
            
            Pointer.transform.SetParent(parent.transform);
            Pointer.transform.localPosition = Vector3.zero;
            Pointer.transform.localScale = Vector3.one * 0.2f;
            
            Effect.transform.SetParent(parent.transform);
            Effect.transform.localPosition = Vector3.zero;
        }

        public void AddMember(BattleCharacter character)
        {
            character.transform.SetParent(teamHolder.transform);
            Characters.Add(character);
            RefreshFollowTarget();
            if (isMyTeam)
            {
                StandIdxWithLineup(character);
            }
        }

        public void RefreshFollowTarget()
        {
            if (!isMyTeam)
            {
                return;
            }
            List<CinemachineTargetGroup.Target> list = new List<CinemachineTargetGroup.Target>();
            list.Add(new CinemachineTargetGroup.Target(){target = teamCenterObj.transform, weight = 1f, radius = 0});
            for (int i = 0; i < Characters.Count; i++)
            {
                if (!Characters[i].IsDead)
                {
                    list.Add(new CinemachineTargetGroup.Target(){target = Characters[i].transform, weight = 1f, radius = 0});
                }
            }

            _targetGroup.m_Targets = list.ToArray();
        }

        public void OnMove(Vector2 moveInput)
        {
            int idleCnt = 0;
            for (int i = 0; i < Characters.Count; i++)
            {
                if (Characters[i].IsIdle)
                {
                    idleCnt++;
                }
            }

            if (idleCnt == 0)
            {
                curVelocity = Vector3.zero;
                return;
            }
            
            if (moveInput == Vector2.zero)
            {
                curVelocity = Vector3.zero;
                return;
            }
            
            
            Vector3 newInputDir = new Vector3(moveInput.x, 0.0f, moveInput.y);
            float angle = Vector3.Angle(newInputDir, lastInputDir);
            if (angle > 160)
            {
                ReverseTeamPoses();
            }
            
            lastInputDir = newInputDir.normalized;
            var newRotation = Mathf.Atan2(lastInputDir.x, lastInputDir.z)* Mathf.Rad2Deg ;
            curRotation = newRotation;
            curVelocity = lastInputDir * teamSpeed * Time.deltaTime * moveInput.magnitude;

            for (int i = 0; i < Characters.Count; i++)
            {
                Characters[i].OnMove();
            }

            moving = true;
        }

        public void Update()
        {
            if (!inited)
            {
                return;
            }
            
            DrawDebugGizmos();

            if (curVelocity == Vector3.zero)
            {
                if (battle.useImmdiateStop)
                {
                    StopAllMember();
                }

                if (moving)
                {
                    RamdomOffsetStopPos();
                    moving = false;
                }
                
                return;
            }
            
            UpdateTeam();

            UpdateTeamPoses();
            
            ActiveMembers();
        }

        private void ReverseTeamPoses()
        {
            if (!isMyTeam)
            {
                return;
            }
            var rowCnt = (int)Mathf.Ceil(memberPosObj.Length / colCount);
            for (int i = 0; i < rowCnt; i++)
            {
                for (int j = 0; j < colCount; j++)
                {
                    var temp = memberPosObj[i*colCount + j].transform.position;
                    memberPosObj[i * colCount + j].transform.position = memberPosObj[(rowCnt - 1 - i) * colCount + j].transform.position;
                    memberPosObj[(rowCnt - 1 - i) * colCount + j].transform.position = temp;
                }
            }
        }

        private void UpdateTeam()
        {
            if (battle.useRoatateSpeed)
            {
                TeamRotation = Quaternion.RotateTowards(TeamRotation, 
                    Quaternion.Euler(0.0f, curRotation, 0.0f), rotateSpeed);
            }
            else
            {
                TeamRotation = Quaternion.Euler(0.0f, curRotation, 0.0f);
            }

            var position = teamCenterObj.transform.position;
            centerPos = Vector3.MoveTowards(position, 
                position + curVelocity, Time.deltaTime * teamSpeed);
            
            position = centerPos;
            teamCenterObj.transform.position = position;
        }

        private void UpdateTeamPoses()
        {
            if (moving)
            {
                if (isMyTeam)
                {
                    RefreshLineupTeam(gapSize, colCount);
                }
                else
                {
                    RefreshCircleTeam();
                }
            }

        }

        private void DrawDebugGizmos()
        {
            if (!isMyTeam)
            {
                return;
            }
            
            UDrawTool tool;
            for (int i = 0; i < memberPosObj.Length; i++)
            {
                if (!memberPosObj[i].TryGetComponent(out tool))
                {
                    tool = memberPosObj[i].AddComponent<UDrawTool>();
                }
                tool?.DrawCircleSolid(memberPosObj[i].transform, memberPoses[i], 0.3f, Color.magenta, ref _meshes[i]);
                tool.DrawCircle(memberPosObj[i].transform, memberPosObj[i].transform.position, offsetRadius, Color.magenta);
            }
        }

        public void RamdomOffsetStopPos()
        {
            if (isMyTeam)
            {
                for (int i = 0; i < memberPoses.Length; i++)
                {
                    Vector3 truePos = memberPosObj[i].transform.position;
                    memberPoses[i] = truePos + RadiusOffset(i);;
                }
            }
        }
        
        private Vector3 RadiusOffset(int seed)
        {
            Random.InitState((int) (seed * Time.time));
            float x = Random.Range((lastInputDir.x < 0 ? -1 : 0) * offsetRadius * 100 , (lastInputDir.x > 0 ? 1 : 0) * offsetRadius*100)/100;
            float z = Random.Range((lastInputDir.z < 0 ? -1 : 0) *offsetRadius *100, (lastInputDir.z > 0 ? 1 : 0)*offsetRadius*100)/100;
            return new Vector3(x, 0, z);
        }
        
        public void OnDestroy()
        {
            GameObject.Destroy(_targetGroup);
            GameObject.Destroy(teamCenterObj);
        }

        private void StopAllMember()
        {
            if (!battle.useImmdiateStop)
            {
                return;
            }
            for (int i = 0; i < Characters.Count; i++)
            {
                Characters[i].ForceStop();
            }
        }

        private void ActiveMembers()
        {
            for (int i = 0; i < Characters.Count; i++)
            {
                Characters[i].ForceStoped = false;
            }
        }

        public void SetStopMode()
        {
            if (battle.useImmdiateStop)
            {
                StopAllMember();
            }
            else
            {
                ActiveMembers();
            }
        }

        public int GetTeamIdxByID(int id)
        {
            if (!isMyTeam)
            {
                return -1;
            }
            for (int i = 0; i < lineup.Count; i++)
            {
                if (lineup[i] == id)
                {
                    return i;
                }
            }

            return -1;
        }
    }
}