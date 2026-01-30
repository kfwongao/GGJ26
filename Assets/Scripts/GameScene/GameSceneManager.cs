using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using TMPro;
using UnityEngine.UI;
using UI_Inputs;
using UnityEngine.Localization.Settings;

public class GameSceneManager : MonoBehaviour
{
    public GameObject ConfirmDialogGO;
    public AudioSource BGMusic;
    public AudioSource SceneSound;
    public GameObject PlayerGO;
    public TextMeshProUGUI MapNameText;
    public TextMeshProUGUI PlayerLocationText;

    public Button SettingCloseButton;

    public Slider MusicSilder;
    public Slider SoundSlider;

    public Toggle TipsToggle;
    public Toggle VibrationToggle;

    // Graphic
    public Toggle LowToggle;
    public Toggle MediumToggle;
    public Toggle HighToggle;

    //Language Localization
    public Toggle ZH_CN_Toggle;
    public Toggle EN_US_Toggle;

    public static GameSceneManager Instance;
    public GameObject UIPrefabsSpawnHolder;
    public GameObject LocationChangeTips;

    //
    public Slider LookSpeedSilder;

    //
    public Transform RenderTextureCharacter;

    // PC and JoyStick Control Switch Button
    public Button PC_or_JoyStick_SwitchButton;
    public Image PC_or_JoyStick_Image;
    public Sprite PC_Icon;
    public Sprite MobilePhone_Icon;
    public GameObject JoyStickGO;
    public GameObject PCControlGO;

    private void Awake()
    {
        Instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        Application.targetFrameRate = 60;
        LoadSettingDataUI();
        MusicSilder.onValueChanged.AddListener(OnMusicSilderChange);
        SoundSlider.onValueChanged.AddListener(OnSoundSilderChange);

        TipsToggle.onValueChanged.AddListener(OnTipsToggleChange);
        VibrationToggle.onValueChanged.AddListener(OnVibrationToggleChange);

        LowToggle.onValueChanged.AddListener(OnLowToggleChange);
        MediumToggle.onValueChanged.AddListener(OnMediumToggleChange);
        HighToggle.onValueChanged.AddListener(OnHighToggleChange);

        LookSpeedSilder.onValueChanged.AddListener(OnLookSpeedSilderChange);

        PC_or_JoyStick_SwitchButton.onClick.AddListener(onSwitchModeClick);

        ZH_CN_Toggle.onValueChanged.AddListener(OnZH_CN_ToggleChange);
        EN_US_Toggle.onValueChanged.AddListener(OnEN_US_ToggleChange);
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetButtonDown("Cancel"))
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
            ConfirmDialogGO.SetActive(true);
        }
    }

    private void LateUpdate()
    {
        MapNameText.text = MapsDataSingleton.Instance.LocationAreaName; // "Moon Surface";
        Vector3 pos = PlayerGO.transform.position;
        MapsDataSingleton.Instance.MiniMapPlayerLocation = pos;
        PlayerLocationText.text = $"(x:{(int)pos.x},y:{(int)pos.y},z:{(int)pos.z})";
    }

    public void LoadSettingDataUI()
    {
        GameSettingDataSingleton.Instance.LoadData();
        MusicSilder.value = GameSettingDataSingleton.Instance.musicVolume;
        BGMusic.volume = GameSettingDataSingleton.Instance.musicVolume;
        SoundSlider.value = GameSettingDataSingleton.Instance.soundVolume;
        AudioManager.Instance.audioSource.volume = GameSettingDataSingleton.Instance.soundVolume;
        SceneSound.volume = GameSettingDataSingleton.Instance.soundVolume;
        TipsToggle.isOn = GameSettingDataSingleton.Instance.isTipsOn;
        VibrationToggle.isOn = GameSettingDataSingleton.Instance.isVibrationOn;
        LookSpeedSilder.value = GameSettingDataSingleton.Instance.playerLookSpeed;
        switch (GameSettingDataSingleton.Instance.GraphicIndex)
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
        if (GameSettingDataSingleton.Instance.isPCMode)
        {
            PC_or_JoyStick_Image.sprite = MobilePhone_Icon;
            JoyStickGO.SetActive(false);
            PCControlGO.SetActive(true);
        }
        else
        {
            PC_or_JoyStick_Image.sprite = PC_Icon;
            JoyStickGO.SetActive(true);
            PCControlGO.SetActive(false);
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
    }

    public void onSwitchModeClick()
    {
        GameSettingDataSingleton.Instance.isPCMode = !GameSettingDataSingleton.Instance.isPCMode;
        if(GameSettingDataSingleton.Instance.isPCMode)
        {
            PC_or_JoyStick_Image.sprite = MobilePhone_Icon;
            JoyStickGO.SetActive(false);
            PCControlGO.SetActive(true);
        }
        else
        {
            PC_or_JoyStick_Image.sprite = PC_Icon;
            JoyStickGO.SetActive(true);
            PCControlGO.SetActive(false);
        }
        GameSettingDataSingleton.Instance.SaveData();
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
        AudioManager.Instance.audioSource.volume = value;
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

    public void OnLookSpeedSilderChange(float value)
    {

        GameSettingDataSingleton.Instance.playerLookSpeed = value;
        GameSettingDataSingleton.Instance.SaveData();
    }

    public void onBackToMenuButtonClick()
    {
        PlayerData.Instance.SaveData();
        MapsDataSingleton.Instance.MapName = "";
        initSceneManager.Instance.InitScene("StartMenu");
    }

    public void ShowLocationChangeUIAnim(string name)
    {
        GameObject go = Instantiate(LocationChangeTips, UIPrefabsSpawnHolder.transform);
        go.GetComponent<TipsAnim>().Init(name);
    }

    public void GameSceneQuitGame()
    {
        PlayerData.Instance.SaveData();
#if UNITY_EDITOR
        EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    public void OnLeftClickChangeRenderTextureRotation()
    {
        RenderTextureCharacter.Rotate(new Vector3(0, 25f, 0));
    }

    public void OnRightClickChangeRenderTextureRotation()
    {
        RenderTextureCharacter.Rotate(new Vector3(0, -25f, 0));
    }

    public void onWinResetButtonClick()
    {
        PlayerData.Instance.isGameWin = false;
        // onBackToMenuButtonClick();
    }

    public void PauseGame()
    {
        Time.timeScale = 0.0f;
    }

    public void ResumeGame()
    {
        Time.timeScale = 1.0f;
    }
}
