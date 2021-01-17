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
        CheckSucceeded();
        CheckFailed();
    }


    #region Initialization
    void InitText()
    {
        for (int i = 0; i < data.QuestList[data.CurrentStage].TextList.Count; i++)
        {
            QuestDescription[i].text = data.QuestList[data.CurrentStage].TextList[i];
        }
    }

    void InitNumber(int stage)
    {
        switch (stage)
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


    #endregion


    #region Check Stage State(Clear / Fail)
    void CheckSucceeded()
    {
        if (DestroyedRobot() && GotKey() && RespawnedProperly() && CaughtProperly() && GotPotionProperly() && KilledBoss())
        {
            data.bSucceed = true;
        }
    }

    void CheckFailed()
    {
        if (data.CurrentRespawnCount > data.MaxRespawnCount)
        {
            data.bFailed = true;
        }
        if (data.CurrentRespawnCount > data.MaxRespawnCount)
        {
            data.bFailed = true;
        }
        if (data.CurrentCaughtCount > data.MaxCaughtCount)
        {
            data.bFailed = true;
        }
        if (data.CurrentPotionCount > data.MaxPotionCount)
        {
            data.bFailed = true;
        }
    }

    #endregion

    #region Quests
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
    #endregion

    #region Check Quest Clear
    bool DestroyedRobot()
    {
        if (data.CurrentKillCount == data.MaxKillCount)
            return true;
        return false;
    }

    bool GotKey()
    {
        if (data.HaveKey == data.bKey)
            return true;
        return false;
    }

    bool RespawnedProperly()
    {
        if (data.CurrentRespawnCount <= data.MaxRespawnCount)
            return true;
        return false;
    }

    bool CaughtProperly()
    {
        if (data.CurrentCaughtCount<= data.MaxCaughtCount)
            return true;
        return false;
    }

    bool GotPotionProperly()
    {
        if (data.CurrentPotionCount <= data.MaxPotionCount)
            return true;
        return false;
    }

    bool KilledBoss()
    {
        if (data.BossKilled == data.bBoss)
            return true;
        return false;
    }

    #endregion
}
