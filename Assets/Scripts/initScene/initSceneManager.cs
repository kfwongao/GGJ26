using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using UnityEngine.Localization.Settings;
//using UnityEngine.SceneManagement;

public class initSceneManager : MonoSingleton<initSceneManager>
{
    public Slider LoadingBar;
    public TextMeshProUGUI LoadingTextMeshPro;
    public GameObject LoadingScene;
    public TextMeshProUGUI MapNameTextMeshPro;
    //private SceneManager sceneManager;

    public float loadingProgressValue = 0;
    public string currentSceneName = "initScene";
    public string initSceneName = "initScene";

    // Reference to the load operation.
    private AsyncOperation loadOperation;
    // Reference to the progress bar in the UI.

    // Progress values.
    private float currentValue;
    private float targetValue;
    // Multiplier for progress animation speed.
    [SerializeField]
    [Range(0, 1)]
    private float progressAnimationMultiplier = 0.25f;

    private bool startLoading = false;


    private void Awake()
    {
        //sceneManager = SceneManager.Instance;
    }

    // Use this for initialization
    //protected override void OnStart()
    //{
    //    if (LoadingBar == null || LoadingTextMeshPro == null)
    //    {
    //        Debug.LogError("initSceneManager :: LoadingBar == null || LoadingTextMeshPro == null");
    //        return;
    //    }

    //    loadingProgressValue = 0;

    //    if (LoadingBar != null)
    //    {
    //        LoadingBar.value = loadingProgressValue;
    //    }

    //    if (LoadingTextMeshPro != null)
    //    {
    //        LoadingTextMeshPro.text = "0%";
    //    }


    //    //StartMenu
    //    InitScene("StartMenu");
    //}

    // Start is called before the first frame update
    void Start()
    {
        // 必須先關閉垂直同步 (VSync)，否則 targetFrameRate 會被忽略
        QualitySettings.vSyncCount = 0;
        // 設定目標幀率，例如 60 FPS
        Application.targetFrameRate = 60;
        GameSettingDataSingleton.Instance.LoadData();
        //LocalizationSettings.SelectedLocale 
        //    = LocalizationSettings.AvailableLocales.Locales[GameSettingDataSingleton.Instance.localization_index];
        //Debug.Log(LocalizationSettings.SelectedLocale);

        if (LoadingBar == null || LoadingTextMeshPro == null)
        {
            Debug.LogError("initSceneManager :: LoadingBar == null || LoadingTextMeshPro == null");
            return;
        }

        loadingProgressValue = 0;

        if (LoadingBar != null)
        {
            LoadingBar.value = loadingProgressValue;
        }

        if (LoadingTextMeshPro != null)
        {
            LoadingTextMeshPro.text = "0%";
        }

        DontDestroyOnLoad(LoadingScene);
        DontDestroyOnLoad(this.gameObject);

        //StartMenu
        InitScene("StartMenu");
    }

    public void InitScene(string name)
    {
        startLoading = true;
        currentSceneName = name;
        loadingProgressValue = 0;
        //LoadingBar.value = 0;
        LoadingScene.SetActive(true);
        MapNameTextMeshPro.text = $"Loading {name} ...";

        // Set 0 for progress values.
        LoadingBar.value = currentValue = targetValue = 0;
        LoadingBar.value = (int)currentValue;
        LoadingTextMeshPro.text = $@"{LoadingBar.value}%";
        // Load the next scene.
        var currentScene = UnityEngine.SceneManagement.SceneManager.GetActiveScene();
        loadOperation = UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(name);
        // Don't active the scene when it's fully loaded, let the progress bar finish the animation.
        // With this flag set, progress will stop at 0.9f.
        loadOperation.allowSceneActivation = false;




        //if (name != currentSceneName && initSceneName != currentSceneName)
        //{
        //    UnityEngine.SceneManagement.SceneManager.UnloadSceneAsync(currentSceneName);
        //}
    //    currentSceneName = name;
    //    sceneManager.LoadScene(name);

    //    sceneManager.onProgress = (value) =>
    //    {
    //        loadingProgressValue = value * 100;
    //        if (loadingProgressValue > 100)
    //        {
    //            loadingProgressValue = 100;
    //        }
    //        LoadingBar.value = (int)loadingProgressValue;
    //        LoadingTextMeshPro.text = $@"{LoadingBar.value}%";
    //    };

    //    sceneManager.onSceneLoadDone = () =>
    //    {
    //        //while(loadingProgressValue < 100)
    //        //{
    //        //    loadingProgressValue += 0.5f;
    //        //    if (loadingProgressValue > 100)
    //        //    {
    //        //        loadingProgressValue = 100;
    //        //    }
    //        //    //Debug.Log(loadingProgressValue);
    //        //    LoadingBar.value = (int)loadingProgressValue;
    //        //    LoadingTextMeshPro.text = $@"{LoadingBar.value}%";
    //        //}

    //        loadingProgressValue = 100;
    //        //Debug.Log(loadingProgressValue);
    //        LoadingBar.value = (int)loadingProgressValue;
    //        LoadingTextMeshPro.text = $@"{LoadingBar.value}%";

    //        UnityEngine.SceneManagement.SceneManager.SetActiveScene(
    //UnityEngine.SceneManagement.SceneManager.GetSceneByName(currentSceneName));
    //        LoadingScene.SetActive(false);
    //        currentSceneName = name;
    //        loadingProgressValue = 0;
    //        LoadingBar.value = 0;
    //    };
    }

    // Update is called once per frame
    void Update()
    {
        if (!startLoading) return;


        if(loadOperation.progress < 0.9f)
        {
            //loadingProgressValue += Time.deltaTime * progressAnimationMultiplier;
            targetValue = loadOperation.progress;
            // Calculate progress value to display.
            currentValue = Mathf.MoveTowards(currentValue, targetValue, progressAnimationMultiplier * Time.deltaTime);
            LoadingBar.value = (int)(currentValue * 100);
            LoadingTextMeshPro.text = $@"{LoadingBar.value}%";
            return;
        }


        // Assign current load progress, divide by 0.9f to stretch it to values between 0 and 1.

        // When the progress reaches 1, allow the process to finish by setting the scene activation flag.
        if (currentValue < 1f)
        {
            currentValue += Time.deltaTime * progressAnimationMultiplier;
            LoadingBar.value = (int)(int)(currentValue * 100);
            LoadingTextMeshPro.text = $@"{LoadingBar.value}%";
        }
        else
        {
            loadOperation.allowSceneActivation = true;
            if (!loadOperation.isDone) return;
            LoadingBar.value = 100;
            LoadingTextMeshPro.text = $@"{LoadingBar.value}%";

            startLoading = false;
            LoadingScene.SetActive(false);
            

        }

    }
}
