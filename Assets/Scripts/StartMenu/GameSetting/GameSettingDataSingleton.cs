using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization.Settings;

public class GameSettingDataSingleton : Singleton<GameSettingDataSingleton>
{

    public float musicVolume = 0.5f;
    public float soundVolume = 0.5f;
    public bool isTipsOn = true;
    public bool isVibrationOn = false;
    public int GraphicIndex = 0;
    public int DifficultyIndex = 0;

    public float playerLookSpeed = 30f;
    public bool isPCMode = true;
    public int localization_index = 0;


    public bool LoadData()
    {
        try
        {
            musicVolume = PlayerPrefs.GetFloat("musicVolume", 0.5f);
            soundVolume = PlayerPrefs.GetFloat("soundVolume", 0.5f);
            isTipsOn = PlayerPrefs.GetInt("isTipsOn", 1) == 1 ? true : false;
            isVibrationOn = PlayerPrefs.GetInt("isVibrationOn", 0) == 1 ? true : false;
            GraphicIndex = PlayerPrefs.GetInt("GraphicIndex", 0);
            DifficultyIndex = PlayerPrefs.GetInt("DifficultyIndex", 0);

            playerLookSpeed = PlayerPrefs.GetFloat("playerLookSpeed", 30f);
            isPCMode = PlayerPrefs.GetInt("isPCMode", 1) == 1 ? true : false;
            localization_index = PlayerPrefs.GetInt("localization_index", 0);
        }
        catch(Exception e)
        {
            Debug.LogError(e);
            return false;
        }

        return true;
    }

    public bool SaveData()
    {
        try
        {
            PlayerPrefs.SetFloat("musicVolume", musicVolume);
            PlayerPrefs.SetFloat("soundVolume", soundVolume);
            PlayerPrefs.SetInt("isTipsOn", isTipsOn ? 1 : 0);
            PlayerPrefs.SetInt("isVibrationOn", isVibrationOn ? 1 : 0);
            PlayerPrefs.SetInt("GraphicIndex", GraphicIndex);
            PlayerPrefs.SetInt("DifficultyIndex", DifficultyIndex);

            PlayerPrefs.SetFloat("playerLookSpeed", playerLookSpeed);

            PlayerPrefs.SetInt("isPCMode", isPCMode ? 1 : 0);
            PlayerPrefs.SetInt("localization_index", localization_index);
        }
        catch (Exception e)
        {
            Debug.LogError("Save Game Setting Data Failure!");
            Debug.LogError(e);
            return false;
        }

        return true;
    }

    public void selectLanguage(int index)
    {
        //将下拉框当前选中选项的下标作为参数设置到LocalizationSettings的SelectedLocale达到实现语言切换的效果
        //LocalizationSettings.SelectedLocale = LocalizationSettings.AvailableLocales.Locales[index];
    }

}
