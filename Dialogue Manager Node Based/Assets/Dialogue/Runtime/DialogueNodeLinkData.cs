using System;
using UnityEngine;

[SerializeField]
public class DialogueNodeLinkData 
{
    /// <summary>
    /// Saves the data of the LINK BETWEEN nodes. 
    /// Save the ID of the Input and Output nodes as well as the name of the port
    /// </summary>
    

    public string BaseNodeGUId;
    public string TargetNodeGUID;
    public string PortName;
}
