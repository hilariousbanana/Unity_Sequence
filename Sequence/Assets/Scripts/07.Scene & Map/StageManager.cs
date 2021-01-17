using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Stage
{
    public GameObject StagePrefab;
    public int StageNum;
}

public class StageManager : MonoSingleton<StageManager>
{
    public Stage[] Stages;
    public GameObject Crosshair;

    private void Awake()
    {
        CreateStage(DataController.instance.data.CurrentStage);
    }

    private void Update()
    {
        if(DialogueManager.instance.bDialEnd)
        {
            Crosshair.SetActive(true);
        }
    }

    void CreateStage(int _stageNum)
    {
        Instantiate(Stages[_stageNum].StagePrefab);
    }
}
