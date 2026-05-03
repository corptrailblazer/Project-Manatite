using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;
using TMPro;
using System.Collections.Generic;
using System.Linq;

public class SettingsMenu : MonoBehaviour
{
    [Header("Audio")]
    public AudioMixer mainMixer;
    public Slider musicSlider;
    public Slider sfxSlider;

    [Header("Video")]
    public Dropdown resolutionDropdown;
    public Toggle fullscreenToggle;

    private Resolution[] resolutions;

    void Start()
    {
        // Load Audio Settings
        float musicVol = PlayerPrefs.GetFloat("MusicVolume", 0.75f);
        float sfxVol = PlayerPrefs.GetFloat("SFXVolume", 0.75f);
        
        if (musicSlider != null)
        {
            musicSlider.value = musicVol;
            musicSlider.onValueChanged.AddListener(SetMusicVolume);
        }
        
        if (sfxSlider != null)
        {
            sfxSlider.value = sfxVol;
            sfxSlider.onValueChanged.AddListener(SetSFXVolume);
        }

        // Apply immediately
        SetMusicVolume(musicVol);
        SetSFXVolume(sfxVol);

        // Load Video Settings
        if (fullscreenToggle != null)
        {
            bool isFullscreen = PlayerPrefs.GetInt("FullscreenPreference", Screen.fullScreen ? 1 : 0) == 1;
            fullscreenToggle.isOn = isFullscreen;
            fullscreenToggle.onValueChanged.AddListener(SetFullscreen);
            Screen.fullScreen = isFullscreen;
        }

        if (resolutionDropdown != null)
        {
            SetupResolutionDropdown();
            resolutionDropdown.onValueChanged.AddListener(SetResolution);
        }
    }

    private void SetupResolutionDropdown()
    {
        resolutions = Screen.resolutions;
        resolutionDropdown.ClearOptions();

        List<string> options = new List<string>();
        int currentResolutionIndex = PlayerPrefs.GetInt("ResolutionPreference", -1);
        int fallbackIndex = 0;

        for (int i = 0; i < resolutions.Length; i++)
        {
            string option = resolutions[i].width + " x " + resolutions[i].height + " @ " + Mathf.Round((float)resolutions[i].refreshRateRatio.value) + "Hz";
            options.Add(option);

            if (resolutions[i].width == Screen.currentResolution.width && 
                resolutions[i].height == Screen.currentResolution.height &&
                Mathf.Approximately((float)resolutions[i].refreshRateRatio.value, (float)Screen.currentResolution.refreshRateRatio.value))
            {
                fallbackIndex = i;
            }
        }

        resolutionDropdown.AddOptions(options);

        if (currentResolutionIndex == -1 || currentResolutionIndex >= resolutions.Length)
        {
            currentResolutionIndex = fallbackIndex;
        }

        resolutionDropdown.value = currentResolutionIndex;
        resolutionDropdown.RefreshShownValue();
    }

    public void SetMusicVolume(float volume)
    {
        if (mainMixer != null)
        {
            float dbVolume = volume > 0.001f ? Mathf.Log10(volume) * 20f : -80f;
            mainMixer.SetFloat("MusicVol", dbVolume);
        }
        PlayerPrefs.SetFloat("MusicVolume", volume);
    }

    public void SetSFXVolume(float volume)
    {
        if (mainMixer != null)
        {
            float dbVolume = volume > 0.001f ? Mathf.Log10(volume) * 20f : -80f;
            mainMixer.SetFloat("SFXVol", dbVolume);
        }
        PlayerPrefs.SetFloat("SFXVolume", volume);
    }

    public void SetFullscreen(bool isFullscreen)
    {
        Screen.fullScreen = isFullscreen;
        PlayerPrefs.SetInt("FullscreenPreference", isFullscreen ? 1 : 0);
    }

    public void SetResolution(int resolutionIndex)
    {
        if (resolutions == null || resolutionIndex < 0 || resolutionIndex >= resolutions.Length) return;
        
        Resolution resolution = resolutions[resolutionIndex];
        Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);
        PlayerPrefs.SetInt("ResolutionPreference", resolutionIndex);
    }

    public void CloseSettings()
    {
        gameObject.SetActive(false);
    }
}
