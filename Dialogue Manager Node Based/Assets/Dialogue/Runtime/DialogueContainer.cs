using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class DialogueContainer : ScriptableObject
{
    public List<DialogueNodeLinkData> NodeLinks = new List<DialogueNodeLinkData>();
    public List<DialogueNodeData> DialogueNodeData = new List<DialogueNodeData>();
}
