[System.Serializable]
public class OptionsModel
{
    public float volume = 1f;
    public float brightness = 1f;

    private const string VolumeKey = "OPTIONS_VOLUME";
    private const string BrightnessKey = "OPTIONS_BRIGHTNESS";

    public void Load()
    {
        volume = UnityEngine.PlayerPrefs.GetFloat(VolumeKey, 1f);
        brightness = UnityEngine.PlayerPrefs.GetFloat(BrightnessKey, 1f);
    }

    public void Save()
    {
        UnityEngine.PlayerPrefs.SetFloat(VolumeKey, volume);
        UnityEngine.PlayerPrefs.SetFloat(BrightnessKey, brightness);
        UnityEngine.PlayerPrefs.Save();
    }
}