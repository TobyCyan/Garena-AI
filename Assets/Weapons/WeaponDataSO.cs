using UnityEngine;

namespace Player.Weapons
{
    [CreateAssetMenu(fileName = "WeaponData", menuName = "Weapons/Melee Weapon")]
    public class WeaponDataSO : ScriptableObject
    {
        [Header("Presentation")]
        [SerializeField] private string weaponName = "Practice Sword";
        [SerializeField, TextArea] private string description;
        [SerializeField] private Sprite icon;
        [SerializeField] private Sprite weaponSprite;
        [SerializeField] private RuntimeAnimatorController animatorController;

        [Header("Melee Attack")]
        [SerializeField, Min(1)] private int damage = 1;
        [SerializeField, Min(1)] private int numberOfAttacks = 1;
        [SerializeField, Min(0f)] private float attackDuration = 0.35f;
        [SerializeField, Min(0f)] private float hitDelay = 0.12f;
        [SerializeField, Min(0f)] private float attackCooldown = 0.1f;
        [SerializeField] private Rect hitBox = new(0.3f, -0.25f, 1f, 0.5f);
        [SerializeField] private LayerMask targetLayers = ~0;
        [SerializeField] private bool drawDebugHitBox = true;

        public string WeaponName => weaponName;
        public string Description => description;
        public Sprite Icon => icon;
        public Sprite Image => weaponSprite;
        public Sprite WeaponSprite => weaponSprite;
        public RuntimeAnimatorController AnimatorController => animatorController;
        public int Damage => damage;
        public int NumberOfAttacks => numberOfAttacks;
        public float AttackDuration => attackDuration;
        public float HitDelay => Mathf.Min(hitDelay, attackDuration);
        public float AttackCooldown => attackCooldown;
        public Rect HitBox => hitBox;
        public Vector2 Size => hitBox.size;
        public float Speed => 1f / Mathf.Max(0.01f, attackDuration);
        public LayerMask TargetLayers => targetLayers;
        public bool DrawDebugHitBox => drawDebugHitBox;

        public void ConfigureRuntime(string displayName, int damageAmount, Rect meleeHitBox,
            float duration = 0.35f, float delay = 0.12f, float cooldown = 0.1f)
        {
            weaponName = string.IsNullOrWhiteSpace(displayName) ? "Runtime Weapon" : displayName;
            damage = Mathf.Max(1, damageAmount);
            hitBox = meleeHitBox;
            attackDuration = Mathf.Max(0.01f, duration);
            hitDelay = Mathf.Clamp(delay, 0f, attackDuration);
            attackCooldown = Mathf.Max(0f, cooldown);
        }

        public static WeaponDataSO CreateRuntimeMelee(string displayName, int damageAmount, Rect meleeHitBox)
        {
            WeaponDataSO data = CreateInstance<WeaponDataSO>();
            data.ConfigureRuntime(displayName, damageAmount, meleeHitBox);
            data.name = displayName;
            return data;
        }

        public void ConfigureRuntime(WeaponBuildRequest request)
        {
            if (request == null)
            {
                return;
            }

            weaponName = request.Image != null && !string.IsNullOrWhiteSpace(request.Image.name)
                ? request.Image.name
                : "AI Weapon";
            icon = request.Image;
            weaponSprite = request.Image;
            animatorController = null;
            damage = Mathf.Max(1, request.Damage);

            Vector2 size = new(
                Mathf.Max(0.05f, request.Size.x),
                Mathf.Max(0.05f, request.Size.y));
            hitBox = new Rect(0.15f, -size.y * 0.5f, size.x, size.y);

            attackDuration = 1f / Mathf.Max(0.1f, request.Speed);
            hitDelay = attackDuration * 0.45f;
            attackCooldown = 0f;
            numberOfAttacks = 1;
        }

        public static WeaponDataSO CreateRuntime(WeaponBuildRequest request)
        {
            if (request == null)
            {
                return null;
            }

            WeaponDataSO data = CreateInstance<WeaponDataSO>();
            data.ConfigureRuntime(request);
            data.name = data.WeaponName;
            return data;
        }
    }
}
