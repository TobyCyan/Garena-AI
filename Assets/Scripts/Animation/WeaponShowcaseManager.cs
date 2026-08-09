using UnityEngine;
using System.Collections;
using System;
using UnityEngine.UI;

public class WeaponShowcaseManager : MonoBehaviour
{
    [SerializeField] private ParticleSystem burstEffect;
    [SerializeField] private Button confirmButton;
    [SerializeField] private Button retryButton;
    [SerializeField] private Image weaponImage;

    // Particle System Settings
    private const int SEED = 12345;

    // Weapon Sprite Animation Settings
    [SerializeField] private float lerpDuration = 0.25f;
    [SerializeField] private Vector2 initialSpriteScale = new(600f, 600f);
    private static readonly Vector2 ORIGINAL_SPRITE_SCALE = new(500f, 500f);
    public Action OnWeaponConfirmed;
    public Action OnWeaponRetry;

    public void Init()
    {
        if (burstEffect != null)
        {
            burstEffect.Stop();
            burstEffect.useAutoRandomSeed = false;
            burstEffect.randomSeed = SEED;
        }

        if (weaponImage != null)
        {
            weaponImage.gameObject.SetActive(false);
        }

        if (confirmButton != null)
        {
            confirmButton.onClick.AddListener(OnConfirmButtonClicked);
            SetConfirmButtonActive(false);
        }

        if (retryButton != null)
        {
            retryButton.onClick.AddListener(OnRetryButtonClicked);
            SetRetryButtonActive(false);
        }
    }

    public void ShowcaseWeapon(Sprite weaponSprite)
    {
        SetConfirmButtonActive(false);

        if (weaponSprite == null)
        {
            Debug.LogWarning("Weapon Sprite is null. Cannot showcase.");
            return;
        }

        if (weaponImage != null)
        {
            weaponImage.sprite = weaponSprite;
            weaponImage.gameObject.SetActive(true);
            LerpWeaponSpriteScale(weaponImage.GetComponent<RectTransform>(), lerpDuration);
        }
        else
        {
            Debug.LogWarning("Weapon GameObject does not have a SpriteRenderer component.");
        }

        SetConfirmButtonActive(true);
        SetRetryButtonActive(true);

        SoundEffectManager.Play("generationcomplete");
        if (burstEffect != null)
        {
            burstEffect.Play();
        }
    }

    private void LerpWeaponSpriteScale(
        RectTransform rect,
        float duration)
    {
        if (rect == null)
        {
            Debug.LogWarning("RectTransform is null. Cannot lerp scale.");
            return;
        }
        StartCoroutine(LerpCoroutine(rect, duration));
    }

    private IEnumerator LerpCoroutine(
        RectTransform rect,
        float duration)
    {
        rect.sizeDelta = initialSpriteScale;
        
        float elapsedTime = 0f;
        while (elapsedTime < duration)
        {
            float t = elapsedTime / duration;
            rect.sizeDelta = Vector3.Lerp(initialSpriteScale, ORIGINAL_SPRITE_SCALE, t);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        rect.sizeDelta = ORIGINAL_SPRITE_SCALE;
    }

    private void OnConfirmButtonClicked()
    {
        SoundEffectManager.Play("buttonclick");
        SetConfirmButtonActive(false);
        SetRetryButtonActive(false);
        SetWeaponImageActive(false);
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

    private void OnRetryButtonClicked()
    {
        SoundEffectManager.Play("buttonclick");
        SetConfirmButtonActive(false);
        SetRetryButtonActive(false);
        SetWeaponImageActive(false);
        OnWeaponRetry?.Invoke();
    }

    private void SetRetryButtonActive(bool active)
    {
        if (retryButton != null)
        {
            retryButton.interactable = active;
            retryButton.gameObject.SetActive(active);
        }
    }

    private void SetWeaponImageActive(bool active)
    {
        if (weaponImage != null)
        {
            weaponImage.gameObject.SetActive(active);
        }
    }

    private void OnDestroy()
    {
        if (confirmButton != null)
        {
            confirmButton.onClick.RemoveListener(OnConfirmButtonClicked);
        }

        if (retryButton != null)
        {
            retryButton.onClick.RemoveListener(OnRetryButtonClicked);
        }

        OnWeaponConfirmed = null;
        OnWeaponRetry = null;
    }
}
