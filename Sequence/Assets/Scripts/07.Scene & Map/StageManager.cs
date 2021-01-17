using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Stage
{
    public GameObject StagePrefab;
    public int StageNum;
}

public class StageManager : MonoBehaviour
{
    public Stage[] Stages;
    public GameObject Crosshair;
    private DialogueManager dial;

    private void Awake()
    {
        CreateStage(DataController.instance.data.CurrentStage);
        dial = FindObjectOfType<DialogueManager>();
    }

    private void Update()
    {
        if(dial.bDialEnd)
        {
            Crosshair.SetActive(true);
        }
    }

    void CreateStage(int _stageNum)
    {
        Instantiate(Stages[_stageNum].StagePrefab);
    }
}
