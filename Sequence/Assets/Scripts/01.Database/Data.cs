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
    public bool HaveKey = false;
    public bool BossKilled = false;

    public void AddQuestList()
    {
        QuestList.Add(new Quest(Quest.STAGE.Stage1, 10, 0, 0, 0));
        QuestList.Add(new Quest(Quest.STAGE.Stage2, 10, 5, 0, 0));
        QuestList.Add(new Quest(Quest.STAGE.Stage3, 0, 3, 3, 3));
        QuestList.Add(new Quest(Quest.STAGE.Final, 20, 0, 0, 0));
    }
}
