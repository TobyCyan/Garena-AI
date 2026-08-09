namespace Player.PlayerStates
{
    public sealed class PlayerIdleState : PlayerGroundedState
    {
        public PlayerIdleState(PlayerController player, PlayerStateMachine stateMachine, string animationParameter)
            : base(player, stateMachine, animationParameter)
        {
        }

        public override void Enter()
        {
            base.Enter();
            player.Movement.SetVelocityX(0f);
        }

        public override void LogicUpdate()
        {
            base.LogicUpdate();
            if (stateMachine.CurrentState != this)
            {
                return;
            }

            player.Movement.SetVelocityX(0f);
            if (HorizontalInput != 0)
            {
                stateMachine.ChangeState(player.MoveState);
            }
        }
    }
}
