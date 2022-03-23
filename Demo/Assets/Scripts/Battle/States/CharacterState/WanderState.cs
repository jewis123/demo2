using UnityEngine;

namespace Battle.States
{
	public class WanderState: ChargeableState
	{
		private static readonly int SpeedHash = Animator.StringToHash("speed");
		private float rangeTime;
		private float enterTm;
		
		public override void EnterState()
		{
			base.EnterState();
			fsm.target.agent.speed = fsm.target.data.speed / 2;
			
			RefeshTime();
			GetWanderPos();
			fsm.target.animator.CrossFade("walking",0.2f,0);
		}

		public override void ExitState()
		{
			
		}

		public override void UpdateState()
		{
			base.UpdateState();
			if (fsm.target.agent.remainingDistance  <= fsm.target.agent.stoppingDistance)
			{
				fsm.target.animator.CrossFade("idle",0.2f,0);
			}
			
			enterTm += Time.deltaTime;

			if (fsm.target.team.IsMyTeam)
			{
				if (rangeTime < enterTm)
				{
					RefeshTime();
					GetWanderPos();
				}
			}
		}

		private void RefeshTime()
		{
			enterTm = Time.time;
			Random.InitState((int) (fsm.target.data.id * Time.time));
			rangeTime = enterTm + Random.Range(fsm.target.WanderTime.x * 100,fsm.target.WanderTime.y * 100)/100;
		}
		
		private void GetWanderPos()
        {
	        float x = Random.Range(-fsm.target.data.wanderRadius*100, fsm.target.data.wanderRadius*100)/100;
	        float z = Random.Range(-fsm.target.data.wanderRadius*100, fsm.target.data.wanderRadius*100)/100;

	        Vector3 tar = fsm.target.StandPos + new Vector3(x, fsm.target.transform.position.y, z);
	        fsm.target.animator.CrossFade("walking",0.2f,0);
	        fsm.target.SetDestination(tar);
        }
	}
}