using UnityEngine;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using Unity.VisualScripting;
using UnityEngine.UIElements;
using System.Linq;
using System;
using System.Collections.Generic;

public class DialogueGraphView : GraphView
{
    /// <summary>
    /// Creates and manages everything within the dialogue editor window
    /// </summary>

    public readonly Vector2 DefaultNodeSize = new Vector2(x: 150, y: 200);

    public DialogueGraphView()
    {
        
        //ability to zoom in and out in the editor window. Wow, that's so easy!
        SetupZoom(ContentZoomer.DefaultMinScale, ContentZoomer.DefaultMaxScale);

        //Creates drag and drop selection features for graph
        this.AddManipulator(new ContentDragger());
        this.AddManipulator(new SelectionDragger());
        this.AddManipulator(new RectangleSelector());

        //Create a black background. I one day wanna figure out how to add a grid
        var grid = new GridBackground();
        Insert(index: 0, grid);
        grid.StretchToParentSize();
        //grid.style.backgroundColor = Color.blue; //doesnt work for some reason

        AddElement(GenerateEntryPointNode());
    }

    
    // allows outputs to connect to inputs. Idk exactly how this section works though
    public override List<Port> GetCompatiblePorts(Port startPort, NodeAdapter nodeAdapter)
    {
        var compatiblePorts = new List<Port>();

        ports.ForEach(funcCall: (port) => 
        {  
            if(startPort!=port && startPort.node!=port.node)
                compatiblePorts.Add(port);
        });

        return compatiblePorts;
    }

    //pass the target node (dialogue node), direction info (I/O), capacity (one or two connections?) 
    private Port GeneratePort(DialogueNode node, Direction portDirection, Port.Capacity capacity = Port.Capacity.Single)
    {
        return node.InstantiatePort(Orientation.Horizontal, portDirection, capacity, typeof(float));
    }

    //create node to start that starts the dialogue tree
    private DialogueNode GenerateEntryPointNode()
    {
        var node = new DialogueNode
        {
            title = "START",
            GUID = Guid.NewGuid().ToString(),
            DialogueText = "ENTRYPOINT",
            EntryPoint = true
        };


        //Create output port on starting node
        var generatedPort = GeneratePort(node, Direction.Output);
        generatedPort.portName = "Next";
        node.outputContainer.Add(generatedPort);

        //updates node visuals
        node.RefreshExpandedState();
        node.RefreshPorts();

        //set node position
        node.SetPosition(new Rect(x: 100, y:200, width: 100, height: 150));
        return node;
    }

    //allows the user to click a button to generate a new node
    public void CreateNode(string nodeName)
    {
        AddElement(CreateDialogueNode(nodeName));
    }

    public DialogueNode CreateDialogueNode(string nodeName)
    {
        var dialogueNode = new DialogueNode
        {
            title = nodeName,
            DialogueText = nodeName,
            GUID = Guid.NewGuid().ToString()
        };

        var inputPort = GeneratePort(dialogueNode, Direction.Input, Port.Capacity.Multi);
        inputPort.portName = "Input";
        dialogueNode.inputContainer.Add(inputPort);

        //add button to create output branches
        var button = new Button(clickEvent: () => { AddChoicePort(dialogueNode); });
        button.text = "Add Branch";
        dialogueNode.titleContainer.Add(button);

        dialogueNode.RefreshExpandedState();
        dialogueNode.RefreshPorts();

        dialogueNode.SetPosition(new Rect(position: Vector2.zero, DefaultNodeSize));

        return dialogueNode;
    }

    //create output ports on basic (non-entry) nodes
    public void AddChoicePort(DialogueNode dialogueNode, string overriddenPortName = "")
    {
        var generatedPort = GeneratePort(dialogueNode, Direction.Output);

        //removes redudant labels from choice container
        //commented out because it currently creates bugs
        //later just try and hide (instead of removing) the port by using "display:none"

        //var oldLabel = generatedPort.contentContainer.Q<Label>(name: "type");
        //generatedPort.contentContainer.Remove(oldLabel);

        var outputPortCount = dialogueNode.outputContainer.Query(name: "connector").ToList().Count;
        generatedPort.portName = $"Choice {outputPortCount}";

        //not the slighltess clue as to what the hell this does
        var choicePortName = string.IsNullOrEmpty(overriddenPortName) 
            ? $"Choice {outputPortCount + 1}"
            : overriddenPortName;

        var textField = new TextField
        {
            name = string.Empty,
            value = choicePortName
        };
        textField.RegisterValueChangedCallback(evt => generatedPort.portName = evt.newValue);
        generatedPort.contentContainer.Add(new Label(" "));
        generatedPort.contentContainer.Add(textField);

        //make specific size restrictions for the text field
        textField.style.minWidth = 60;
        textField.style.minHeight = 10;
        textField.style.maxWidth = 100;
        textField.style.maxHeight = 200;

        //create a delete button on the choice ports
        var deleteButton = new Button(clickEvent: () => RemovePort(dialogueNode, generatedPort)) 
        { 
            text = "X"
        };
        generatedPort.contentContainer.Add(deleteButton);

        generatedPort.portName = choicePortName;

        dialogueNode.outputContainer.Add(generatedPort);
        dialogueNode.RefreshPorts();
        dialogueNode.RefreshExpandedState();
    }

    //allows you to remove choice ports
    private void RemovePort(DialogueNode dialogueNode, Port generatedPort)
    {
        
        var targetEdge = edges.ToList().Where(x => x.output.portName == generatedPort.portName && x.output.node == generatedPort.node);

        if (targetEdge.Any())
        {
            var edge = targetEdge.First();
            edge.input.Disconnect(edge);
            RemoveElement(targetEdge.First());
        }
        
        dialogueNode.outputContainer.Remove(generatedPort);
        dialogueNode.RefreshPorts();
        dialogueNode.RefreshExpandedState();
    }
}
