using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
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
    public GameObject ResultBox;
    public GameObject GameOverPanel;
    public GameObject ClearPanel;
    public DialogueManager dial;

    private void Start()
    {
        data = DataController.instance.data;
        //dial = FindObjectOfType<DialogueManager>();
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
        data.bSucceed = false;
        data.CurrentStage++;
        data.UpdateVariables(data.CurrentStage);
        StartCoroutine(ClearCoroutine());
    }

    IEnumerator ClearCoroutine()
    {
        ResultBox.GetComponent<Animator>().SetBool("Clear", true);
        ClearPanel.SetActive(true);

        yield return new WaitForSeconds(2f);

        LoadingSceneManager.LoadScene(1);

        ChangeState(GAMESTATE.Entry);
    }

    void GameOver()
    {
        data.bFailed = true;
        dial.SetEndDial(false);
        data.UpdateVariables(data.CurrentStage);
        StartCoroutine(GameOverCoroutine());
    }

    IEnumerator GameOverCoroutine()
    {
        ResultBox.GetComponent<Animator>().SetBool("GameOver", true);
        GameOverPanel.SetActive(true);

        yield return new WaitForSeconds(2f);

        LoadingSceneManager.LoadScene(1);
        
        ChangeState(GAMESTATE.Entry);
    }
}
