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
