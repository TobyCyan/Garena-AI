using Player.Weapons;
using UnityEngine;

namespace Player.PlayerStates
{
    public sealed class PlayerAttackState : PlayerState
    {
        private Weapon weapon;
        private bool attackFinished;
        private float elapsedAttackTime;
        private float attackDuration;

        public PlayerAttackState(PlayerController player, PlayerStateMachine stateMachine, string animationParameter)
            : base(player, stateMachine, animationParameter)
        {
        }

        public override void Enter()
        {
            attackFinished = false;
            elapsedAttackTime = 0f;
            weapon = player.ActiveWeapon;
            attackDuration = weapon?.AttackDuration ?? 0.6f;
            player.SetAttackAnimationDuration(attackDuration);
            base.Enter();
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
            elapsedAttackTime += Time.deltaTime;
            if (elapsedAttackTime >= attackDuration + 0.1f)
            {
                attackFinished = true;
            }

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
            player.ResetAttackAnimationSpeed();
            base.Exit();
        }

        private void HandleAttackFinished(Weapon finishedWeapon)
        {
            attackFinished = true;
        }
    }
}
