using SuperCLine.ActionEngine.Editor;

namespace SuperCLine.ActionEngine
{
    public class PhysicalEnityWrapper
    {
        public PhysicalEntity AttackEntity;

        public void SetAnimator(EMotionAnimatorType motionAnimatorType, MotionAnimatorProperty property, AttackHit AH)
        {
            MotionAnimator Animator = null;

            switch (motionAnimatorType)
            {
                case EMotionAnimatorType.EMAT_Line:
                    Animator = new LineAnimator(AH, AttackEntity, property as LineAnimatorProperty);
                    break;
                case EMotionAnimatorType.EMAT_Curve:
                    Animator = new CurveAnimator(AH, AttackEntity, property as CurveAnimatorProperty);
                    break;
                case EMotionAnimatorType.EMAT_PingPong:
                    Animator = new PingPongAnimator(AH, AttackEntity, property as PingPongAnimatorProperty);
                    break;
            }

            AttackEntity.Animator = Animator;
        }

        public void Update(float fTick)
        {
            AttackEntity.Update(fTick);
            AttackEntity.FixedUpdate(fTick);
        }

        public void Dispose()
        {
            AttackEntity.Dispose();
        }
    }
}