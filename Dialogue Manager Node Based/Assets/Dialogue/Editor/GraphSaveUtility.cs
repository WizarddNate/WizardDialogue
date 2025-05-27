using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class GraphSaveUtility 
{

    private DialogueGraphView _targetGraphView;
    private DialogueContainer _containerCache;

    private List<Edge> Edges => _targetGraphView.edges.ToList();
    private List<DialogueNode> Nodes => _targetGraphView.nodes.ToList().Cast<DialogueNode>().ToList();


   public static GraphSaveUtility GetInstance(DialogueGraphView targetGraphView)
    {
        return new GraphSaveUtility
        {
            _targetGraphView = targetGraphView
        };
    }

    public List<Edge> GetEdges()
    {
        return Edges;
    }

    public void SaveGraph(string fileName)
    {
        //Check if there are any edges made, if not, there there are no node connections and therefore nothing to save
        if (!Edges.Any()) return;

        var dialogueContainer = ScriptableObject.CreateInstance<DialogueContainer>();

        //save the connections (or "edges") between each node
        //every node has one single input port regardless, so only the output ports are being saved
        var connectedPorts = Edges.Where(x => x.input.node != null).ToArray();

        for (var i = 0; i < connectedPorts.Length; i++)
        {
            var outputNode = connectedPorts[i].output.node as DialogueNode;
            var inputNode = connectedPorts[i].input.node as DialogueNode;

            dialogueContainer.NodeLinks.Add(item: new DialogueNodeLinkData 
            { 
                BaseNodeGUId = outputNode.GUID,
                PortName = connectedPorts[i].output.portName,
                TargetNodeGUID = inputNode.GUID
            });
        }

        //save data for the node itself
        foreach (var dialogueNode in Nodes.Where(node => !node.EntryPoint))
        {
            dialogueContainer.DialogueNodeData.Add(new DialogueNodeData 
            {
                NodeGUID = dialogueNode.GUID,
                DialogueText = dialogueNode.DialogueText,
                Position = dialogueNode.GetPosition().position
            });
        }

        //Create a resources folder if one does not already exist
        if (!AssetDatabase.IsValidFolder("Assets/Resources"))
            //                    parent folder, new folder
            AssetDatabase.CreateFolder("Assets", "Resources");

        //Save data to resource folder
        AssetDatabase.CreateAsset(dialogueContainer, $"Assets/Resources/{fileName}.asset");
        AssetDatabase.SaveAssets();
    }

    public void LoadGraph(string fileName)
    {
        _containerCache = Resources.Load<DialogueContainer>(fileName);

        //check if file exists
        if(_containerCache == null)
        {
            EditorUtility.DisplayDialog(title: "File Not Found", message: "Target file does not exist.", ok: "ACCEPT");
            return;
        }

        ClearGraph();
        CreateNodes();
        ConnectNodes();
    }

    private void ClearGraph()
    {
        //Set entry point GUID back from the save. Discard exisiting GUID.
        Nodes.Find(x => x.EntryPoint).GUID = _containerCache.NodeLinks[0].BaseNodeGUId;

        //iterate through each node in the file
        foreach(var node in Nodes)
        {
            if (node.EntryPoint) return;

            //Remove edges that are connnected to this node
            Edges.Where(x => x.input.node == node).ToList().ForEach(edge => _targetGraphView.RemoveElement(edge));

            //Next, remove the node
            _targetGraphView.RemoveElement(node);
        }

    }

    //Create nodes that exist in a save file
    private void CreateNodes()
    {
        foreach (var nodeData in _containerCache.DialogueNodeData)
        {
            var tempNode = _targetGraphView.CreateDialogueNode(nodeData.DialogueText);
            tempNode.GUID = nodeData.NodeGUID;

            //add new node to the graph view
            _targetGraphView.AddElement(tempNode);

            var nodePort = _containerCache.NodeLinks.Where(x => x.BaseNodeGUId == nodeData.NodeGUID).ToList();
            nodePort.ForEach(x => _targetGraphView.AddChoicePort(tempNode, x.PortName));
        }
    }

    private void ConnectNodes()
    {
        return;
    }
}
