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
        private const float HitMoment = 0.45f;
        private const float HitBoxWorldScale = 0.25f;
        private const float HitBoxForwardGap = 0.03f;

        [SerializeField] private string weaponName = "AI Weapon";
        [SerializeField] private WeaponCategory category = WeaponCategory.Sword;
        [SerializeField] private Sprite image;
        [SerializeField] private WeaponStats stats = new();
        [SerializeField] private SpriteRenderer weaponRenderer;

        public event Action<Weapon> OnAttackStarted;
        public event Action<Weapon> OnAttackAction;
        public event Action<Weapon> OnAttackFinished;
        public event Action<Weapon, Collider2D> OnHit;

        public string WeaponName => weaponName;
        public WeaponCategory Category => category;
        public Sprite Image => image;
        public WeaponStats Stats => stats;
        public PlayerCore Core { get; private set; }
        public bool IsAttacking { get; private set; }
        public bool CanEnterAttack => stats != null && !IsAttacking;
        public float AttackDuration => 1f / Mathf.Max(0.1f, stats?.Speed ?? 1f);

        private readonly HashSet<Collider2D> hitTargets = new();
        private Coroutine attackRoutine;
        private bool attackActionTriggered;

        private void Awake()
        {
            weaponRenderer ??= GetComponentInChildren<SpriteRenderer>(true);
            ApplyPresentation();
        }

        private void OnDisable()
        {
            if (attackRoutine != null)
            {
                StopCoroutine(attackRoutine);
                attackRoutine = null;
            }

            IsAttacking = false;
        }

        public void Configure(WeaponDefinition definition)
        {
            if (definition == null)
            {
                return;
            }

            weaponName = string.IsNullOrWhiteSpace(definition.Name)
                ? "AI Weapon"
                : definition.Name;
            category = definition.Category;
            image = definition.Image;
            stats = definition.Stats?.CopySanitized() ?? new WeaponStats();
            weaponRenderer ??= GetComponentInChildren<SpriteRenderer>(true);
            ApplyPresentation();
        }

        public void SetCore(PlayerCore core)
        {
            Core = core;
        }

        public bool Attack()
        {
            if (!CanEnterAttack)
            {
                return false;
            }

            IsAttacking = true;
            attackActionTriggered = false;
            hitTargets.Clear();
            OnAttackStarted?.Invoke(this);
            attackRoutine = StartCoroutine(AttackRoutine());
            return true;
        }

        private IEnumerator AttackRoutine()
        {
            float duration = AttackDuration;
            float hitDelay = duration * HitMoment;

            if (hitDelay > 0f)
            {
                yield return new WaitForSeconds(hitDelay);
            }

            PerformAttackAction();

            float remainingDuration = duration - hitDelay;
            if (remainingDuration > 0f)
            {
                yield return new WaitForSeconds(remainingDuration);
            }

            FinishAttack();
        }

        private void PerformAttackAction()
        {
            if (!IsAttacking || attackActionTriggered)
            {
                return;
            }

            attackActionTriggered = true;
            OnAttackAction?.Invoke(this);

            Vector2 hitBoxSize = GetHitBoxSize();
            Collider2D[] targets = Physics2D.OverlapBoxAll(
                GetHitBoxCenter(hitBoxSize), hitBoxSize, 0f);
            GameObject source = Core != null && Core.Root != null ? Core.Root : gameObject;

            foreach (Collider2D target in targets)
            {
                if (target == null || target.transform.root == source.transform.root || !hitTargets.Add(target))
                {
                    continue;
                }

                IDamageable damageable = target.GetComponentInParent<IDamageable>();
                damageable?.TakeDamage(new DamageData(stats.Damage, source, this));
                OnHit?.Invoke(this, target);
            }
        }

        private void FinishAttack()
        {
            if (!IsAttacking)
            {
                return;
            }

            IsAttacking = false;
            attackRoutine = null;
            OnAttackFinished?.Invoke(this);
        }

        private void ApplyPresentation()
        {
            if (weaponRenderer != null)
            {
                weaponRenderer.sprite = image;
                weaponRenderer.enabled = image != null;
                weaponRenderer.gameObject.SetActive(true);
            }

            gameObject.name = weaponName;
        }

        private Vector2 GetHitBoxCenter(Vector2 hitBoxSize)
        {
            Vector2 localOffset = new(HitBoxForwardGap + hitBoxSize.x * 0.5f, 0f);
            Movement movement = Core != null ? Core.GetCoreComponent<Movement>() : null;
            if (movement != null)
            {
                return movement.GetWorldPoint(localOffset);
            }

            int facing = transform.root.localScale.x >= 0f ? 1 : -1;
            localOffset.x *= facing;
            return (Vector2)transform.root.position + localOffset;
        }

        private Vector2 GetHitBoxSize()
        {
            return new Vector2(
                Mathf.Max(0.05f, stats.Size.x * HitBoxWorldScale),
                Mathf.Max(0.05f, stats.Size.y * HitBoxWorldScale));
        }

        private void OnDrawGizmosSelected()
        {
            if (stats == null)
            {
                return;
            }

            Vector2 hitBoxSize = GetHitBoxSize();
            Gizmos.color = Color.red;
            Gizmos.DrawWireCube(GetHitBoxCenter(hitBoxSize), hitBoxSize);
        }
    }
}
