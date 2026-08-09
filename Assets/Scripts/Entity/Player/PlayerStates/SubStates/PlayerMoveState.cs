using UnityEngine;

namespace Player.PlayerStates
{
    public sealed class PlayerMoveState : PlayerGroundedState
    {
        private float footstepTimer;

        public PlayerMoveState(PlayerController player, PlayerStateMachine stateMachine, string animationParameter)
            : base(player, stateMachine, animationParameter)
        {
        }

        public override void Enter()
        {
            base.Enter();
            footstepTimer = 0f;
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

            footstepTimer -= Time.deltaTime;

            if (footstepTimer <= 0f)
            {
                SoundEffectManager.Play("footstep");
                footstepTimer = player.FootstepInterval;
            }
        }

        public override void Exit()
        {
            base.Exit();
            footstepTimer = 0f;
        }
    }
}
