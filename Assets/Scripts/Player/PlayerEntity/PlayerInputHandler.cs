using UnityEngine;
using UnityEngine.InputSystem;

namespace Player
{
    [DisallowMultipleComponent]
    public class PlayerInputHandler : MonoBehaviour
    {
        [SerializeField, Range(0f, 1f)] private float axisDeadZone = 0.4f;
        [SerializeField] private bool enableKeyboardFallback = true;
        [SerializeField] private bool logInputDebug = true;

        [Header("Runtime Debug")]
        [SerializeField] private Vector2 rawMovementInput;
        [SerializeField] private int normalizedHorizontal;
        [SerializeField] private int normalizedVertical;

        public Vector2 RawMovementInput => rawMovementInput;
        public int NormInputX => normalizedHorizontal;
        public int NormInputY => normalizedVertical;
        public bool AttackInput => attackInput;
        public bool NextWeaponInput => nextWeaponInput;
        public bool PreviousWeaponInput => previousWeaponInput;

        private bool attackInput;
        private bool nextWeaponInput;
        private bool previousWeaponInput;
        private bool usesPlayerInputComponent;
        private PlayerInput playerInput;
        private InputAction fallbackMoveAction;
        private InputAction fallbackAttackAction;
        private InputAction fallbackNextWeaponAction;
        private InputAction fallbackPreviousWeaponAction;
        private int lastLoggedHorizontal;

        private void Awake()
        {
            playerInput = GetComponent<PlayerInput>();
            usesPlayerInputComponent = playerInput != null;
            if (!usesPlayerInputComponent)
            {
                CreateFallbackActions();
            }
        }

        private void Start()
        {
            if (playerInput == null || playerInput.actions == null)
            {
                return;
            }

            playerInput.actions.Disable();
            playerInput.defaultActionMap = "Player";
            playerInput.ActivateInput();

            if (playerInput.currentActionMap == null || playerInput.currentActionMap.name != "Player")
            {
                playerInput.SwitchCurrentActionMap("Player");
            }

            if (logInputDebug)
            {
                Debug.Log(
                    $"[PlayerInput] Active action map: {playerInput.currentActionMap?.name ?? "None"}.",
                    this);
            }
        }

        private void OnEnable()
        {
            if (enableKeyboardFallback && !usesPlayerInputComponent)
            {
                fallbackMoveAction?.Enable();
                fallbackAttackAction?.Enable();
                fallbackNextWeaponAction?.Enable();
                fallbackPreviousWeaponAction?.Enable();
            }

            if (logInputDebug)
            {
                Debug.Log(
                    $"[PlayerInput] Enabled. PlayerInput component: {usesPlayerInputComponent}, " +
                    $"generated keyboard actions: {fallbackMoveAction?.enabled == true}.",
                    this);
            }
        }

        private void OnDisable()
        {
            fallbackMoveAction?.Disable();
            fallbackAttackAction?.Disable();
            fallbackNextWeaponAction?.Disable();
            fallbackPreviousWeaponAction?.Disable();
        }

        private void OnDestroy()
        {
            fallbackMoveAction?.Dispose();
            fallbackAttackAction?.Dispose();
            fallbackNextWeaponAction?.Dispose();
            fallbackPreviousWeaponAction?.Dispose();
        }

        private void Update()
        {
            if (!enableKeyboardFallback || usesPlayerInputComponent || fallbackMoveAction == null)
            {
                return;
            }

            Keyboard keyboard = Keyboard.current;
            if (logInputDebug && keyboard != null &&
                (keyboard.aKey.wasPressedThisFrame || keyboard.dKey.wasPressedThisFrame))
            {
                Debug.Log(
                    $"[PlayerInput] Physical key detected. A: {keyboard.aKey.isPressed}, " +
                    $"D: {keyboard.dKey.isPressed}.",
                    this);
            }

            SetMovement(fallbackMoveAction.ReadValue<Vector2>());

            if (fallbackAttackAction.WasPressedThisFrame())
            {
                attackInput = true;
            }

            if (fallbackNextWeaponAction.WasPressedThisFrame())
            {
                nextWeaponInput = true;
            }

            if (fallbackPreviousWeaponAction.WasPressedThisFrame())
            {
                previousWeaponInput = true;
            }
        }

        public void OnMove(InputValue value)
        {
            SetMovement(value.Get<Vector2>());
        }

        public void OnAttack(InputValue value)
        {
            if (value.isPressed)
            {
                attackInput = true;
            }
        }

        public void OnNext(InputValue value)
        {
            if (value.isPressed)
            {
                nextWeaponInput = true;
            }
        }

        public void OnPrevious(InputValue value)
        {
            if (value.isPressed)
            {
                previousWeaponInput = true;
            }
        }

        public bool ConsumeAttackInput()
        {
            return Consume(ref attackInput);
        }

        public bool ConsumeNextWeaponInput()
        {
            return Consume(ref nextWeaponInput);
        }

        public bool ConsumePreviousWeaponInput()
        {
            return Consume(ref previousWeaponInput);
        }

        private void SetMovement(Vector2 input)
        {
            rawMovementInput = input;
            normalizedHorizontal = Mathf.Abs(input.x) >= axisDeadZone ? (int)Mathf.Sign(input.x) : 0;
            normalizedVertical = Mathf.Abs(input.y) >= axisDeadZone ? (int)Mathf.Sign(input.y) : 0;

            if (logInputDebug && normalizedHorizontal != lastLoggedHorizontal)
            {
                Debug.Log(
                    $"[PlayerInput] Movement changed. Raw: {rawMovementInput}, " +
                    $"normalized X: {normalizedHorizontal}.",
                    this);
                lastLoggedHorizontal = normalizedHorizontal;
            }
        }

        private void CreateFallbackActions()
        {
            fallbackMoveAction = new InputAction("Fallback Move", InputActionType.Value);
            fallbackMoveAction.AddCompositeBinding("2DVector")
                .With("Up", "<Keyboard>/w")
                .With("Down", "<Keyboard>/s")
                .With("Left", "<Keyboard>/a")
                .With("Right", "<Keyboard>/d");
            fallbackMoveAction.AddCompositeBinding("2DVector")
                .With("Up", "<Keyboard>/upArrow")
                .With("Down", "<Keyboard>/downArrow")
                .With("Left", "<Keyboard>/leftArrow")
                .With("Right", "<Keyboard>/rightArrow");

            fallbackAttackAction = new InputAction("Fallback Attack", InputActionType.Button);
            fallbackAttackAction.AddBinding("<Keyboard>/space");
            fallbackAttackAction.AddBinding("<Mouse>/leftButton");

            fallbackNextWeaponAction = new InputAction("Fallback Next Weapon", InputActionType.Button);
            fallbackNextWeaponAction.AddBinding("<Keyboard>/e");
            fallbackNextWeaponAction.AddBinding("<Keyboard>/2");

            fallbackPreviousWeaponAction = new InputAction("Fallback Previous Weapon", InputActionType.Button);
            fallbackPreviousWeaponAction.AddBinding("<Keyboard>/q");
            fallbackPreviousWeaponAction.AddBinding("<Keyboard>/1");
        }

        private bool Consume(ref bool input)
        {
            if (!input)
            {
                return false;
            }

            input = false;
            return true;
        }
    }
}
