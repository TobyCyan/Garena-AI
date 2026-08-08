using UnityEngine;
using UnityEngine.UI;

public class ProgressBar : MonoBehaviour
{
    [SerializeField] private Slider slider;

    public float NormalizedValue => slider != null ? slider.normalizedValue : 0f;

    private void Awake()
    {
        ResolveSlider();
    }

    public void UpdateFill(float progress, float target)
    {
        ResolveSlider();
        if (slider == null)
        {
            Debug.LogError("ProgressBar needs a Slider component.", this);
            return;
        }

        float newFillAmount = target > 0f
            ? Mathf.Clamp01(progress / target)
            : 0f;
        slider.SetValueWithoutNotify(newFillAmount);

        if (slider.fillRect != null &&
            slider.fillRect.TryGetComponent(out Image fillImage))
        {
            slider.fillRect.anchorMin = Vector2.zero;
            slider.fillRect.anchorMax = Vector2.one;
            slider.fillRect.offsetMin = Vector2.zero;
            slider.fillRect.offsetMax = Vector2.zero;
            fillImage.type = Image.Type.Filled;
            fillImage.fillMethod = Image.FillMethod.Horizontal;
            fillImage.fillOrigin = (int)Image.OriginHorizontal.Left;
            fillImage.fillAmount = newFillAmount;
            fillImage.SetVerticesDirty();
        }
    }

    public void ResetFill()
    {
        UpdateFill(0f, 1f);
    }

    public void ShowBar()
    {
        ResolveSlider();
        slider?.gameObject.SetActive(true);
    }

    public void HideBar()
    {
        ResolveSlider();
        slider?.gameObject.SetActive(false);
    }

    private void ResolveSlider()
    {
        slider ??= GetComponent<Slider>();
    }
}
