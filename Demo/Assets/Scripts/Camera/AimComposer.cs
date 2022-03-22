
using Cinemachine;
using UnityEngine;

public class AimComposer:CinemachineComposer
{
        public bool ConstraintX;
        public bool ConstraintY;
        public bool ConstraintZ;
        public Vector2 xRotationRange;
        public Vector2 yRotationRange;
        public Vector2 zRotationRange;
        
        public override void MutateCameraState(ref CameraState curState, float deltaTime)
        {
                base.MutateCameraState(ref curState, deltaTime);

                if (ConstraintX)
                {
                        float x = curState.RawOrientation.eulerAngles.x;
                        x = x < xRotationRange.x ? xRotationRange.x : x;
                        x = x > xRotationRange.y ? xRotationRange.x : x;
                        Vector3 angles = new Vector3(x, curState.RawOrientation.eulerAngles.y,curState.RawOrientation.eulerAngles.z);
                        curState.RawOrientation = Quaternion.Euler(angles);
                }
                if (ConstraintY)
                {
                        float y = curState.RawOrientation.eulerAngles.x;
                        y = y < yRotationRange.x ? yRotationRange.x : y;
                        y = y > yRotationRange.y ? yRotationRange.x : y;
                        Vector3 angles = new Vector3(curState.RawOrientation.eulerAngles.x, y,curState.RawOrientation.eulerAngles.z);
                        curState.RawOrientation = Quaternion.Euler(angles);
                }
                if (ConstraintZ)
                {
                        float z = curState.RawOrientation.eulerAngles.x;
                        z = z < zRotationRange.x ? zRotationRange.x : z;
                        z = z > zRotationRange.y ? zRotationRange.x : z;
                        Vector3 angles = new Vector3(curState.RawOrientation.eulerAngles.x, curState.RawOrientation.eulerAngles.y,z);
                        curState.RawOrientation = Quaternion.Euler(angles);
                }
        }
        
}