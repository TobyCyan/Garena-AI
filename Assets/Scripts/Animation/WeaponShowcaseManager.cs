using UnityEngine;
using System.Collections;
using System;
using UnityEngine.UI;

public class WeaponShowcaseManager : MonoBehaviour
{
    [SerializeField] private ParticleSystem burstEffect;
    [SerializeField] private Button confirmButton;

    // Particle System Settings
    private const int SEED = 12345;

    // Weapon Sprite Animation Settings
    [SerializeField] private float lerpDuration = 0.25f;
    [SerializeField] private Vector3 initialSpriteScale = new(1.5f, 1.5f, 1f);
    [SerializeField] private int showcaseSortingOrder = 100;
    private static readonly Vector3 ORIGINAL_SPRITE_SCALE = Vector3.one;
    public Action OnWeaponConfirmed;

    public void Init()
    {
        if (burstEffect != null)
        {
            burstEffect.Stop();
            burstEffect.useAutoRandomSeed = false;
            burstEffect.randomSeed = SEED;
        }

        if (confirmButton != null)
        {
            confirmButton.onClick.AddListener(OnConfirmButtonClicked);
            confirmButton.interactable = false;
            confirmButton.gameObject.SetActive(false);
        }
    }

    public void ShowcaseWeapon(GameObject weaponGo)
    {
        SetConfirmButtonActive(false);

        if (weaponGo == null)
        {
            Debug.LogWarning("Weapon GameObject is null. Cannot showcase.");
            return;
        }

        weaponGo.SetActive(true);
        weaponGo.transform.SetParent(transform, false);
        weaponGo.transform.localPosition = Vector3.zero;

        SpriteRenderer spriteRenderer =
            weaponGo.GetComponentInChildren<SpriteRenderer>(true);
        Player.Weapons.MeleeWeaponSwingAnimator swingAnimator =
            weaponGo.GetComponent<Player.Weapons.MeleeWeaponSwingAnimator>();
        swingAnimator?.SetKeepVisible(true);

        if (spriteRenderer != null)
        {
            spriteRenderer.enabled = true;
            spriteRenderer.gameObject.SetActive(true);
            spriteRenderer.transform.localPosition = Vector3.zero;
            ApplyShowcaseSorting(spriteRenderer);
            LerpWeaponSpriteScale(spriteRenderer, lerpDuration);
        }
        else
        {
            Debug.LogWarning("Weapon GameObject does not have a SpriteRenderer component.");
        }

        SetConfirmButtonActive(true);

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
        float duration)
    {
        if (spriteRenderer == null)
        {
            Debug.LogWarning("SpriteRenderer is null. Cannot lerp scale.");
            return;
        }
        StartCoroutine(LerpCoroutine(spriteRenderer, duration));
    }

    private IEnumerator LerpCoroutine(
        SpriteRenderer spriteRenderer,
        float duration)
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

    private void OnConfirmButtonClicked()
    {
        SetConfirmButtonActive(false);
        OnWeaponConfirmed?.Invoke();
    }

    private void SetConfirmButtonActive(bool active)
    {
        if (confirmButton != null)
        {
            confirmButton.interactable = active;
            confirmButton.gameObject.SetActive(active);
        }
    }

    private void OnDestroy()
    {
        if (confirmButton != null)
        {
            confirmButton.onClick.RemoveListener(OnConfirmButtonClicked);
        }
    }
}
