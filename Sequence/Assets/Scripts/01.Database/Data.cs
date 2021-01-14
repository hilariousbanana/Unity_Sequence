using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Data
{
    public List<Quest> QuestList = new List<Quest>();
    public List<ClearRecord> RecordList = new List<ClearRecord>();

    public int CurrentStage;
    public int CurrentKillCount;
    public int CurrentRespawnCount;
    public int CurrentPotionCount;

    public int MaxKillCount;
    public int MaxRespawnCount;
    public int MaxPotionCount;

    public bool bKey = false;
    public bool bBoss = false;
    public bool HaveKey = false;
    public bool BossKilled = false;

    public void AddQuestList()
    {
        QuestList.Add(new Quest(Quest.STAGE.Stage1, 10, 0, 0, 0));
        QuestList.Add(new Quest(Quest.STAGE.Stage2, 10, 5, 0, 0));
        QuestList.Add(new Quest(Quest.STAGE.Stage3, 0, 3, 3, 3));
        QuestList.Add(new Quest(Quest.STAGE.Final, 20, 0, 0, 0));
    }

    public void Initialization()
    {
        AddQuestList();

        CurrentStage = 0;
        CurrentKillCount = 0;
        CurrentRespawnCount = 0;
        CurrentPotionCount = 0;

        MaxKillCount = QuestList[0].KillCount;
        MaxRespawnCount = QuestList[0].RespawnCount;
        MaxPotionCount = QuestList[0].PotionCount;
        HaveKey = QuestList[0].bHaveKey;
        BossKilled = QuestList[0].bBossKilled;
    }
}
