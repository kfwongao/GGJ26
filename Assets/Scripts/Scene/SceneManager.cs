using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

//public class SceneManager : MonoSingleton<SceneManager>
//{
//    public UnityAction<float> onProgress = null;

//    public UnityAction onSceneLoadDone = null;

//    public float timeProgress = 0;

//    private string sceneName = "";

//    // Use this for initialization
//    protected override void OnStart()
//    {
        
//    }

//    // Update is called once per frame
//    void Update () {
		
//	}

//    public void LoadScene(string name)
//    {
//        timeProgress = 0;
//        sceneName = name;
//        StartCoroutine(LoadLevel(name));
//    }

//    IEnumerator LoadLevel(string name)
//    {
//        Debug.LogFormat("LoadLevel: {0}", name);
//        AsyncOperation async = UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(name);
//        async.allowSceneActivation = false;
//        async.completed += LevelLoadCompleted;
//        while (!async.isDone)
//        {
//            if (onProgress != null)
//            {
//                if(async.progress < 0.99f)
//                {
//                    timeProgress += Time.deltaTime;
//                    timeProgress = Mathf.Clamp(timeProgress, 0, 1);
//                    onProgress(timeProgress);
//                }
//                else
//                {
//                    timeProgress += 0.1f;
//                    timeProgress = Mathf.Clamp(timeProgress, 0, 1);
//                    onProgress(timeProgress);
//                }

//                //onProgress(async.progress);
//            }
                
//            yield return null;
//        }
//    }

//    private void LevelLoadCompleted(AsyncOperation obj)
//    {
//        //if (onProgress != null)
//        //    onProgress(1f);
//        while(timeProgress < 1f)
//        {
//            timeProgress += Time.deltaTime;
//            timeProgress = Mathf.Clamp(timeProgress, 0, 1);
//            if (onProgress != null)
//                onProgress(timeProgress);
//        }

//        Debug.Log("LevelLoadCompleted:" + obj.progress);
//        if (onSceneLoadDone != null)
//        {
            
//            onSceneLoadDone();
//            //onSceneLoadDone = null;
//        }
            
//    }
//}
