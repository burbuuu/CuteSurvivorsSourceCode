namespace Managers.StateMachine
{
    public interface IState
    {
        void Enter();
        void Exit();
    }
}