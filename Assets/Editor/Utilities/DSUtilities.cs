using System;
using UnityEditor.Experimental.GraphView;
using UnityEngine.UIElements;

public static class DSUtilities
{

    public static Port CreatePort(this DSNode node, string portName = "", Orientation orientation = Orientation.Horizontal,
        Port.Capacity capacity = Port.Capacity.Single, Direction direction = Direction.Output)
    {
        Port port = node.InstantiatePort(orientation, direction, capacity, typeof(bool));
        port.portName = portName;
        return port;
    }

    public static Button CreateButton(string buttonName, Action onClick = null)
    {
        Button button = new Button(onClick)
        {
            text = buttonName
        };
        return button;
    }
    
    public static Foldout CreateFoldout(string title = null, bool collapsed = false)
    {
        Foldout foldout = new Foldout()
        {
            text = title,
            value = !collapsed
        };
        return foldout;
    }
    
    public static TextField CreateTextField(string value = null,string label = null, EventCallback<ChangeEvent<string>> onValueChanged = null)
    {
        TextField textField = new TextField()
        {
            value = value,
            label = label
        };
        
        if (onValueChanged != null)
        {
            textField.RegisterValueChangedCallback(onValueChanged);
        }

        return textField;
    }

    public static TextField CreateTextArea(string value = null, string label = null, EventCallback<ChangeEvent<string>> onValueChanged = null)
    {
        TextField textArea = CreateTextField(value, label, onValueChanged);
        textArea.multiline = true;

        return textArea;
    }
}
