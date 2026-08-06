using UnityEngine;
using System.Collections;

public class WeaponShowcaseManager : MonoBehaviour
{
    [SerializeField] private ParticleSystem burstEffect;

    // Particle System Settings
    private const int SEED = 12345;

    // For testing.
    [SerializeField] private GameObject weapon;

    // Weapon Sprite Animation Settings
    [SerializeField] private float lerpDuration = 0.25f;
    [SerializeField] private Vector3 initialSpriteScale = new(1.5f, 1.5f, 1f);
    private static readonly Vector3 ORIGINAL_SPRITE_SCALE = Vector3.one;

    private void Awake()
    {
        if (burstEffect != null)
        {
            burstEffect.Stop();
            burstEffect.useAutoRandomSeed = false;
            burstEffect.randomSeed = SEED;
        }

        if (weapon != null)
        {
            weapon.SetActive(false);
        }
    }

    private void Start()
    {
        if (weapon != null)
        {
            ShowcaseWeapon(weapon);
        }
    }

    public void ShowcaseWeapon(GameObject weaponGo)
    {
        if (weaponGo == null)
        {
            Debug.LogWarning("Weapon GameObject is null. Cannot showcase.");
            return;
        }

        weaponGo.SetActive(true);
        weaponGo.transform.localPosition = Vector3.zero;
        if (burstEffect != null)
        {
            burstEffect.Play();
        }

        if (weaponGo.TryGetComponent(out SpriteRenderer spriteRenderer))
        {
            LerpWeaponSpriteScale(spriteRenderer, lerpDuration);
        }
        else
        {
            Debug.LogWarning("Weapon GameObject does not have a SpriteRenderer component.");
        }
    }

    private void LerpWeaponSpriteScale(SpriteRenderer spriteRenderer, float duration)
    {
        if (spriteRenderer == null)
        {
            Debug.LogWarning("SpriteRenderer is null. Cannot lerp scale.");
            return;
        }
        StartCoroutine(LerpCoroutine(spriteRenderer, duration));
    }

    private IEnumerator LerpCoroutine(SpriteRenderer spriteRenderer, float duration)
    {
        spriteRenderer.transform.localScale = initialSpriteScale;

        float elapsedTime = 0f;
        while (elapsedTime < duration)
        {
            float t = elapsedTime / duration;
            spriteRenderer.transform.localScale = Vector3.Lerp(initialSpriteScale, ORIGINAL_SPRITE_SCALE, t);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        spriteRenderer.transform.localScale = ORIGINAL_SPRITE_SCALE;
    }
}
