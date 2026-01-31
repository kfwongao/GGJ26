using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;
using UnityEngine.SceneManagement;
using System.IO;
using UnityEngine.Localization.Settings;

public class StartMenuManager : MonoBehaviour
{
    public Button StartButton;
    public Button SettingButton;
    public GameObject SettingMenuGO;
    public Button SettingCloseButton;
    public VideoPlayer videoPlayer;
    public AudioSource BGMusic;
    public AudioSource SceneSound;

    public Slider MusicSilder;
    public Slider SoundSlider;

    public Toggle TipsToggle;
    public Toggle VibrationToggle;

    // Graphic
    public Toggle LowToggle;
    public Toggle MediumToggle;
    public Toggle HighToggle;

    //Difficulty
    public Toggle EasyToggle;
    public Toggle NormalToggle;
    public Toggle HardToggle;

    //Language Localization
    public Toggle ZH_CN_Toggle;
    public Toggle EN_US_Toggle;

    // Start is called before the first frame update
    void Start()
    {
        Application.targetFrameRate = 60;
        LoadSettingDataUI();
    }


    // Update is called once per frame
    void Update()
    {
        
    }

    public void LoadSettingDataUI()
    {
        GameSettingDataSingleton.Instance.LoadData();
        MusicSilder.value = GameSettingDataSingleton.Instance.musicVolume;
        BGMusic.volume = GameSettingDataSingleton.Instance.musicVolume;
        SoundSlider.value = GameSettingDataSingleton.Instance.soundVolume;
        SceneSound.volume = GameSettingDataSingleton.Instance.soundVolume;
        TipsToggle.isOn = GameSettingDataSingleton.Instance.isTipsOn;
        VibrationToggle.isOn = GameSettingDataSingleton.Instance.isVibrationOn;
        switch(GameSettingDataSingleton.Instance.GraphicIndex)
        {
            case 0:
                LowToggle.isOn = true;
                MediumToggle.isOn = false;
                HighToggle.isOn = false;
                break;
            case 1:
                LowToggle.isOn = false;
                MediumToggle.isOn = true;
                HighToggle.isOn = false;
                break;
            case 2:
                LowToggle.isOn = false;
                MediumToggle.isOn = false;
                HighToggle.isOn = true;
                break;
        }
        switch (GameSettingDataSingleton.Instance.DifficultyIndex)
        {
            case 0:
                EasyToggle.isOn = true;
                NormalToggle.isOn = false;
                HardToggle.isOn = false;
                break;
            case 1:
                EasyToggle.isOn = false;
                NormalToggle.isOn = true;
                HardToggle.isOn = false;
                break;
            case 2:
                EasyToggle.isOn = false;
                NormalToggle.isOn = false;
                HardToggle.isOn = true;
                break;
        }
        switch (GameSettingDataSingleton.Instance.localization_index)
        {
            case 0:
                EN_US_Toggle.isOn = true;
                ZH_CN_Toggle.isOn = false;
                break;
            case 1:
                EN_US_Toggle.isOn = false;
                ZH_CN_Toggle.isOn = true;
                break;
            case 2:
                EN_US_Toggle.isOn = false;
                ZH_CN_Toggle.isOn = true;
                break;
        }
        //GameSettingDataSingleton.Instance.selectLanguage(GameSettingDataSingleton.Instance.localization_index);
    }

    public void OnMusicSilderChange(float value)
    {

        GameSettingDataSingleton.Instance.musicVolume = value;
        BGMusic.volume = value;
        GameSettingDataSingleton.Instance.SaveData();
    }

    public void OnSoundSilderChange(float value)
    {
        GameSettingDataSingleton.Instance.soundVolume = value;
        SceneSound.volume = value;
        GameSettingDataSingleton.Instance.SaveData();
    }

    public void OnTipsToggleChange(bool isTipsOn)
    {
        GameSettingDataSingleton.Instance.isTipsOn = isTipsOn;
        GameSettingDataSingleton.Instance.SaveData();
    }

    public void OnVibrationToggleChange(bool isVibrationOn)
    {
        GameSettingDataSingleton.Instance.isVibrationOn = isVibrationOn;
        GameSettingDataSingleton.Instance.SaveData();
    }

    public void OnLowToggleChange(bool isTrue)
    {
        if (isTrue)
        {
            GameSettingDataSingleton.Instance.GraphicIndex = 0;
            GameSettingDataSingleton.Instance.SaveData();
        }
    }

    public void OnMediumToggleChange(bool isTrue)
    {
        if (isTrue)
        {
            GameSettingDataSingleton.Instance.GraphicIndex = 1;
            GameSettingDataSingleton.Instance.SaveData();
        }
    }

    public void OnHighToggleChange(bool isTrue)
    {
        if (isTrue)
        {
            GameSettingDataSingleton.Instance.GraphicIndex = 2;
            GameSettingDataSingleton.Instance.SaveData();
        }
    }

    public void OnEasyToggleChange(bool isTrue)
    {
        if (isTrue)
        {
            GameSettingDataSingleton.Instance.DifficultyIndex = 0;
            GameSettingDataSingleton.Instance.SaveData();
        }
    }

    public void OnNormalToggleChange(bool isTrue)
    {
        if (isTrue)
        {
            GameSettingDataSingleton.Instance.DifficultyIndex = 1;
            GameSettingDataSingleton.Instance.SaveData();
        }
    }

    public void OnHardToggleChange(bool isTrue)
    {
        if (isTrue)
        {
            GameSettingDataSingleton.Instance.DifficultyIndex = 2;
            GameSettingDataSingleton.Instance.SaveData();
        }
    }

    public void OnZH_CN_ToggleChange(bool isTrue)
    {
        if (isTrue)
        {
            Debug.Log("OnZH_CN_ToggleChange");
            GameSettingDataSingleton.Instance.localization_index = 1;
            GameSettingDataSingleton.Instance.selectLanguage(1);
            GameSettingDataSingleton.Instance.SaveData();
        }
    }

    public void OnEN_US_ToggleChange(bool isTrue)
    {
        if (isTrue)
        {
            Debug.Log("OnEN_US_ToggleChange");
            GameSettingDataSingleton.Instance.localization_index = 0;
            GameSettingDataSingleton.Instance.selectLanguage(0);
            GameSettingDataSingleton.Instance.SaveData();
        }
    }


    public void onStartButtonClick()
    {
        MapsDataSingleton.Instance.MapName = "level_1";
        MapsDataSingleton.Instance.LocationAreaName = "level_1";
        initSceneManager.Instance.InitScene("level_1");
    }

    public void onStartNewGameButtonClick()
    {
        PlayerData.Instance.ClearData();
        File.Delete(Path.Combine(Application.persistentDataPath, "inventory.txt"));
        MapsDataSingleton.Instance.MapName = "level_1";
        MapsDataSingleton.Instance.LocationAreaName = "level_1";
        initSceneManager.Instance.InitScene("level_1");
    }

    public void onQuitGameButtonClick()
    {
#if UNITY_EDITOR
        EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
