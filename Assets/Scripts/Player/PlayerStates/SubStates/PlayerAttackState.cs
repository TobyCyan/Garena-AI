using Player.Weapons;

namespace Player.PlayerStates
{
    public sealed class PlayerAttackState : PlayerState
    {
        private Weapon weapon;
        private bool attackFinished;

        public PlayerAttackState(PlayerController player, PlayerStateMachine stateMachine, string animationParameter)
            : base(player, stateMachine, animationParameter)
        {
        }

        public override void Enter()
        {
            base.Enter();
            attackFinished = false;
            weapon = player.ActiveWeapon;
            player.Movement.SetVelocityX(player.Movement.CurrentVelocity.x * player.Data.AttackMovementMultiplier);

            if (weapon == null)
            {
                attackFinished = true;
                return;
            }

            weapon.OnAttackFinished += HandleAttackFinished;
            if (!weapon.Attack())
            {
                attackFinished = true;
            }
        }

        public override void LogicUpdate()
        {
            base.LogicUpdate();
            if (!attackFinished)
            {
                return;
            }

            PlayerState nextState = player.InputHandler.NormInputX == 0 ? player.IdleState : player.MoveState;
            stateMachine.ChangeState(nextState);
        }

        public override void Exit()
        {
            if (weapon != null)
            {
                weapon.OnAttackFinished -= HandleAttackFinished;
            }

            weapon = null;
            base.Exit();
        }

        private void HandleAttackFinished(Weapon finishedWeapon)
        {
            attackFinished = true;
        }
    }
}
