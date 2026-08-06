using System.Collections;
using UnityEngine;

namespace Player.Weapons
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(Weapon))]
    public sealed class WeaponSwingAnimator : MonoBehaviour
    {
        [SerializeField] private Transform pivot;
        [SerializeField] private float windUpAngle = -55f;
        [SerializeField] private float strikeAngle = 65f;

        private Weapon weapon;
        private Coroutine swingRoutine;

        private void Awake()
        {
            weapon = GetComponent<Weapon>();
            pivot ??= transform.Find("Base");
        }

        private void OnEnable()
        {
            weapon ??= GetComponent<Weapon>();
            weapon.OnAttackStarted += HandleAttackStarted;
            weapon.OnAttackFinished += HandleAttackFinished;
        }

        private void OnDisable()
        {
            if (weapon != null)
            {
                weapon.OnAttackStarted -= HandleAttackStarted;
                weapon.OnAttackFinished -= HandleAttackFinished;
            }

            StopSwing();
        }

        public void SetPivot(Transform swingPivot)
        {
            pivot = swingPivot;
        }

        private void HandleAttackStarted(Weapon attackingWeapon)
        {
            StopSwing();
            swingRoutine = StartCoroutine(Swing(attackingWeapon.Data));
        }

        private void HandleAttackFinished(Weapon attackingWeapon)
        {
            if (pivot != null)
            {
                pivot.localRotation = Quaternion.identity;
            }
        }

        private IEnumerator Swing(WeaponDataSO data)
        {
            if (pivot == null || data == null)
            {
                yield break;
            }

            Quaternion windUp = Quaternion.Euler(0f, 0f, windUpAngle);
            Quaternion strike = Quaternion.Euler(0f, 0f, strikeAngle);
            pivot.localRotation = windUp;

            yield return Rotate(windUp, strike, data.HitDelay);
            yield return Rotate(strike, Quaternion.identity, data.AttackDuration - data.HitDelay);
            swingRoutine = null;
        }

        private IEnumerator Rotate(Quaternion from, Quaternion to, float duration)
        {
            if (duration <= 0f)
            {
                pivot.localRotation = to;
                yield break;
            }

            float elapsed = 0f;
            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                pivot.localRotation = Quaternion.Slerp(from, to, Mathf.Clamp01(elapsed / duration));
                yield return null;
            }

            pivot.localRotation = to;
        }

        private void StopSwing()
        {
            if (swingRoutine != null)
            {
                StopCoroutine(swingRoutine);
                swingRoutine = null;
            }

            if (pivot != null)
            {
                pivot.localRotation = Quaternion.identity;
            }
        }
    }
}
