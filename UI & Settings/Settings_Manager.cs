using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

//Simple settings management script: Audio volume, Resolutions and Graphics Quality settings, it uses PlayerPrefs to perform this tasks.
public class Settings_Manager : MonoBehaviour
{
    
    public Slider volumeSlider;
    public Dropdown resDropdown;

    public SharedVariables settingsSharVar;
    public AudioMixer audioMixer;

    Resolution[] _resolutions;
    

    private void Start()
    {
        volumeSlider.value = settingsSharVar._volumeSlider;

        _resolutions = Screen.resolutions;

        resDropdown.ClearOptions();

        List<string> _resOptions = new List<string>();

        int _currentResIndex = 0;

        for (int i = 0; i < _resolutions.Length; i++)
        {
            string option = _resolutions[i].width + " x " + _resolutions[i].height;
            _resOptions.Add(option);

            if (_resolutions[i].width == Screen.currentResolution.width &&
                _resolutions[i].height == Screen.currentResolution.height)
            {
                _currentResIndex = i;
            }
        }

        resDropdown.AddOptions(_resOptions);
        resDropdown.value = _currentResIndex;
        resDropdown.RefreshShownValue();
    }

    public void SetVolume(float _volume)
    {
        audioMixer.SetFloat("Volume", _volume);
        settingsSharVar._volumeSlider = volumeSlider.value;
    }

    public void OnSaveButtonClick()
    {
        PlayerPrefs.SetFloat("Volume", volumeSlider.value);
    }

    public void SetGraphics(int graphicsIndex)
    {
        QualitySettings.SetQualityLevel(graphicsIndex);
    }

    public void SetFullScreen(bool _isFullscreen)
    {
        Screen.fullScreen = _isFullscreen;
    }

    public void SetResolution(int _resIndex)
    {
        Resolution _resolution = _resolutions[_resIndex];
        Screen.SetResolution(_resolution.width, _resolution.height, Screen.fullScreen);
    }
}
