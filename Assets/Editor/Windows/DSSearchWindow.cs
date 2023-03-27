using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class DSSearchWindow : ScriptableObject, ISearchWindowProvider
{
    private DSGraphView _graphView;
    private Texture2D _indentationalTexture;

    public void Initialize(DSGraphView graphView)
    {
        _graphView = graphView;
        _indentationalTexture = new Texture2D(1, 1);
        _indentationalTexture.SetPixel(0,0,Color.clear);
        _indentationalTexture.Apply();
    }
    
    public List<SearchTreeEntry> CreateSearchTree(SearchWindowContext context)
    {
        List<SearchTreeEntry> searchTreeEntries = new List<SearchTreeEntry>()
        {
            new SearchTreeGroupEntry(new GUIContent("Create Element")),
            new SearchTreeGroupEntry(new GUIContent("Dialogue Node"), 1),
            new SearchTreeEntry(new GUIContent("Single Choice",_indentationalTexture))
            {
                level = 2,
                userData = DSNodeEnum.SingleChoice
            },
            new SearchTreeEntry(new GUIContent("Multiply Choice",_indentationalTexture))
            {
                level = 2,
                userData = DSNodeEnum.MultiplyChoice
            },
            new SearchTreeGroupEntry(new GUIContent("Groups"), 1),
            new SearchTreeEntry(new GUIContent("Single Choice Group",_indentationalTexture))
            {
                level = 2,
                userData = new Group()
            }
        };
        return searchTreeEntries;
    }

    public bool OnSelectEntry(SearchTreeEntry searchTreeEntry, SearchWindowContext context)
    {
        Vector2 localMousePosition = _graphView.GetLocalPosition(context.screenMousePosition, true);
        switch (searchTreeEntry.userData)
        {
            case DSNodeEnum.SingleChoice:
            {
                DSSingleChoiceNode singleChoiceNode =
                    (DSSingleChoiceNode) _graphView.AddNode(DSNodeEnum.SingleChoice, localMousePosition);
                _graphView.AddElement(singleChoiceNode);
                return true;
            }
            case DSNodeEnum.MultiplyChoice:
            {
                DSMultiplyChoiceNode multiplyChoiceNode =
                    (DSMultiplyChoiceNode) _graphView.AddNode(DSNodeEnum.MultiplyChoice, localMousePosition);
                _graphView.AddElement(multiplyChoiceNode);
                return true;
            }
            case Group _:
            {
                Group group = _graphView.AddGroup("New Group", localMousePosition);
                _graphView.AddElement(group);
                return true;
            }
            default:
            {
                return false;
            }
        }
    }
}
