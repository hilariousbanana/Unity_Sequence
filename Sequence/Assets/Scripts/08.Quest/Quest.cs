using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Quest
{
    public int KillCount;
    public int RespawnCount;
    public int CaughtCount;
    public int PotionCount;
    public bool bHaveKey;
    public bool bBossKilled;
    public STAGE Stage;
    public List<string> TextList;

    public enum STAGE
    {
        Stage1,
        Stage2,
        Stage3,
        Final
    }

    public Quest(STAGE _stage, int _killCount, int _respawnCount, int _caughtCount, int _potionCount, bool _haveKey = true, bool _bossKilled = true)
    {
        Stage = _stage;
        KillCount = _killCount;
        RespawnCount = _respawnCount;
        CaughtCount = _caughtCount;
        PotionCount = _potionCount;
        bHaveKey = _haveKey;
        bBossKilled = _bossKilled;
    }

    public void AddText(List<string> _list)
    {
        TextList = _list;
    }
}
