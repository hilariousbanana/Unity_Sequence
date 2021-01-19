using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DialogueManager : MonoBehaviour
{
    public Dialogue DialogueList;

    public int CurrentDial = 0;
    public int CurrentIndex = 0;
    
    public GameObject DialogueBox;
    public GameObject QuestBox;
    public GameObject BtnOk;
    public GameObject BtnNext;
    public Text text;
    public Animator anim_window;

    public bool bNext;
    public bool bDialEnd;

    // Start is called before the first frame update
    void Start()
    {
        bNext = true;
        bDialEnd = false;
        InitializeDialogue();
        DialogueBox.SetActive(true);

        if(DataController.instance.data.CurrentStage == 0)
            CurrentDial = 0;
        else if(DataController.instance.data.bFailed)
        {
            CurrentDial = 2;
            DataController.instance.data.bFailed = false;
        }
        else
        {
            CurrentDial = 1;
            DataController.instance.data.bSucceed = false;
        }
    }

    // Update is called once per frame
    void Update()
    {
        text.text = DialogueList.Dialogues[CurrentDial][CurrentIndex];
    }


    void InitializeDialogue()
    {
        DialogueList.AddDialogue(Dialogue.DialogueType.Entry, new string[]
        {
            "Welcome to \n \"Project Sequence.\"",
            "We sincerely appreciate \n your participation \n to our new project.",
            "\"Project Sequence\" is designed to resolve \n  'Robot Saturation.'",
            "Before entering to real field, \n we prepared a tutorial field for you.",
            "We hope it will help you in actual combat.",
            "On the left side, \n the quest window will indicate \n what you have to do.",
            "This is the end of the introduction to our project. \n Good Luck."
        }
        );

        DialogueList.AddDialogue(Dialogue.DialogueType.Succeed, new string[]
        {
            "You finally did it ! \n Great job.",
            "Next mission would be harder than the tutorial. \n This is actual field.",
            "Be Prepared. \n There's no time to hesitate. \n Move faster."
        }
        );

        DialogueList.AddDialogue(Dialogue.DialogueType.Fail, new string[]
       {
            "You failed the mission.",
            "So we re-created the stage for you.",
            "This will be the last chance. \n Good Luck, my friend."
       }
       );
    }

    public void ToNextIndex()
    {
        if(CurrentIndex == DialogueList.Dialogues[CurrentDial].Length - 1)
        {
            //CurrentDial++;
            //CurrentIndex = 0;
            QuestBox.SetActive(true);
            BtnNext.SetActive(false);
            BtnOk.SetActive(true);
        }
        else if(bNext)
        {
            bDialEnd = false;
            StartCoroutine(IndexChangeEffect());
        }

        QuestBoxAlert();
    }

    IEnumerator IndexChangeEffect()
    {
        bNext = false;

        Color col = text.color;

        while (col.a >= 0)
        {
            col.a -= 0.025f;
            text.color = col;
            yield return new WaitForSeconds(0.01f);
        }

        CurrentIndex++;

        while (col.a <= 1)
        {
            col.a += 0.025f;
            text.color = col;
            yield return new WaitForSeconds(0.01f);
        }
        
        bNext = true;

        yield return null;
    }

    IEnumerator Timer(float time)
    {
        yield return new WaitForSeconds(time);
        bDialEnd = true;
        DialogueBox.SetActive(false);
    }

    public void BtnOK()
    {
        anim_window.SetBool("Close", true);
        StartCoroutine(Timer(0.5f));
    }

   void QuestBoxAlert()
    {
        if (CurrentDial == 0 && CurrentIndex == 4)
        {
            QuestBox.SetActive(true);
        }
    }

    public void SetEndDial(bool ended)
    {
        bDialEnd = ended;
    }

}
