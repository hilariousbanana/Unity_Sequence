using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Stage
{
    public GameObject StagePrefab;
    public Transform RespawnPoint;
    public int StageNum;
}

public class StageManager : MonoBehaviour
{
    public Stage[] Stages;

    // Start is called before the first frame update
    void Start()
    {
        CreateStage(DataController.instance.data.CurrentStage);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void CreateStage(int _stageNum)
    {
        Instantiate(Stages[_stageNum].StagePrefab);
    }
}
