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

    // Start is called before the first frame update
    void Start()
    {
        bNext = true;
        InitializeDialogue();
        DialogueBox.SetActive(true);

        CurrentDial = DataController.instance.data.CurrentStage;
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
    }

    public void ToNextIndex()
    {
        if(CurrentIndex == DialogueList.Dialogues[CurrentDial].Length - 1)
        {
            BtnNext.SetActive(false);
            BtnOk.SetActive(true);
        }
        else if(bNext)
        {
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

}
