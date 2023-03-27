using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class DSNode : Node
{
    public string DialogueName { get; set; }
    public List<string> Choices { get; set; }
    public string DialogueText { get; set; }
    public DSNodeEnum DialogueType { get; set; }

    public Group Group { get; set; }

    private DSGraphView _graphView;

    private Color _defaultColor;

    public virtual void Initialize(Vector2 position, DSGraphView graphView)
    {
        DialogueName = "Dialogue name";
        Choices = new List<string>();
        _graphView = graphView;
        DialogueText = "Dialogue text";
        _defaultColor = new Color(29f/255, 29f/255, 29f/255);
        SetPosition(new Rect(position, Vector2.zero));
        extensionContainer.AddToClassList("ds-node__extension-container");
        mainContainer.AddToClassList("ds-node__main-container");
    }
    
    public override void BuildContextualMenu(ContextualMenuPopulateEvent evt)
    {
        evt.menu.AppendAction("Disconnect all ports", actionEvent => DisconnectAllPorts());
        evt.menu.AppendAction("Disconnect input ports", actionEvent => DisconnectInputPorts());
        evt.menu.AppendAction("Disconnect output ports", actionEvent => DisconnectOutputPorts());
        base.BuildContextualMenu(evt);
    }

    public virtual void Draw()
    {
        DrawTitleContainer();
        DrawTopContainer();
        DrawExtensionContainer();
    }

    private void DrawExtensionContainer()
    {
        var customContainer = new VisualElement();
        
        customContainer.AddToClassList("ds-node__custom-data-container");

        var dialoguesFoldout = DSUtilities.CreateFoldout("Dialogue Title");
        var textField = DSUtilities.CreateTextArea("Dialogue Text");
        textField.AddClases(
            "ds-node__textfield",
            "ds-node__filename-textfield",
            "ds-node__textfield__hidden"
        );

        dialoguesFoldout.Add(textField);
        customContainer.Add(dialoguesFoldout);
        extensionContainer.Add(customContainer);
    }

    private void DrawTopContainer()
    {
        var inputPort = this.CreatePort("Dialogue Connection", Orientation.Horizontal, Port.Capacity.Multi, Direction.Input);
        inputContainer.Add(inputPort);
    }

    private void DrawTitleContainer()
    {
        var labelTextField = DSUtilities.CreateTextField("Dialogue Name" ,null, callback =>
        {
            if (Group == null)
            {
                _graphView.RemoveUngroupedNode(this);

                DialogueName = callback.newValue;
            
                _graphView.AddUngroupedNode(this);
                
                return;
            }
            
            var currentGroup = Group;
                
            _graphView.RemoveGroupedNode(Group, this);

            DialogueName = callback.newValue;
                
            _graphView.AddGroupedNode(currentGroup, this);
            
        });
        labelTextField.AddClases(
            "ds-node__textfield",
            "ds-node__quote-textfield"
        );
        titleContainer.Insert(0,labelTextField);
    }

    public void SetErrorStyle(Color color)
    {
        mainContainer.style.backgroundColor = color;
    }

    public void DisconnectInputPorts()
    {
        DisconnectPorts(inputContainer);
    }

    public void DisconnectOutputPorts()
    {
        DisconnectPorts(outputContainer);
    }

    public void DisconnectAllPorts()
    {
        DisconnectInputPorts();
        DisconnectOutputPorts();
    }

    private void DisconnectPorts(VisualElement container)
    {
        foreach (Port port in container.Children())
        {
            if(port.connected)
                _graphView.DeleteElements(port.connections);
        }
    }

    public void ResetErrorStyle()
    {
        mainContainer.style.backgroundColor = _defaultColor;
    }
    
}
