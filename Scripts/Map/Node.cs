using System.Collections;
using System.Collections.Generic;

public class Node
{
    // Temp for debugging
    public string Name { get; set; }
    public bool Processed { get; set; }
    public List<Node> Next { get; set; }
    public NodeDisplay NodeDisplay { get; set; }
    public int Level { get; set; }
    public NodeEvent NodeEvent { get; set; }
    public Node Left { get; set; }
    public Node Right { get; set; }

    //The layout of the board for this node (set in combat nodes, null in shop, etc)
    public NPCBoardState Layout { get; set; }

    private bool activated;

    public bool IsActivated()
    {
        return activated;
    }

    // Empty constructor
    public Node()
    {
        Next = new List<Node>();
        activated = false;
        Left = null;
        Right = null;
    }

    // Constructor to set next
    public Node(List<Node> next)
    {
        Next = new List<Node>();
        Next = next;
        activated = false;
        Left = null;
        Right = null;
    }

    //Constructor to set level
    public Node(int level)
    {
        Next = new List<Node>();
        Level = level;
        activated = false;
        Left = null;
        Right = null;
    }

    // Constructor to set level and event
    public Node(int level, NodeEvent nodeEvent)
    {
        Next = new List<Node>();
        Level = level;

        NodeEvent = nodeEvent;
        NodeEvent.Initiate(this);

        activated = false;
        Left = null;
        Right = null;
    }

    public void AddNext(Node node)
    {
        Next.Add(node);
    }

    public void ActivateNode()
    {
        activated = true;
        NodeDisplay.ActivateParticles();
    }

    public void DeactivateNode()
    {
        activated = false;
        NodeDisplay.DeactivateParticles();
    }
}