using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class DialogueNode : Node
{
    [Tooltip("unique ID to pass to each node")]
    public string GUID;

    public string DialogueText;

    //start point
    public bool EntryPoint = false;
}

