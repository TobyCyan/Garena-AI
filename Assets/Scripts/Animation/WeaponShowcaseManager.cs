using UnityEngine;
using System.Collections;
using System;

public class WeaponShowcaseManager : MonoBehaviour
{
    [SerializeField] private ParticleSystem burstEffect;

    // Particle System Settings
    private const int SEED = 12345;

    // Weapon Sprite Animation Settings
    [SerializeField] private float lerpDuration = 0.25f;
    [SerializeField] private Vector3 initialSpriteScale = new(1.5f, 1.5f, 1f);
    [SerializeField] private int showcaseSortingOrder = 100;
    private static readonly Vector3 ORIGINAL_SPRITE_SCALE = Vector3.one;

    public void Init()
    {
        if (burstEffect != null)
        {
            burstEffect.Stop();
            burstEffect.useAutoRandomSeed = false;
            burstEffect.randomSeed = SEED;
        }
    }

    public void ShowcaseWeapon(GameObject weaponGo, Action onComplete = null)
    {
        if (weaponGo == null)
        {
            Debug.LogWarning("Weapon GameObject is null. Cannot showcase.");
            onComplete?.Invoke();
            return;
        }

        weaponGo.SetActive(true);
        weaponGo.transform.SetParent(transform, false);
        weaponGo.transform.localPosition = Vector3.zero;

        SpriteRenderer spriteRenderer =
            weaponGo.GetComponentInChildren<SpriteRenderer>(true);
        Player.Weapons.WeaponSwingAnimator swingAnimator =
            weaponGo.GetComponent<Player.Weapons.WeaponSwingAnimator>();
        swingAnimator?.SetKeepVisible(true);

        if (spriteRenderer != null)
        {
            spriteRenderer.enabled = true;
            spriteRenderer.gameObject.SetActive(true);
            spriteRenderer.transform.localPosition = Vector3.zero;
            ApplyShowcaseSorting(spriteRenderer);
            LerpWeaponSpriteScale(spriteRenderer, lerpDuration, onComplete);
        }
        else
        {
            Debug.LogWarning("Weapon GameObject does not have a SpriteRenderer component.");
            onComplete?.Invoke();
        }

        if (burstEffect != null)
        {
            burstEffect.Play();
        }
    }

    private void ApplyShowcaseSorting(SpriteRenderer spriteRenderer)
    {
        if (spriteRenderer == null)
        {
            return;
        }

        spriteRenderer.sortingOrder = showcaseSortingOrder;
    }

    private void LerpWeaponSpriteScale(
        SpriteRenderer spriteRenderer,
        float duration,
        Action onComplete)
    {
        if (spriteRenderer == null)
        {
            Debug.LogWarning("SpriteRenderer is null. Cannot lerp scale.");
            onComplete?.Invoke();
            return;
        }
        StartCoroutine(LerpCoroutine(spriteRenderer, duration, onComplete));
    }

    private IEnumerator LerpCoroutine(
        SpriteRenderer spriteRenderer,
        float duration,
        Action onComplete)
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
        onComplete?.Invoke();
    }
}
