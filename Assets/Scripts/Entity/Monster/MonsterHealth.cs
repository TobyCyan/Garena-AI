using Player.Weapons;
using UnityEngine;

[DisallowMultipleComponent]
[RequireComponent(typeof(BoxCollider2D))]
public sealed class MonsterHealth : MonoBehaviour, IDamageable
{
    [SerializeField, Min(1)] private int maxHealth = 10;
    [SerializeField, Min(0)] private int currentHealth;
    [SerializeField] private ProgressBar healthBar;
    public Animator animator;

    public int CurrentHealth => currentHealth;
    public int MaxHealth => maxHealth;

    private bool isDead;

    private void Awake()
    {
        if (!TryGetComponent<Collider2D>(out Collider2D hitCollider))
        {
            hitCollider = gameObject.AddComponent<BoxCollider2D>();
        }

        hitCollider.isTrigger = true;
        healthBar ??= GetComponentInChildren<ProgressBar>(true);
        currentHealth = Mathf.Max(1, maxHealth);

        if (healthBar == null)
        {
            Debug.LogWarning(
                "[Monster] No ProgressBar was found. Damage still works, but health will not be visible.",
                this);
        }

        UpdateHealthBar();
    }

    public void TakeDamage(DamageData damage)
    {
        if (isDead || damage.Amount <= 0)
        {
            return;
        }

        currentHealth = Mathf.Max(0, currentHealth - damage.Amount);
        UpdateHealthBar();
        animator.SetTrigger("Attacked");
        SoundEffectManager.Play("punch");

        string weaponName = damage.Weapon != null
            ? damage.Weapon.WeaponName
            : "Unknown Weapon";
        Debug.Log(
            $"[Monster] Took {damage.Amount} damage from {weaponName}. " +
            $"Health: {currentHealth}/{MaxHealth}, " +
            $"barFill={(healthBar != null ? healthBar.NormalizedValue : -1f):F2}",
            this);

        if (currentHealth == 0)
        {
            isDead = true;
            Destroy(gameObject);
        }
    }

    private void UpdateHealthBar()
    {
        healthBar?.UpdateFill(currentHealth, MaxHealth);
    }
}
