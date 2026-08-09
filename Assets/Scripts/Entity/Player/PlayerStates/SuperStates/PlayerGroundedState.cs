namespace Player.PlayerStates
{
    public abstract class PlayerGroundedState : PlayerState
    {
        protected int HorizontalInput { get; private set; }

        protected PlayerGroundedState(PlayerController player, PlayerStateMachine stateMachine, string animationParameter)
            : base(player, stateMachine, animationParameter)
        {
        }

        public override void LogicUpdate()
        {
            base.LogicUpdate();
            HorizontalInput = player.InputHandler.NormInputX;
            player.TryStartAttack();
        }
    }
}
