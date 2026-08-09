using UnityEngine;

namespace Player.Weapons
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(Weapon))]
    public sealed class MeleeWeaponSwingAnimator : MonoBehaviour
    {
        private const float RaisedAngle = 130f;
        private const float SwungAngle = -25f;

        [SerializeField] private Transform pivot;

        private Weapon weapon;
        private SpriteRenderer playerRenderer;
        private SpriteRenderer[] weaponRenderers;
        private Vector3 pivotRestLocalPosition;
        private Transform gripStart;
        private Transform gripRaised;
        private Transform gripEnd;
        private WeaponPose currentPose = WeaponPose.None;
        private bool keepVisible;

        private void Awake()
        {
            weapon = GetComponent<Weapon>();
            pivot = pivot != null ? pivot : transform.Find("Base");
            playerRenderer = transform.parent != null
                ? transform.parent.GetComponentInParent<SpriteRenderer>()
                : null;
            weaponRenderers = GetComponentsInChildren<SpriteRenderer>(true);
            pivotRestLocalPosition = pivot != null ? pivot.localPosition : Vector3.zero;
            SetWeaponVisible(keepVisible);
        }

        private void OnEnable()
        {
            weapon = weapon != null ? weapon : GetComponent<Weapon>();
            weaponRenderers ??= GetComponentsInChildren<SpriteRenderer>(true);
            weapon.OnAttackStarted += HandleAttackStarted;
            weapon.OnAttackFinished += HandleAttackFinished;
            SetWeaponVisible(keepVisible || weapon.IsAttacking);
        }

        private void OnDisable()
        {
            if (!keepVisible)
            {
                SetWeaponVisible(false);
            }
            currentPose = WeaponPose.None;

            if (weapon != null)
            {
                weapon.OnAttackStarted -= HandleAttackStarted;
                weapon.OnAttackFinished -= HandleAttackFinished;
            }
        }

        private void LateUpdate()
        {
            if (weapon == null || !weapon.IsAttacking || playerRenderer == null)
            {
                SetWeaponVisible(keepVisible);
                return;
            }

            WeaponPose pose = GetAttackPose(playerRenderer.sprite);
            if (pose == WeaponPose.None)
            {
                SetWeaponVisible(false);
                return;
            }

            if (currentPose != WeaponPose.None && pose < currentPose)
            {
                SetWeaponVisible(false);
                return;
            }

            ApplyPose(pose);
            SetWeaponVisible(true);
        }

        public void SetPivot(Transform swingPivot)
        {
            pivot = swingPivot;
            pivotRestLocalPosition = pivot != null ? pivot.localPosition : Vector3.zero;
        }

        public void SetGripPath(Transform start, Transform raised, Transform end)
        {
            gripStart = start;
            gripRaised = raised;
            gripEnd = end;
        }

        public void SetKeepVisible(bool visible)
        {
            keepVisible = visible;
            SetWeaponVisible(keepVisible || (weapon != null && weapon.IsAttacking));
        }

        private void HandleAttackStarted(Weapon attackingWeapon)
        {
            SetWeaponVisible(keepVisible);
            currentPose = WeaponPose.None;
        }

        private void HandleAttackFinished(Weapon attackingWeapon)
        {
            SetWeaponVisible(keepVisible);
        }

        private static WeaponPose GetAttackPose(Sprite sprite)
        {
            if (sprite == null || !sprite.name.StartsWith("Sword_Base_"))
            {
                return WeaponPose.None;
            }

            string suffix = sprite.name.Substring("Sword_Base_".Length);
            if (!int.TryParse(suffix, out int pose))
            {
                return WeaponPose.None;
            }

            return pose switch
            {
                0 => WeaponPose.Raised,
                1 => WeaponPose.Raised,
                2 => WeaponPose.Swung,
                _ => WeaponPose.None,
            };
        }

        private void ApplyPose(WeaponPose pose)
        {
            currentPose = pose;
            if (pivot != null)
            {
                Vector2 raisedGripOffset = gripStart != null && gripRaised != null
                    ? gripStart.InverseTransformVector(gripRaised.position - gripStart.position)
                    : Vector2.zero;
            
                Vector2 endGripOffset = gripStart != null && gripEnd != null
                    ? gripStart.InverseTransformVector(gripEnd.position - gripStart.position)
                    : Vector2.zero;

                Vector2 poseOffset = pose == WeaponPose.Swung
                    ? endGripOffset
                    : raisedGripOffset;
                float weaponAngle = pose == WeaponPose.Swung
                    ? SwungAngle
                    : RaisedAngle;
            
                pivot.SetLocalPositionAndRotation(
                    pivotRestLocalPosition + (Vector3)poseOffset,
                    Quaternion.Euler(0f, 0f, weaponAngle));
            }
        }

        private void SetWeaponVisible(bool visible)
        {
            if (weaponRenderers == null)
            {
                return;
            }

            foreach (SpriteRenderer weaponRenderer in weaponRenderers)
            {
                if (weaponRenderer != null)
                {
                    weaponRenderer.enabled = visible;
                }
            }
        }
    }

    public enum WeaponPose
    {
        None = -1,
        Raised = 0,
        Swung = 1,
    }
}
