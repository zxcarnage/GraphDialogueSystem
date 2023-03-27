using UnityEngine;

public class DSError
{
    public Color Color {get; private set;}

    public DSError()
    {
        SetRandomColor();
    }
    
    private void SetRandomColor()
    {
        Color = new Color(
            255,
            0,
            0
        );
    }
    
}
