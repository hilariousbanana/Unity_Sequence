using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LoadingSceneManager : MonoSingleton<LoadingSceneManager>
{
    [SerializeField]
    private Slider LoadingBar;
    [SerializeField]
    private Text ProgressValue;
    [SerializeField]
    private Text NextStage;
    public static int TargetSceneNum;

    private void Start()
    {
        NextStage.text += SetNextStageText();
        StartCoroutine(LoadSceneCoroutine());  
    }

    string SetNextStageText()
    {
        string temp = string.Empty;

        //switch parameter: DataController.instance.data.CurrentStage
        switch (0)
        {
            case 0:
                temp = "Stage 1";
                break;
            case 1:
                temp = "Stage 2";
                break;
            case 2:
                temp = "Stage 3";
                break;
            case 3:
                temp = "Final Stage";
                break;
            default:
                temp = "Error: Couldn't Find Stage Number.";
                break;
        }

        return temp;
    }
    public static void LoadScene(int _sceneNum)
    {
        TargetSceneNum = _sceneNum;
        SceneManager.LoadScene(2); //Load Scene to Loading Scene
    }

    IEnumerator LoadSceneCoroutine()
    {
        yield return null;

        AsyncOperation op = SceneManager.LoadSceneAsync(TargetSceneNum);
        op.allowSceneActivation = false;

        float timer = 0.0f;

        while(!op.isDone)
        {
            yield return null;

            timer += Time.deltaTime;

            if(op.progress < 0.9f)
            {
                LoadingBar.value = Mathf.Lerp(LoadingBar.value, op.progress, timer);
                ProgressValue.text = ((int)(LoadingBar.value * 100)).ToString();
                ProgressValue.text += "%";
                if(LoadingBar.value >= op.progress)
                {
                    timer = 0f;
                }
            }
            else
            {
                LoadingBar.value = Mathf.Lerp(LoadingBar.value, 1f, timer);
                ProgressValue.text = ((int)(LoadingBar.value * 100)).ToString();
                ProgressValue.text += "%";

                if (LoadingBar.value == 1.0f)
                {
                    op.allowSceneActivation = true;
                    yield break;
                }
            }
        }
    }
}
