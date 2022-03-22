using System.Collections.Generic;
using Battle.Config;
using Cinemachine;
using SuperCLine.ActionEngine;
using UnityEngine;
using Quaternion = UnityEngine.Quaternion;
using Vector3 = UnityEngine.Vector3;

namespace Battle
{
    public class Team
    {
        private bool isMyTeam;
        private float moveSpeed;
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
        private int deadCnt;
        private bool moving;

        public Quaternion teamRotation;
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
        
        public Vector2 Velocity
        {
            get => curVelocity;
        }
        
        public Team(Vector3 teamPos, int teamCnt, float radius, float speed,float rotateSpeed, bool isMyTeam, int idx, BattleGamePlay battle,float offsetRadius=0)
        {

            this.offsetRadius = offsetRadius;
            this.battle = battle;
            this.rotateSpeed = rotateSpeed;
            centerPos = teamPos;
            _memberCnt = teamCnt;
            teamRadius = radius;
            teamIdx = idx;
            moveSpeed = speed;
            this.isMyTeam = isMyTeam;
        }

        public void InitTeamPos(bool withLineup, CharacterTeamPosConfig config)
        {
            teamHolder = new GameObject($"TeamCenter_{teamIdx}");
            teamHolder.transform.position = centerPos;
            
            if (_targetGroup == null && isMyTeam)
            {
                FollowPoint = new GameObject("FollowPoint");
                _targetGroup = FollowPoint.AddComponent<CinemachineTargetGroup>();
            }
            
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
                gapSize = config.gapSize;
                colCount = config.colCount;
                lineup = config.characters;
                WithLineup(gapSize, colCount);
            }
            else
            {
                Withoutlineup();
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
            
            lastInputDir = new Vector3(moveInput.x, 0.0f, moveInput.y).normalized;
            var newRotation = Mathf.Atan2(lastInputDir.x, lastInputDir.z)* Mathf.Rad2Deg ;
            curRotation = newRotation;
            curVelocity = lastInputDir * moveSpeed * Time.deltaTime * moveInput.magnitude;

            for (int i = 0; i < Characters.Count; i++)
            {
                Characters[i].OnMove();
            }

            moving = true;
        }

        public void Update()
        {
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

                BrakingAll();
                return;
            }

            UpdateTeamCenter();
            
            ActiveMembers();
            
            UpdateTeamPoses();

        }

        private void UpdateTeamCenter()
        {
            if (battle.useRoatateSpeed)
            {
                teamCenterObj.transform.rotation = Quaternion.RotateTowards(teamCenterObj.transform.rotation, Quaternion.Euler(0.0f, curRotation, 0.0f), rotateSpeed);
            }
            else
            {
                teamCenterObj.transform.rotation = Quaternion.Euler(0.0f, curRotation, 0.0f);
            }
            teamCenterObj.transform.position = Vector3.MoveTowards(teamCenterObj.transform.position, teamCenterObj.transform.position + curVelocity, Time.deltaTime * moveSpeed);
            centerPos = teamCenterObj.transform.position;
            teamRotation = teamCenterObj.transform.rotation;
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

        private void BrakingAll()
        {
            for (int i = 0; i < Characters.Count; i++)
            {
                Characters[i].Braking = true;
            }
        }

        private void ActiveMembers()
        {
            for (int i = 0; i < Characters.Count; i++)
            {
                Characters[i].ForceStoped = false;
                Characters[i].Braking = false;
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
    }
}