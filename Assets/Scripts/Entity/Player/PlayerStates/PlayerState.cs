namespace Player.PlayerStates
{
    public abstract class PlayerState
    {
        protected readonly PlayerController player;
        protected readonly PlayerStateMachine stateMachine;
        private readonly string animationParameter;

        protected PlayerState(PlayerController player, PlayerStateMachine stateMachine, string animationParameter)
        {
            this.player = player;
            this.stateMachine = stateMachine;
            this.animationParameter = animationParameter;
        }

        public virtual void Enter()
        {
            player.SetAnimatorBool(animationParameter, true);
        }

        public virtual void Exit()
        {
            player.SetAnimatorBool(animationParameter, false);
        }

        public virtual void LogicUpdate()
        {
        }

        public virtual void PhysicsUpdate()
        {
        }
    }
}
