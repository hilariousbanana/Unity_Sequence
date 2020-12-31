using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ClearRecord
{
    public string PlayerName;
    public int Minutes;
    public int Seconds;
    public int MilliSeconds;

    public ClearRecord(string _playerName, int _minutes, int _seconds, int _milliSeconds)
    {
        PlayerName = _playerName;
        Minutes = _minutes;
        Seconds = _seconds;
        MilliSeconds = _milliSeconds;
    }
}
