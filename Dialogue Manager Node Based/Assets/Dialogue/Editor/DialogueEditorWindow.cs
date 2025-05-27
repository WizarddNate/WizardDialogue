using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public class DialogueEditorWindow : EditorWindow
{
    [MenuItem("Window/UI Toolkit/DialogueEditorWindow")]
    public static void ShowExample()
    {
        DialogueEditorWindow wnd = GetWindow<DialogueEditorWindow>();
        wnd.titleContent = new GUIContent("DialogueEditorWindow");
    }

    public void CreateGUI()
    {
        // Each editor window contains a root VisualElement object
        VisualElement root = rootVisualElement;

        // VisualElements objects can contain other VisualElement following a tree hierarchy.
        VisualElement label = new Label("Hello World! From C#");
        root.Add(label);

    }
}
