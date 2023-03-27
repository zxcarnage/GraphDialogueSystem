using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

public class DSGraphView : GraphView
{
    private DSSearchWindow _searchWindow;
    private DialogueGraph _dialogueGraphWindow;
    private SerializableDictionary<string, DSNodeError> _ugroupedNodes;
    private SerializableDictionary<Group, SerializableDictionary<string, DSNodeError>> _groupedNodes;

    private int _repeatedNamesAmount;

    public int RepeatedNamesAmount
    {
        get => _repeatedNamesAmount;
        set
        {
            _repeatedNamesAmount = value;
            if (_repeatedNamesAmount == 0)
                _dialogueGraphWindow.EnableSaveButton();
            else
                _dialogueGraphWindow.DisableSaveButton();
        }
    }
    public DSGraphView(DialogueGraph dialogueGraphWindow)
    {
        _dialogueGraphWindow = dialogueGraphWindow;
        _ugroupedNodes = new SerializableDictionary<string, DSNodeError>();
        _groupedNodes = new SerializableDictionary<Group, SerializableDictionary<string, DSNodeError>>();
        AddManipulators();
        AddSearchWindow();
        AddGridBackground();

        OnGroupElementsAdded();
        OnGroupedElementsDeleted();
        OnElementsDeleted();
        
        AddStyles();
    }

    private void AddSearchWindow()
    {
        if (_searchWindow == null)
        {
            _searchWindow = ScriptableObject.CreateInstance<DSSearchWindow>();
            _searchWindow.Initialize(this);
        }

        nodeCreationRequest = contex =>
          SearchWindow.Open(new SearchWindowContext(contex.screenMousePosition),
                _searchWindow);
    }

    public override List<Port> GetCompatiblePorts(Port startPort, NodeAdapter nodeAdapter)
    {
        var compatiblePorts = new List<Port>();
        ports.ForEach(port =>
        {
            if (port == startPort)
                return;
            if (port.direction == startPort.direction)
                return;
            if (port.node == startPort.node)
                return;
            compatiblePorts.Add(port);
        });
        
        return compatiblePorts;
    }

    private void AddManipulators()
    {
        SetupZoom(ContentZoomer.DefaultMinScale, ContentZoomer.DefaultMaxScale);
        this.AddManipulator(new ContentDragger());
        this.AddManipulator(new SelectionDragger());
        this.AddManipulator(new RectangleSelector());
        this.AddManipulator(CreateNodeContextualMenu(DSNodeEnum.SingleChoice,"Add Node (Single Choice)"));
        this.AddManipulator(CreateNodeContextualMenu(DSNodeEnum.MultiplyChoice,"Add Node (Multiply Choice)"));
        
        this.AddManipulator(CreateGroupContextualMenu());
    }

    private IManipulator CreateGroupContextualMenu()
    {
        var contextualMenu = new ContextualMenuManipulator(
            menuEvent => 
                menuEvent.menu.AppendAction("Add Group",actionEvent => 
                    AddGroup("DialogueTitle",GetLocalPosition(actionEvent.eventInfo.localMousePosition)))
        );
        return contextualMenu;
    }

    public Group AddGroup(string title,Vector2 mousePosition)
    {
        var group = new Group()
        {
            title = title
        };
        
        group.SetPosition(new Rect(mousePosition, Vector2.zero));
        
        AddElement(group);
        foreach (var element in selection)
        {
            if (element is DSNode)
            {
                DSNode node = (DSNode) element;
                group.AddElement(node);
            }
        }

        return group;
    }

    private IManipulator CreateNodeContextualMenu(DSNodeEnum nodeType,string nodeName)
    {
        var contextualMenu = new ContextualMenuManipulator(
            menuEvent => 
                menuEvent.menu.AppendAction(nodeName, actionEvent => 
                    AddElement(AddNode(nodeType,GetLocalPosition(actionEvent.eventInfo.localMousePosition))))
            );
        return contextualMenu;
    }
    
    public DSNode AddNode(DSNodeEnum nodeType,Vector2 mousePosition)
    {
        var typeOfNode = Type.GetType($"DS{nodeType}Node");
        var node = (DSNode) Activator.CreateInstance(typeOfNode);
        node.Initialize(mousePosition, this);
        node.Draw();
        AddUngroupedNode(node);
        return node;
    }

    public void AddUngroupedNode(DSNode node)
    {
        string nodeName = node.DialogueName;
        if (!_ugroupedNodes.ContainsKey(nodeName))
        {
            DSNodeError nodeError = new DSNodeError();
            nodeError.Nodes.Add(node);
            _ugroupedNodes.Add(nodeName, nodeError);
            return;
        }

        List<DSNode> ungroupedNodesList = _ugroupedNodes[nodeName].Nodes;
        
        ungroupedNodesList.Add(node);

        Color errorColor = _ugroupedNodes[nodeName].DSError.Color;

        if (ungroupedNodesList.Count >= 2)
        {
            ++RepeatedNamesAmount;
            foreach (var element in ungroupedNodesList)
            {
                element.SetErrorStyle(errorColor);
            }
        }
    }
    

    public void RemoveUngroupedNode(DSNode node)
    {
        string nodeName = node.DialogueName;
        List<DSNode> ungroupedNodesList = _ugroupedNodes[nodeName].Nodes;
        ungroupedNodesList.Remove(node);
        node.ResetErrorStyle();
        if (ungroupedNodesList.Count == 1)
        {
            --RepeatedNamesAmount;
            ungroupedNodesList[0].ResetErrorStyle();
            return;
        }

        if (ungroupedNodesList.Count == 0)
        {
            _ugroupedNodes.Remove(nodeName);
        }
    }
    

    private void AddStyles()
    {
        this.AddStyles(
            "DSGraphViewStyle.uss",
            "DSNodeStyle.uss"
        );
    }

    private void AddGridBackground()
    {
        var gridBackground = new GridBackground();
        
        gridBackground.StretchToParentSize();
        
        Insert(0,gridBackground);
    }

    private void OnElementsDeleted()
    {
        List<DSNode> nodesToDelete = new List<DSNode>();
        List<Group> groupsToDelete = new List<Group>();
        List<Edge> edgesToDelete = new List<Edge>();
        deleteSelection = (operationName, askUser) =>
        {
            foreach (var element in selection)
            {
                switch (element)
                {
                    case Edge edge:
                        edgesToDelete.Add(edge);
                        break;
                    case DSNode node:
                        nodesToDelete.Add(node);
                        break;
                    case Group group:
                        groupsToDelete.Add(group);
                        break;
                }
            }

            foreach (var node in nodesToDelete)
            {
                if (node.Group != null)
                {
                    node.Group.RemoveElement(node);
                }
                RemoveUngroupedNode(node);
                node.DisconnectAllPorts();
                RemoveElement(node);
            }
            
            DeleteElements(edgesToDelete);

            foreach (var group in groupsToDelete)
            {
                List<DSNode> groupedNodes = new List<DSNode>();
                foreach (var groupElement in group.containedElements)
                {
                    if (groupElement is DSNode groupedNode)
                    {
                        groupedNodes.Add(groupedNode);
                    }
                }
                group.RemoveElements(groupedNodes);
                RemoveElement(group);
            }
        };
    }

    private void OnGroupedElementsDeleted()
    {
        elementsRemovedFromGroup = (group, elements) =>
        {
            foreach (var element in elements)
            {
                if (!(element is DSNode))
                    continue;
                DSNode node = (DSNode) element;
                RemoveGroupedNode(group, node);
                AddUngroupedNode(node);
            }
        };
    }

    public void RemoveGroupedNode(Group group, DSNode node)
    {
        string nodeName = node.DialogueName;
        node.Group = null;
        List<DSNode> groupedNodesList = _groupedNodes[group][nodeName].Nodes;
        groupedNodesList.Remove(node);
        node.ResetErrorStyle();
        if (groupedNodesList.Count == 1)
        {
            --RepeatedNamesAmount;
            groupedNodesList[0].ResetErrorStyle();
            return;
        }

        if (groupedNodesList.Count == 0)
        {
            _groupedNodes[group].Remove(nodeName);
            if (_groupedNodes[group].Count == 0)
            {
                _groupedNodes.Remove(group);
            }
        }
    }

    private void OnGroupElementsAdded()
    {
        elementsAddedToGroup = (group, elements) =>
        {
            foreach (var element in elements)
            {
                if (!(element is DSNode))
                    continue;

                DSNode node = (DSNode) element;
                RemoveUngroupedNode(node);
                AddGroupedNode(group, node);
            }
        };
    }

    public void AddGroupedNode(Group group, DSNode node)
    {
        string nodeName = node.DialogueName;
        node.Group = group;
        if (!_groupedNodes.ContainsKey(group))
        {
            _groupedNodes.Add(group, new SerializableDictionary<string, DSNodeError>());
        }

        if (!_groupedNodes[group].ContainsKey(nodeName))
        {
            DSNodeError nodeError = new DSNodeError();
            nodeError.Nodes.Add(node);
            _groupedNodes[group].Add(nodeName, nodeError);
            return;
        }

        List<DSNode> groupedNodesList = _groupedNodes[group][nodeName].Nodes;
        groupedNodesList.Add(node);
        Color errorColor = _groupedNodes[group][nodeName].DSError.Color;
        if (groupedNodesList.Count == 2)
        {
            ++RepeatedNamesAmount;
            foreach (var element in groupedNodesList)
            {
                element.SetErrorStyle(errorColor);
            }
        }
    }

    public Vector2 GetLocalPosition(Vector2 mousePosition, bool isSearchWindow = false)
    {
        if (isSearchWindow)
            mousePosition -= _dialogueGraphWindow.position.position;
        return contentViewContainer.WorldToLocal(mousePosition);
    }
}
