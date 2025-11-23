using UnityEngine;
using UnityEngine.UI;

public class OptionsView : MonoBehaviour
{
    [SerializeField] private Slider volumeSlider;
    [SerializeField] private Slider brightnessSlider;

    public Slider VolumeSlider => volumeSlider;
    public Slider BrightnessSlider => brightnessSlider;

    public void Bind(float volume, float brightness)
    {
        if (volumeSlider != null) volumeSlider.value = volume;
        if (brightnessSlider != null) brightnessSlider.value = brightness;
    }
}