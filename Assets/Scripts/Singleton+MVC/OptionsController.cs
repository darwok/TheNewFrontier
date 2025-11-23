using UnityEngine;

public class OptionsController : MonoBehaviour
{
    [SerializeField] private OptionsView view;
    [SerializeField] private CanvasGroup brightnessOverlay; // Black overlay to simulate brightness adjustment, useful for brightness feedback simulation

    private OptionsModel model = new OptionsModel();

    private void Awake()
    {
        model.Load();

        if (view != null)
        {
            view.Bind(model.volume, model.brightness);

            if (view.VolumeSlider != null)
                view.VolumeSlider.onValueChanged.AddListener(OnVolumeChanged);

            if (view.BrightnessSlider != null)
                view.BrightnessSlider.onValueChanged.AddListener(OnBrightnessChanged);
        }

        ApplyVolume(model.volume);
        ApplyBrightness(model.brightness);
    }

    private void OnDestroy()
    {
        if (view != null)
        {
            if (view.VolumeSlider != null)
                view.VolumeSlider.onValueChanged.RemoveListener(OnVolumeChanged);

            if (view.BrightnessSlider != null)
                view.BrightnessSlider.onValueChanged.RemoveListener(OnBrightnessChanged);
        }
    }

    private void OnVolumeChanged(float value)
    {
        model.volume = value;
        ApplyVolume(value);
        model.Save();
    }

    private void OnBrightnessChanged(float value)
    {
        model.brightness = value;
        ApplyBrightness(value);
        model.Save();
    }

    private void ApplyVolume(float value)
    {
        if (AudioManager.Instance != null)
            AudioManager.Instance.SetMasterVolume(value);
    }

    private void ApplyBrightness(float value)
    {
        if (brightnessOverlay == null) return;

        // 1 = normal, 0 = black screen
        float alpha = Mathf.Clamp01(1f - value);
        brightnessOverlay.alpha = alpha;
    }

    // Called from UI Button "Back"
    public void CloseOptions()
    {
        model.Save();
    }
}