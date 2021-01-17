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

    private Data data;
    GAMESTATE gState = GAMESTATE.Title;
    public GameObject GameOverPanel;
    public GameObject ClearPanel;

    private void Start()
    {
        data = DataController.instance.data;
    }

    private void Update()
    {
        GameState();

        if(data.bSucceed)
        {
            ChangeState(GAMESTATE.Clear);
        }

        if(data.bFailed)
        {
            ChangeState(GAMESTATE.GameOver);
        }
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
                Clear();
                break;
            case GAMESTATE.GameOver:
                GameOver();
                break;
            case GAMESTATE.Ending:
                break;
        }
    }

    void Clear()
    {
        data.CurrentStage++;
        data.UpdateVariables(data.CurrentStage);
        StartCoroutine(ClearCoroutine());
    }

    IEnumerator ClearCoroutine()
    {
        yield return new WaitForSeconds(1f);


        LoadingSceneManager.LoadScene(1);

        ChangeState(GAMESTATE.Entry);
    }

    void GameOver()
    {
        data.UpdateVariables(data.CurrentStage);
    }

    IEnumerator GameOverCoroutine()
    {
        yield return new WaitForSeconds(1f);


        LoadingSceneManager.LoadScene(1);

        ChangeState(GAMESTATE.Entry);
    }
}
