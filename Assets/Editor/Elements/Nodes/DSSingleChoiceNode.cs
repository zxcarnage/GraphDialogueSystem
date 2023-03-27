using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class DSSingleChoiceNode : DSNode
{
    public override void Initialize(Vector2 position, DSGraphView graphView)
    {
        base.Initialize(position,graphView);
        DialogueType = DSNodeEnum.SingleChoice;
        Choices.Add("Next Dialogue");
    }

    public override void Draw()
    {
        base.Draw();

        foreach (var choice in Choices)
        {
            var choicePort = this.CreatePort(choice);
            outputContainer.Add(choicePort);
        }
        RefreshExpandedState();
    }
}
