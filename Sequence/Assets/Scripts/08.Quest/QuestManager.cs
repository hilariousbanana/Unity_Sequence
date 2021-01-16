using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class QuestManager : MonoBehaviour
{
    Data data;

    public Text[] QuestDescription;
    public Text[] CurrentQuest;

    private void Start()
    {
        data = DataController.instance.data;

        InitText();
        InitNumber(data.CurrentStage);
    }

    private void Update()
    {
        InitNumber(data.CurrentStage);
    }

    void InitText()
    {
        for(int i = 0; i < data.QuestList[data.CurrentStage].TextList.Count; i++)
        {
            QuestDescription[i].text = data.QuestList[data.CurrentStage].TextList[i];
        }
    }

    void InitNumber(int stage)
    {
        switch(stage)
        {
            case 0:
                DestroyQuest(0);
                GetKeyQuest(1);
                break;
            case 1:
                DestroyQuest(0);
                RespawnQuest(1);
                GetKeyQuest(2);
                break;
            case 2:
                DestroyQuest(0);
                RespawnQuest(1);
                CaughtQuest(2);
                PotionQuest(3);
                GetKeyQuest(4);
                break;
            case 3:
                KillBossQuest(1);
                break;
            default:
                break;
        }
    }

    void DestroyQuest(int index)
    {
        CurrentQuest[index].text = $"{data.CurrentKillCount} / {data.MaxKillCount}";
    }

    void GetKeyQuest(int index)
    {
        if (data.bKey)
        {
            CurrentQuest[index].text = "Succeeded";
        }
        else
        {
            CurrentQuest[index].text = "Proceeding";
        }
    }

    void RespawnQuest(int index)
    {
        CurrentQuest[index].text = $"{data.CurrentRespawnCount} / {data.MaxRespawnCount}";
    }

    void CaughtQuest(int index)
    {
        CurrentQuest[index].text = $"{data.CurrentCaughtCount} / {data.MaxCaughtCount}";
    }

    void PotionQuest(int index)
    {
        CurrentQuest[index].text = $"{data.CurrentPotionCount} / {data.MaxPotionCount}";
    }

    void KillBossQuest(int index)
    {
        if (data.bBoss)
        {
            CurrentQuest[index].text = "Succeeded";
        }
        else
        {
            CurrentQuest[index].text = "Proceeding";
        }
    }
}
