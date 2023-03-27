using UnityEditor;
using UnityEngine.UIElements;
using UnityEditor.Experimental.GraphView;
using UnityEditor.UIElements;

public class DialogueGraph : EditorWindow
{
    private const string _defaultFileName = "NewSavedFile";
    private Button _saveButton;
    [MenuItem("Window/UI Toolkit/DialogueGraph")]
    public static void Open()
    {
        GetWindow<DialogueGraph>("DialogueGraph");
    }

    private void OnEnable()
    {
        AddGraphView();
        AddToolbar();
        AddStyles();
    }
    

    private void AddStyles()
    {
        rootVisualElement.AddStyles("DSGraphVariables.uss");
    }

    private void AddToolbar()
    {
        Toolbar toolbar = new Toolbar();
        TextField textField = DSUtilities.CreateTextField(_defaultFileName, "File name: ");
        _saveButton = DSUtilities.CreateButton("Save");
        toolbar.Add(textField);
        toolbar.Add(_saveButton);
        rootVisualElement.Add(toolbar);
    }

    private void AddGraphView()
    {
        var graphView = new DSGraphView(this);
        
        graphView.StretchToParentSize();
        
        rootVisualElement.Add(graphView);
    }

    public void EnableSaveButton()
    {
        _saveButton.SetEnabled(true);
    }

    public void DisableSaveButton()
    {
        _saveButton.SetEnabled(false);
    }
}