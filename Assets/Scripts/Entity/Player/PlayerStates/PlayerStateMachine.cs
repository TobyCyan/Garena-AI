namespace Player.PlayerStates
{
    public sealed class PlayerStateMachine
    {
        public PlayerState CurrentState { get; private set; }

        public void Initialize(PlayerState startingState)
        {
            CurrentState = startingState;
            CurrentState?.Enter();
        }

        public void ChangeState(PlayerState nextState)
        {
            if (nextState == null || nextState == CurrentState)
            {
                return;
            }

            CurrentState?.Exit();
            CurrentState = nextState;
            CurrentState.Enter();
        }
    }
}
