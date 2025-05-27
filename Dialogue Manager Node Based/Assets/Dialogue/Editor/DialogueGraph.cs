using UnityEditor;
using UnityEngine;
using UnityEditor.Experimental.GraphView;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using UnityEditor.Overlays;
using System;
using System.Collections.Generic;

public class DialogueGraph : EditorWindow
{

    /// <summary>
    /// create a dialogue graph window within the Unity editor. COOL AS HELL !!!
    /// </summary>

    private DialogueGraphView _graphView;
    private string _fileName = "New Dialogue Tree";

    //open a graph window. Menu item allows you to call it from the editor
    [MenuItem("Graph/Diaglogue Graph")]
    public static void OpenDialogueGraphWindow()
    {
        var window = GetWindow<DialogueGraph>();
        window.titleContent = new GUIContent(text: "Dialogue Graph");
    }

    private void OnEnable()
    {
        ConstructGraphView();
        GenerateToolbar();
    }

    private void ConstructGraphView()
    {
        //instantiate graph view
        _graphView = new DialogueGraphView
        {
            name = "Dialogue Graph"
        };

        //make the graph view stretch to the entire size of the editor
        _graphView.StretchToParentSize();

        //add graph view to editor window
        rootVisualElement.Add(_graphView);
    }

    private void GenerateToolbar()
    {
        //create toolbar within the Dialogue Graph view
        var toolbar = new Toolbar();

        var fileNameTextField = new TextField(label: "File Name:");
        fileNameTextField.SetValueWithoutNotify(_fileName);
        fileNameTextField.MarkDirtyRepaint();
        fileNameTextField.RegisterValueChangedCallback(evt => _fileName = evt.newValue);
        toolbar.Add(fileNameTextField);

        toolbar.Add(child: new Button(clickEvent: () => RequestDataOperation(save: true)) { text = "Save Data"});
        toolbar.Add(child: new Button(clickEvent: () => RequestDataOperation(save: false)) { text = "Load Data" });

        //create a button that will generate nodes
        var nodeCreateButtton = new Button(clickEvent: () => { _graphView.CreateNode("Dialogue Node"); });

        //name of button
        nodeCreateButtton.text = "Create Node";

        toolbar.Add(nodeCreateButtton);
        rootVisualElement.Add(toolbar);
    }

    private void RequestDataOperation(bool save)
    {
        //make sure that the file name isnt empty or invalid
        if (string.IsNullOrEmpty(_fileName))
        {
            EditorUtility.DisplayDialog(title: "ERROR: INVALID FILE", message: "Invalid file name. Please enter a different file name or make sure that the file name isn't blank", ok: "OK.");
            return;
        }

        //if save = true, save data. If false, load data.
        var saveUtility = GraphSaveUtility.GetInstance(_graphView);
        if (save)
        {
            //make sure that the user wants to save first. Might later come back and specify when a save is being overwritten
            int saveCheck = EditorUtility.DisplayDialogComplex("Saving File",
                $"Are you sure that you want to save this file as {_fileName} ?",
                "Save",
                "Cancel",
                "Do Not Save");

            switch (saveCheck)
            { 
                //Save
                case 0:
                    saveUtility.SaveGraph(_fileName);
                    EditorUtility.DisplayDialog(title: "Saving File", message: $"File {_fileName} has been saved.", ok: "OK.");
                    break;

                //cancel
                case 1:
                    break;

                //Don't save
                case 2:
                    break;

                default:
                    break;
            }

            
        }
        else
        {
            saveUtility.LoadGraph(_fileName);
        }
    }

    private void OnDisable()
    {
        rootVisualElement.Remove(_graphView);
    }
}
