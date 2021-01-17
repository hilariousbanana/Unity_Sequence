using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Dialogue
{
    public enum DialogueType
    {
        Entry,
        Succeed,
        Fail
    }

    public DialogueType type;
    public List<string[]> Dialogues = new List<string[]>();

    public void AddDialogue(DialogueType _type, string[] _ments)
    {
        type = _type;
        Dialogues.Add(_ments);
    }
}
