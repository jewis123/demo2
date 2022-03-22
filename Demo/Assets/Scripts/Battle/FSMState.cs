namespace Battle
{
    public abstract class FSMState<T>
    {
        //状态控制机
        protected FSM<T> fsm { get; private set; }
        

        public void Init(FSM<T> fstm)
        {
            fsm = fstm;
        }

        //进入状态方法
        public abstract void EnterState();
        //离开状态方法
        public abstract void ExitState();
        //更新状态方法
        public abstract void UpdateState();
    }
}