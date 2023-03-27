using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

public class DSMultiplyChoiceNode : DSNode
{
    public override void Initialize(Vector2 position, DSGraphView graphView)
    {
        base.Initialize(position, graphView);
        DialogueType = DSNodeEnum.MultiplyChoice;
        Choices.Add("New Choice");
    }

    public override void Draw()
    {
        base.Draw();

        var addChoiceButton = DSUtilities.CreateButton("Add Choice", () =>
        {
            var choicePort = CreateChoicePort("New Choice");
            Choices.Add("New Choice");
            
            outputContainer.Add(choicePort);
        });
        
        addChoiceButton.AddToClassList("ds-node__button");

        mainContainer.Insert(1,addChoiceButton);
        
        foreach (var choice in Choices)
        {
            var choicePort = CreateChoicePort(choice);

            outputContainer.Add(choicePort);
        }
        RefreshExpandedState();
    }

    private Port CreateChoicePort(string choice)
    {
        var choicePort = this.CreatePort();

        var deleteChoiceButton = DSUtilities.CreateButton("X");
            
        deleteChoiceButton.AddToClassList("ds-node__button");

        var choiceTextField = DSUtilities.CreateTextField(choice);

        choiceTextField.AddClases(
            "ds-node__textfield",
            "ds-node__choice-textfield",
            "ds-node__textfield__hidden"
        );

        choicePort.Add(choiceTextField);
        choicePort.Add(deleteChoiceButton);
        
        return choicePort;
    }
}
