using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;
using TMPro;
using ProjectManatite.Core;

public class SettingsMenu : MonoBehaviour
{
    [Header("Audio — Sliders")]
    public AudioMixer mainMixer;
    public Slider masterSlider;
    public Slider musicSlider;
    public Slider sfxSlider;

    [Header("Audio — Labels")]
    public TMP_Text masterLabel;
    public TMP_Text musicLabel;
    public TMP_Text sfxLabel;

    [Header("Audio — Event Channels")]
    public FloatEventChannelSO masterVolumeChannel;
    public FloatEventChannelSO musicVolumeChannel;
    public FloatEventChannelSO sfxVolumeChannel;

    [Header("Video")]
    public Dropdown resolutionDropdown;
    public Toggle fullscreenToggle;
    public Toggle vsyncToggle;

    [Header("Resolution Revert")]
    public CanvasGroup settingsContentGroup;
    public GameObject revertBanner;
    public TMP_Text countdownLabel;
    public GameConfigSO config;

    [Header("Events")]
    public VoidEventChannelSO settingsAppliedChannel;

    private Resolution[] _resolutions;
    private int _previousResolutionIndex;
    private int _pendingResolutionIndex;
    private Coroutine _revertCoroutine;

    private void Awake()
    {
        CanvasGroup cg = GetComponent<CanvasGroup>();
        if (cg == null) cg = gameObject.AddComponent<CanvasGroup>();
        cg.ignoreParentGroups = true;
    }

    private void Start()
    {
        float master = PlayerPrefs.GetFloat("MasterVolume", 1f);
        float music  = PlayerPrefs.GetFloat("MusicVolume", 0.75f);
        float sfx    = PlayerPrefs.GetFloat("SFXVolume", 0.75f);

        if (masterSlider != null) { masterSlider.value = master; masterSlider.onValueChanged.AddListener(OnMasterChanged); }
        if (musicSlider  != null) { musicSlider.value  = music;  musicSlider.onValueChanged.AddListener(OnMusicChanged); }
        if (sfxSlider    != null) { sfxSlider.value    = sfx;    sfxSlider.onValueChanged.AddListener(OnSFXChanged); }

        ApplyVolumes(master, music, sfx);

        if (fullscreenToggle != null)
        {
            bool isFullscreen = PlayerPrefs.GetInt("FullscreenPreference", Screen.fullScreen ? 1 : 0) == 1;
            fullscreenToggle.isOn = isFullscreen;
            fullscreenToggle.onValueChanged.AddListener(SetFullscreen);
            Screen.fullScreen = isFullscreen;
        }

        if (vsyncToggle != null)
        {
            bool vsync = PlayerPrefs.GetInt("VSyncPreference", 1) == 1;
            vsyncToggle.isOn = vsync;
            vsyncToggle.onValueChanged.AddListener(SetVSync);
            QualitySettings.vSyncCount = vsync ? 1 : 0;
        }

        if (resolutionDropdown != null)
        {
            SetupResolutionDropdown();
            resolutionDropdown.onValueChanged.AddListener(OnResolutionChanged);
        }

        if (revertBanner != null) revertBanner.SetActive(false);
    }

    // ── Audio ─────────────────────────────────────────────────────────────────

    private void OnMasterChanged(float value)
    {
        PlayerPrefs.SetFloat("MasterVolume", value);
        ApplyVolumes(value, SliderVal(musicSlider, 0.75f), SliderVal(sfxSlider, 0.75f));
    }

    private void OnMusicChanged(float value)
    {
        PlayerPrefs.SetFloat("MusicVolume", value);
        ApplyVolumes(SliderVal(masterSlider, 1f), value, SliderVal(sfxSlider, 0.75f));
    }

    private void OnSFXChanged(float value)
    {
        PlayerPrefs.SetFloat("SFXVolume", value);
        ApplyVolumes(SliderVal(masterSlider, 1f), SliderVal(musicSlider, 0.75f), value);
    }

    // Computes effective volumes (master * channel), updates mixer, labels, and event channels.
    private void ApplyVolumes(float master, float music, float sfx)
    {
        SetMixerDb("MusicVol", master * music);
        SetMixerDb("SFXVol",   master * sfx);

        if (masterLabel != null) masterLabel.text = Mathf.RoundToInt(master * 100f) + "%";
        if (musicLabel  != null) musicLabel.text  = Mathf.RoundToInt(music  * 100f) + "%";
        if (sfxLabel    != null) sfxLabel.text    = Mathf.RoundToInt(sfx    * 100f) + "%";

        masterVolumeChannel?.Raise(master);
        musicVolumeChannel?.Raise(master * music);
        sfxVolumeChannel?.Raise(master * sfx);
    }

    private void SetMixerDb(string param, float linear)
    {
        if (mainMixer == null) return;
        mainMixer.SetFloat(param, linear > 0.001f ? Mathf.Log10(linear) * 20f : -80f);
    }

    // ── Video ─────────────────────────────────────────────────────────────────

    public void SetFullscreen(bool isFullscreen)
    {
        Screen.fullScreen = isFullscreen;
        PlayerPrefs.SetInt("FullscreenPreference", isFullscreen ? 1 : 0);
    }

    public void SetVSync(bool enabled)
    {
        QualitySettings.vSyncCount = enabled ? 1 : 0;
        PlayerPrefs.SetInt("VSyncPreference", enabled ? 1 : 0);
    }

    // ── Resolution ────────────────────────────────────────────────────────────

    private void SetupResolutionDropdown()
    {
        _resolutions = Screen.resolutions;
        resolutionDropdown.ClearOptions();

        var options = new List<string>();
        int savedIndex   = PlayerPrefs.GetInt("ResolutionPreference", -1);
        int fallbackIndex = 0;

        for (int i = 0; i < _resolutions.Length; i++)
        {
            options.Add(_resolutions[i].width + " x " + _resolutions[i].height +
                        " @ " + Mathf.Round((float)_resolutions[i].refreshRateRatio.value) + "Hz");

            if (_resolutions[i].width  == Screen.currentResolution.width &&
                _resolutions[i].height == Screen.currentResolution.height &&
                Mathf.Approximately((float)_resolutions[i].refreshRateRatio.value,
                                    (float)Screen.currentResolution.refreshRateRatio.value))
                fallbackIndex = i;
        }

        resolutionDropdown.AddOptions(options);

        if (savedIndex < 0 || savedIndex >= _resolutions.Length) savedIndex = fallbackIndex;

        _previousResolutionIndex = savedIndex;
        _pendingResolutionIndex  = savedIndex;
        resolutionDropdown.value = savedIndex;
        resolutionDropdown.RefreshShownValue();
    }

    private void OnResolutionChanged(int index)
    {
        if (_resolutions == null || index < 0 || index >= _resolutions.Length) return;
        if (index == _previousResolutionIndex) return;

        _pendingResolutionIndex = index;
        Resolution res = _resolutions[index];
        Screen.SetResolution(res.width, res.height, Screen.fullScreen);

        if (_revertCoroutine != null) StopCoroutine(_revertCoroutine);
        _revertCoroutine = StartCoroutine(RevertCountdown());
    }

    private IEnumerator RevertCountdown()
    {
        LockPanel(true);
        float duration = config != null ? config.ResolutionRevertDuration : 15f;
        int remaining  = Mathf.RoundToInt(duration);

        while (remaining > 0)
        {
            if (countdownLabel != null)
                countdownLabel.text = $"Keep this resolution? Reverting in {remaining}s…";
            yield return WaitCache.Seconds(1f);
            remaining--;
        }

        RevertResolution();
    }

    // Called by the [Keep] button in the revert banner.
    public void KeepResolution()
    {
        if (_revertCoroutine != null) { StopCoroutine(_revertCoroutine); _revertCoroutine = null; }
        _previousResolutionIndex = _pendingResolutionIndex;
        PlayerPrefs.SetInt("ResolutionPreference", _pendingResolutionIndex);
        settingsAppliedChannel?.Raise();
        LockPanel(false);
    }

    // Called by the [Revert] button and on countdown expiry.
    public void RevertResolution()
    {
        if (_revertCoroutine != null) { StopCoroutine(_revertCoroutine); _revertCoroutine = null; }

        Resolution res = _resolutions[_previousResolutionIndex];
        Screen.SetResolution(res.width, res.height, Screen.fullScreen);

        // Suppress OnResolutionChanged re-trigger while restoring the dropdown value.
        resolutionDropdown.onValueChanged.RemoveListener(OnResolutionChanged);
        resolutionDropdown.value = _previousResolutionIndex;
        resolutionDropdown.onValueChanged.AddListener(OnResolutionChanged);

        LockPanel(false);
    }

    private void LockPanel(bool locked)
    {
        if (revertBanner != null) revertBanner.SetActive(locked);

        if (settingsContentGroup != null)
        {
            settingsContentGroup.interactable = !locked;
            settingsContentGroup.alpha         = locked ? 0.5f : 1f;
        }
    }

    // ── Panel Lifecycle ───────────────────────────────────────────────────────

    public void Open()   => gameObject.SetActive(true);
    public void Toggle() { if (gameObject.activeSelf) CloseSettings(); else Open(); }

    public void CloseSettings()
    {
        if (_revertCoroutine != null) RevertResolution();
        gameObject.SetActive(false);
    }

    // ── Helpers ───────────────────────────────────────────────────────────────

    private static float SliderVal(Slider s, float fallback) => s != null ? s.value : fallback;
}
