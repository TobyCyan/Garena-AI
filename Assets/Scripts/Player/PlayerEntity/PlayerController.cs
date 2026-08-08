using Player.Core;
using Player.PlayerStates;
using Player.Weapons;
using UnityEngine;

namespace Player
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(Rigidbody2D), typeof(PlayerInputHandler))]
    public class PlayerController : MonoBehaviour
    {
        [SerializeField] private PlayerData playerData;
        [SerializeField] private Animator animator;
        private Core.Core core;
        [SerializeField] private WeaponInventory weaponInventory;
        [SerializeField, Min(0.01f)] private float baseAttackAnimationDuration = 0.6f;
        [SerializeField] private bool logMovementDebug = true;

        public PlayerData Data => playerData;
        public PlayerInputHandler InputHandler { get; private set; }
        public Movement Movement { get; private set; }
        public WeaponInventory WeaponInventory => weaponInventory;
        public Weapon ActiveWeapon => weaponInventory != null ? weaponInventory.CurrentWeapon : null;
        public PlayerStateMachine StateMachine { get; private set; }
        public PlayerIdleState IdleState { get; private set; }
        public PlayerMoveState MoveState { get; private set; }
        public PlayerAttackState AttackState { get; private set; }

        private void Awake()
        {
            playerData ??= PlayerData.CreateRuntimeDefault();
            InputHandler = GetComponent<PlayerInputHandler>();
            animator ??= GetComponent<Animator>();

            EnsureCore();
            EnsureWeapons();
            EnsureStateMachine();

            if (logMovementDebug)
            {
                Debug.Log(
                    $"[PlayerController] Awake complete. Input: {InputHandler != null}, " +
                    $"Core: {core != null}, Movement: {Movement != null}, " +
                    $"Rigidbody: {Movement?.Rigidbody != null}.",
                    this);
            }
        }

        private void Start()
        {
            EnsureRuntimeReferences();
            if (StateMachine.CurrentState == null)
            {
                StateMachine.Initialize(IdleState);
            }

            if (logMovementDebug)
            {
                Debug.Log("[PlayerController] State machine started in PlayerIdleState.", this);
            }
        }

        private void Update()
        {
            EnsureRuntimeReferences();
            core.LogicUpdate();
            HandleWeaponSwapInput();
            StateMachine?.CurrentState?.LogicUpdate();

            LogMovementWhenInputChanges();
        }

        private void FixedUpdate()
        {
            StateMachine?.CurrentState?.PhysicsUpdate();
        }

        public bool TryStartAttack()
        {
            if (!InputHandler.AttackInput || ActiveWeapon == null || !ActiveWeapon.CanEnterAttack)
            {
                return false;
            }

            InputHandler.ConsumeAttackInput();
            StateMachine.ChangeState(AttackState);
            return true;
        }

        public void SetAnimatorBool(string parameterName, bool value)
        {
            if (animator != null && animator.runtimeAnimatorController != null && !string.IsNullOrWhiteSpace(parameterName))
            {
                animator.SetBool(parameterName, value);
            }
        }

        public void SetAttackAnimationDuration(float duration)
        {
            if (animator != null && animator.runtimeAnimatorController != null)
            {
                animator.SetFloat("attackSpeed", baseAttackAnimationDuration / Mathf.Max(0.01f, duration));
            }
        }

        public void ResetAttackAnimationSpeed()
        {
            if (animator != null && animator.runtimeAnimatorController != null)
            {
                animator.SetFloat("attackSpeed", 1f);
            }
        }

        [ContextMenu("Debug/Move Right During Play Mode")]
        private void DebugMoveRight()
        {
            if (!Application.isPlaying || Movement == null)
            {
                Debug.LogWarning("[PlayerController] Enter Play Mode before running the movement test.", this);
                return;
            }

            Movement.SetVelocityX(Data.MovementSpeed);
            Debug.Log(
                $"[PlayerController] Forced movement test. Requested X velocity: {Data.MovementSpeed}, " +
                $"actual velocity: {Movement.Rigidbody.linearVelocity}.",
                this);
        }

        private void HandleWeaponSwapInput()
        {
            if (StateMachine?.CurrentState == AttackState || weaponInventory == null || InputHandler == null)
            {
                return;
            }

            if (InputHandler.ConsumeNextWeaponInput())
            {
                weaponInventory.EquipNext();
            }
            else if (InputHandler.ConsumePreviousWeaponInput())
            {
                weaponInventory.EquipPrevious();
            }
        }

        private void EnsureCore()
        {
            core ??= GetComponentInChildren<Core.Core>(true);
            if (core == null)
            {
                GameObject coreObject = new("Core");
                coreObject.transform.SetParent(transform, false);
                core = coreObject.AddComponent<Core.Core>();
            }

            Movement = core.GetCoreComponent<Movement>();
            if (Movement == null)
            {
                Movement = core.gameObject.AddComponent<Movement>();
                core.RegisterComponent(Movement);
            }
        }

        private void EnsureWeapons()
        {
            weaponInventory ??= GetComponentInChildren<WeaponInventory>(true);
            if (weaponInventory == null)
            {
                weaponInventory = gameObject.AddComponent<WeaponInventory>();
            }

            weaponInventory.Initialize(core);
        }

        private void EnsureStateMachine()
        {
            if (StateMachine != null && IdleState != null && MoveState != null && AttackState != null)
            {
                return;
            }

            StateMachine = new PlayerStateMachine();
            IdleState = new PlayerIdleState(this, StateMachine, "idle");
            MoveState = new PlayerMoveState(this, StateMachine, "move");
            AttackState = new PlayerAttackState(this, StateMachine, "attack");
        }

        private void EnsureRuntimeReferences()
        {
            playerData ??= PlayerData.CreateRuntimeDefault();
            InputHandler ??= GetComponent<PlayerInputHandler>();
            animator ??= GetComponent<Animator>();

            if (core == null || Movement == null)
            {
                EnsureCore();
            }

            if (weaponInventory == null || weaponInventory.CurrentWeapon == null)
            {
                EnsureWeapons();
            }

            if (StateMachine == null || IdleState == null || MoveState == null || AttackState == null)
            {
                EnsureStateMachine();
                StateMachine.Initialize(IdleState);
                Debug.LogWarning(
                    "[PlayerController] Runtime state was missing and has been rebuilt. " +
                    "This usually happens after scripts recompile during Play Mode.",
                    this);
            }
        }

        private int lastLoggedHorizontalInput;

        private void LogMovementWhenInputChanges()
        {
            if (!logMovementDebug || InputHandler.NormInputX == lastLoggedHorizontalInput)
            {
                return;
            }

            lastLoggedHorizontalInput = InputHandler.NormInputX;
            string stateName = StateMachine.CurrentState?.GetType().Name ?? "None";
            Vector2 velocity = Movement?.Rigidbody != null
                ? Movement.Rigidbody.linearVelocity
                : Vector2.zero;

            Debug.Log(
                $"[PlayerController] Input X: {lastLoggedHorizontalInput}, state: {stateName}, " +
                $"Rigidbody velocity: {velocity}.",
                this);
        }
    }
}
