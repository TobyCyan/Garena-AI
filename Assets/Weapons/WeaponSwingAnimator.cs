using UnityEngine;

namespace Player.Weapons
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(Weapon))]
    public sealed class WeaponSwingAnimator : MonoBehaviour
    {
        // Progress from WeaponRaised to WeaponEnd after the raised pose.
        private static readonly float[] RaisedToEndProgress = { 0f, 0f, 0.4f, 0.8f, 1f };
        private static readonly float[] WeaponAngles = { 130f, 130f, -25f, -45f, -45f };

        [SerializeField] private Transform pivot;

        private Weapon weapon;
        private SpriteRenderer playerRenderer;
        private SpriteRenderer[] weaponRenderers;
        private Vector3 pivotRestLocalPosition;
        private Transform gripStart;
        private Transform gripRaised;
        private Transform gripEnd;
        private int currentPose = -1;

        private void Awake()
        {
            weapon = GetComponent<Weapon>();
            pivot ??= transform.Find("Base");
            playerRenderer = transform.parent != null
                ? transform.parent.GetComponentInParent<SpriteRenderer>()
                : null;
            weaponRenderers = GetComponentsInChildren<SpriteRenderer>(true);
            pivotRestLocalPosition = pivot != null ? pivot.localPosition : Vector3.zero;
            SetWeaponVisible(false);
        }

        private void OnEnable()
        {
            weapon ??= GetComponent<Weapon>();
            weaponRenderers ??= GetComponentsInChildren<SpriteRenderer>(true);
            weapon.OnAttackStarted += HandleAttackStarted;
            weapon.OnAttackFinished += HandleAttackFinished;
            SetWeaponVisible(weapon.IsAttacking);
        }

        private void OnDisable()
        {
            if (weapon != null)
            {
                weapon.OnAttackStarted -= HandleAttackStarted;
                weapon.OnAttackFinished -= HandleAttackFinished;
            }

            ResetPose();
        }

        private void LateUpdate()
        {
            if (weapon == null || !weapon.IsAttacking || playerRenderer == null)
            {
                return;
            }

            int pose = GetAttackPose(playerRenderer.sprite);
            if (pose >= 0)
            {
                ApplyPose(pose);
            }
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

        private void HandleAttackStarted(Weapon attackingWeapon)
        {
            currentPose = -1;
            SetWeaponVisible(true);
        }

        private void HandleAttackFinished(Weapon attackingWeapon)
        {
            ResetPose();
        }

        private static int GetAttackPose(Sprite sprite)
        {
            if (sprite == null || !sprite.name.StartsWith("Sword_Base_"))
            {
                return -1;
            }

            string suffix = sprite.name.Substring("Sword_Base_".Length);
            return int.TryParse(suffix, out int pose) && pose >= 0 && pose < RaisedToEndProgress.Length
                ? pose
                : -1;
        }

        private void ApplyPose(int pose)
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

                Vector2 poseOffset = Vector2.Lerp(
                    raisedGripOffset,
                    endGripOffset,
                    RaisedToEndProgress[pose]);
                pivot.localPosition = pivotRestLocalPosition +
                    (Vector3)poseOffset;
                pivot.localRotation = Quaternion.Euler(0f, 0f, WeaponAngles[pose]);
            }
        }

        private void ResetPose()
        {
            currentPose = -1;
            if (pivot != null)
            {
                pivot.localPosition = pivotRestLocalPosition;
                pivot.localRotation = Quaternion.identity;
            }

            SetWeaponVisible(false);
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
}
