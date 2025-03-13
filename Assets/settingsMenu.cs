using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Audio;

public class settingsMenu : MonoBehaviour
{
    public AudioMixer audioMixer;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
   public void setVolume(float volume)
    {
        audioMixer.SetFloat("volume", volume);
    }

    public void SetQualityLevel(int qualityIndex)
    {
        QualitySettings.SetQualityLevel(qualityIndex);
    }

    public void SetFullScreen(bool isFullScreen)
    {
        Screen.fullScreen = isFullScreen;
    }
}
