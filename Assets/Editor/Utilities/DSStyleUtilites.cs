using UnityEditor;
using UnityEngine.UIElements;

public static class DSStyleUtilites
{
    public static VisualElement AddClases(this VisualElement visualElement, params string[] classNames)
    {
        foreach (var className in classNames)
        {
            visualElement.AddToClassList(className);
        }

        return visualElement;
    }
    
    public static VisualElement AddStyles(this VisualElement visualElement, params string[] styleSheetNames)
    {
        foreach (var styleSheetName in styleSheetNames)
        {
            var styleSheet = (StyleSheet) EditorGUIUtility.Load(styleSheetName);

            visualElement.styleSheets.Add(styleSheet);
        }

        return visualElement;
    }
}
