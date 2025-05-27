using System;
using UnityEngine;

[Serializable]
public class DialogueNodeData 
{
    /// <summary>
    /// data saved for each node. 
    /// Save the unique ID, the dialogue text, and the nodes position in the graph
    /// </summary>

    [HideInInspector] public string NodeGUID;
    public string DialogueText;
    public Vector2 Position;
}



////// BIG ISSUE!!!! //////
/// Right now, this data when saved is easily able to be edited in the inspector window.
/// that is BEGGING for a bug. I do not want people to be able to edit save data, especially the node IDs
/// However, simply typing {get; set;} makes all of the data invisiable in the inspector, which I obviously don't want either.
/// 
/// As of right now, I have these ID's hidden from the inspector entirely
/// But ideally, I would love to see these IDs still, because seeing data is always cool. 
/// I guess this works for now though...