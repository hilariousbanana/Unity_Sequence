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
    public int CurrentCaughtCount;

    public int MaxKillCount;
    public int MaxRespawnCount;
    public int MaxPotionCount;
    public int MaxCaughtCount;

    public bool bKey = false;
    public bool bBoss = false;
    public bool HaveKey = false;
    public bool BossKilled = false;

    public void AddQuestList()
    {
        QuestList.Add(new Quest(Quest.STAGE.Stage1, 10, 0, 0, 0, true, false));
        QuestList.Add(new Quest(Quest.STAGE.Stage2, 10, 5, 0, 0, true, false));
        QuestList.Add(new Quest(Quest.STAGE.Stage3, 0, 3, 3, 3, true, false));
        QuestList.Add(new Quest(Quest.STAGE.Final, 20, 0, 0, 0));
    }

    public void AddTextList()
    {
        QuestList[0].AddText(new List<string> 
        {
        "Destroy Tutorial Robots.",
        "Find the Key."
        }
        );
        QuestList[1].AddText(new List<string>
        {
        "Destroy Any Robots.",
        "Respawn 5 Times or Less.",
        "Find the Key."
        }
        );
        QuestList[2].AddText(new List<string>
        {
        "Destroy Tutorial Robots.",
        "Respawn 3 Times or Less.",
        "Don't Get Caught 3 Times or Less.",
        "Take the Heal Pack 3 Times or More.",
        "Find the Key."
        }
        );
        QuestList[3].AddText(new List<string>
        {
        "Destroy All Robots.",
        "Destroy the Boss."
        }
        );

    }

    public void Initialization()
    {
        AddQuestList();
        AddTextList();

        CurrentStage = 0;
        CurrentKillCount = 0;
        CurrentRespawnCount = 0;
        CurrentPotionCount = 0;
        CurrentCaughtCount = 0;

        MaxKillCount = QuestList[0].KillCount;
        MaxRespawnCount = QuestList[0].RespawnCount;
        MaxPotionCount = QuestList[0].PotionCount;
        MaxCaughtCount = QuestList[0].CaughtCount;
        HaveKey = QuestList[0].bHaveKey;
        BossKilled = QuestList[0].bBossKilled;
    }

    public void UpdateVariables(int stage)
    {
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
