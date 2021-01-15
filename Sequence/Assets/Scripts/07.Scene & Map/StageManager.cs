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

    private void Start()
    {
        CreateStage(DataController.instance.data.CurrentStage);
    }

    void CreateStage(int _stageNum)
    {
        Instantiate(Stages[_stageNum].StagePrefab);
    }
}
