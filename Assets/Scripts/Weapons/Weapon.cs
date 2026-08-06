using System;
using System.Collections;
using System.Collections.Generic;
using Player.Core;
using UnityEngine;
using PlayerCore = Player.Core.Core;

namespace Player.Weapons
{
    public class Weapon : MonoBehaviour
    {
        [SerializeField] private WeaponDataSO data;
        [SerializeField] private GameObject baseObject;
        [SerializeField] private SpriteRenderer weaponRenderer;

        public event Action<Weapon> OnAttackStarted;
        public event Action<Weapon> OnAttackAction;
        public event Action<Weapon> OnAttackFinished;
        public event Action<Weapon, Collider2D> OnHit;

        public WeaponDataSO Data => data;
        public PlayerCore Core { get; private set; }
        public Animator Animator { get; private set; }
        public bool IsAttacking { get; private set; }
        public bool CanEnterAttack => data != null && !IsAttacking && Time.time >= nextAttackTime;
        public int AttackCounter { get; private set; }

        private readonly HashSet<Collider2D> hitTargets = new();
        private AnimationEventHandler eventHandler;
        private Coroutine attackRoutine;
        private float nextAttackTime;
        private bool attackActionTriggered;
        private bool attackFinishedTriggered;

        private void Awake()
        {
            ResolveDependencies();
            ApplyData();
        }

        private void OnEnable()
        {
            BindAnimationEvents(true);
        }

        private void OnDisable()
        {
            BindAnimationEvents(false);
            if (attackRoutine != null)
            {
                StopCoroutine(attackRoutine);
                attackRoutine = null;
            }

            IsAttacking = false;
        }

        public void SetCore(PlayerCore core)
        {
            Core = core;
        }

        public void SetData(WeaponDataSO weaponData)
        {
            data = weaponData;
            ResolveDependencies();
            ApplyData();
            AttackCounter = 0;
        }

        public bool Attack()
        {
            if (!CanEnterAttack)
            {
                return false;
            }

            IsAttacking = true;
            attackActionTriggered = false;
            attackFinishedTriggered = false;
            hitTargets.Clear();

            if (Animator != null && Animator.runtimeAnimatorController != null)
            {
                Animator.SetInteger("counter", AttackCounter);
                Animator.SetTrigger("attack");
            }

            OnAttackStarted?.Invoke(this);
            attackRoutine = StartCoroutine(AttackRoutine());
            return true;
        }

        private IEnumerator AttackRoutine()
        {
            if (data.HitDelay > 0f)
            {
                yield return new WaitForSeconds(data.HitDelay);
            }

            if (!attackActionTriggered)
            {
                PerformAttackAction();
            }

            float remainingDuration = Mathf.Max(0f, data.AttackDuration - data.HitDelay);
            if (remainingDuration > 0f)
            {
                yield return new WaitForSeconds(remainingDuration);
            }

            if (!attackFinishedTriggered)
            {
                FinishAttack();
            }
        }

        private void PerformAttackAction()
        {
            if (!IsAttacking || attackActionTriggered)
            {
                return;
            }

            attackActionTriggered = true;
            OnAttackAction?.Invoke(this);

            Vector2 center = GetHitBoxCenter();
            Collider2D[] targets = Physics2D.OverlapBoxAll(center, data.HitBox.size, 0f, data.TargetLayers);
            GameObject source = Core != null && Core.Root != null ? Core.Root : gameObject;

            foreach (Collider2D target in targets)
            {
                if (target == null || target.transform.root == source.transform.root || !hitTargets.Add(target))
                {
                    continue;
                }

                IDamageable damageable = target.GetComponentInParent<IDamageable>();
                damageable?.TakeDamage(new DamageData(data.Damage, source, data));
                OnHit?.Invoke(this, target);
            }
        }

        private void FinishAttack()
        {
            if (!IsAttacking || attackFinishedTriggered)
            {
                return;
            }

            attackFinishedTriggered = true;
            IsAttacking = false;
            attackRoutine = null;
            nextAttackTime = Time.time + data.AttackCooldown;
            AttackCounter = (AttackCounter + 1) % Mathf.Max(1, data.NumberOfAttacks);
            OnAttackFinished?.Invoke(this);
        }

        private void ResolveDependencies()
        {
            baseObject ??= transform.Find("Base")?.gameObject ?? gameObject;
            Animator = baseObject.GetComponent<Animator>();
            eventHandler = baseObject.GetComponent<AnimationEventHandler>();
            weaponRenderer ??= GetComponentInChildren<SpriteRenderer>(true);
        }

        private void ApplyData()
        {
            if (data == null)
            {
                return;
            }

            if (Animator != null)
            {
                Animator.runtimeAnimatorController = data.AnimatorController;
            }

            if (weaponRenderer != null)
            {
                weaponRenderer.sprite = data.WeaponSprite;
            }

            gameObject.name = data.WeaponName;
        }

        private void BindAnimationEvents(bool bind)
        {
            if (eventHandler == null)
            {
                return;
            }

            if (bind)
            {
                eventHandler.AttackAction += PerformAttackAction;
                eventHandler.AttackFinished += FinishAttack;
            }
            else
            {
                eventHandler.AttackAction -= PerformAttackAction;
                eventHandler.AttackFinished -= FinishAttack;
            }
        }

        private Vector2 GetHitBoxCenter()
        {
            Movement movement = Core != null ? Core.GetCoreComponent<Movement>() : null;
            if (movement != null)
            {
                return movement.GetWorldPoint(data.HitBox.center);
            }

            int facing = transform.root.localScale.x >= 0f ? 1 : -1;
            Vector2 offset = data.HitBox.center;
            offset.x *= facing;
            return (Vector2)transform.root.position + offset;
        }

        private void OnDrawGizmosSelected()
        {
            if (data == null || !data.DrawDebugHitBox)
            {
                return;
            }

            Gizmos.color = Color.red;
            Gizmos.DrawWireCube(GetHitBoxCenter(), data.HitBox.size);
        }
    }
}
