using Cinemachine;
using UnityEngine;

public class FramingTarget2:CinemachineFramingTransposer
{
        public bool ConstraintX;
        public bool ConstraintY;
        public bool ConstraintZ;
        public override void MutateCameraState(ref CameraState curState, float deltaTime)
        {
                float x = curState.RawPosition.x;
                float y = curState.RawPosition.y;
                float z = curState.RawPosition.z;
                
                base.MutateCameraState(ref curState, deltaTime);

                if (ConstraintX)
                {
                        curState.RawPosition = new Vector3(x, curState.RawPosition.y , curState.RawPosition.z);
                }
                if (ConstraintY)
                {
                        curState.RawPosition = new Vector3(curState.RawPosition.x, y , curState.RawPosition.z);
                }
                if (ConstraintZ)
                {
                        curState.RawPosition = new Vector3(curState.RawPosition.x, curState.RawPosition.y ,z);
                }
        }
}