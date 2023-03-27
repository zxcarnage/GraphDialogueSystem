
using System.Collections.Generic;

public class DSNodeError
{
    public DSError DSError { get; private set; }
    public List<DSNode> Nodes { get; set; }

    public DSNodeError()
    {
        DSError = new DSError();
        Nodes = new List<DSNode>();
    }
}
