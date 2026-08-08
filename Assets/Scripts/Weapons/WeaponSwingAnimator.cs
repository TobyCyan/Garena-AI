using UnityEngine;

namespace Player.Weapons
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(Weapon))]
    public sealed class WeaponSwingAnimator : MonoBehaviour
    {
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
            SetWeaponVisible(false);
            currentPose = -1;

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
                SetWeaponVisible(false);
                return;
            }

            int pose = GetAttackPose(playerRenderer.sprite);
            if (pose < 0)
            {
                SetWeaponVisible(false);
                return;
            }

            if (currentPose == -1 && pose >= 3)
            {
                SetWeaponVisible(false);
                return;
            }

            if (currentPose >= 0 && pose < currentPose)
            {
                SetWeaponVisible(false);
                return;
            }

            if (pose >= 3)
            {
                SetWeaponVisible(false);
                currentPose = pose;
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

        private void HandleAttackStarted(Weapon attackingWeapon)
        {
            SetWeaponVisible(false);
            currentPose = -1;
        }

        private void HandleAttackFinished(Weapon attackingWeapon)
        {
            SetWeaponVisible(false);
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

                Vector2 raisedCompensation = new Vector2(0.254f, -0.314f);
                Vector2 endCompensation = new Vector2(-0.631f, 0.661f);

                raisedGripOffset += raisedCompensation;
                endGripOffset += endCompensation;

                Vector2 poseOffset = Vector2.Lerp(
                    raisedGripOffset,
                    endGripOffset,
                    RaisedToEndProgress[pose]);
            
                pivot.localPosition = pivotRestLocalPosition + (Vector3)poseOffset;
                pivot.localRotation = Quaternion.Euler(0f, 0f, WeaponAngles[pose]);
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
}