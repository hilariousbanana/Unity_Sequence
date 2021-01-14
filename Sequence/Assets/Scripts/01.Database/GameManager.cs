using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoSingleton<GameManager>
{
    enum GAMESTATE
    {
        Title,
        Entry,
        Respawn,
        Clear,
        GameOver,
        Ending
    }

    private DataController data;
    GAMESTATE gState = GAMESTATE.Title;

    private void Update()
    {
        GameState();
    }

    void GameState()
    {
        switch(gState)
        {
            case GAMESTATE.Title:
                break;
            case GAMESTATE.Entry:
                break;
            case GAMESTATE.Respawn:
                break;
            case GAMESTATE.Clear:
                break;
            case GAMESTATE.GameOver:
                break;
            case GAMESTATE.Ending:
                break;
        }
    }

    void ChangeState(GAMESTATE _gState)
    {
        gState = _gState;

        switch (gState)
        {
            case GAMESTATE.Title:
                break;
            case GAMESTATE.Entry:
                break;
            case GAMESTATE.Respawn:
                break;
            case GAMESTATE.Clear:
                break;
            case GAMESTATE.GameOver:
                break;
            case GAMESTATE.Ending:
                break;
        }
    }
}
