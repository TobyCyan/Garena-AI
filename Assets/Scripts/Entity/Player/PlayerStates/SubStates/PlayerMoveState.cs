namespace Player.PlayerStates
{
    public sealed class PlayerMoveState : PlayerGroundedState
    {
        public PlayerMoveState(PlayerController player, PlayerStateMachine stateMachine, string animationParameter)
            : base(player, stateMachine, animationParameter)
        {
        }

        public override void LogicUpdate()
        {
            base.LogicUpdate();
            if (stateMachine.CurrentState != this)
            {
                return;
            }

            player.Movement.CheckIfShouldFlip(HorizontalInput);
            player.Movement.SetVelocityX(player.Data.MovementSpeed * HorizontalInput);

            if (HorizontalInput == 0)
            {
                stateMachine.ChangeState(player.IdleState);
            }
        }
    }
}
