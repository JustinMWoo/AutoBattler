using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


// TODO: split Map manager into multiple classes to generate map vs manage map (class is currently doing too much)
public class MapManager : MonoBehaviour
{
    #region Singleton
    private static MapManager _instance;
    public static MapManager Instance { get { return _instance; } }

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            _instance = this;
        }
    }
    #endregion

    // Prefabs and events set in the editor
    [Header("Map building prefabs")]
    [SerializeField]
    private GameObject NodePrefab;
    [SerializeField]
    private GameObject LevelPrefab;
    [SerializeField]
    private GameObject MapPanel;
    [SerializeField]
    private GameObject LineParent;
    [SerializeField]
    private GameObject visitedPrefab;

    [Header("Non-random Node Events")]
    [SerializeField]
    private NodeEvent bossEvent;
    [SerializeField]
    private NodeEvent entranceEvent;

    [Header("Animator")]
    public MapAnimator mapAnimator;

    private List<List<Node>> map;
    private Node currentNode;
    private NodeEvent[] nodeEvents;
    private int rateSum;
    private bool mapOpen;

    private void Start()
    {
        // Load the events
        nodeEvents = Resources.LoadAll<NodeEvent>("MapEvents");

        // Sum of event rates for use when choosing events
        rateSum = 0;
        foreach (NodeEvent nEvent in nodeEvents)
        {
            rateSum += nEvent.spawnWeight;
        }


        map = new List<List<Node>>();
        GenerateMap(10, 3, 6, 3);
        StartCoroutine(DrawMap());
        ActivateNextNodes();
        //DebugMap();

        mapOpen = false;
    }

    public Node GetCurrentNode()
    {
        return currentNode;
    }

    /* *
     * Generates a map based on the parameters given
     * 
     * @param numLevels number of levels total (must be at least 2 for boss and start)
     * @param minNodes minimum number of nodes per level
     * @param maxNodes maximum number of nodes per level
     * @param splitMax maximum number of nodes created from a split
     * @return void
     */
    public void GenerateMap(int numLevels, int minNodes, int maxNodes, int splitMax)
    {
        if (minNodes > maxNodes || maxNodes < 0 || minNodes < 0 || numLevels < 2)
        {
            Debug.Log("Error creating map (Invalid node parameters)");
            return;
        }

        List<Node> curLevelNodes = new List<Node> { new Node(0, entranceEvent) }; // Initialize with 1 node for start

        currentNode = curLevelNodes[0];

        map.Add(curLevelNodes);

        List<Node> nextLevel = new List<Node>(); // Initialize next level with minimum nodes per level
        Node tempNode = null;
        for (int i = 0; i < minNodes; i++)
        {
            Node nNode = new Node(1, SelectRandomEvent());
            nextLevel.Add(nNode);
            if (tempNode != null) // Set the left and right variables for the next level
            {
                tempNode.Right = nNode;
                nNode.Left = tempNode;
            }
            tempNode = nNode;

        }
        curLevelNodes[0].Next = nextLevel; // Set the next nodes from the first node

        //~~~~~~~~~~~~~ Name for debugging
        curLevelNodes[0].Name = "Start";
        int name = 2;
        for (int i = 0; i < nextLevel.Count; i++)
        {
            nextLevel[i].Name = name.ToString();
            name++;
        }
        //~~~~~~~~~~~~~


        map.Add(nextLevel);

        curLevelNodes = nextLevel;

        for (int i = 2; i < numLevels - 1; i++) // Make each level (skipping first 2)
        {
            int numNewNodes = curLevelNodes.Count; // Current number of nodes in new level (set to number of nodes in previous level to begin with)
            nextLevel = new List<Node>(maxNodes);
            List<Node> unprocessedNodes = new List<Node>(curLevelNodes); // create a copy of the nodes in the current level for processing

            while (unprocessedNodes.Count > 0) // For each node in the current level selected randomly
            {
                int curNodeIndex = Random.Range(0, unprocessedNodes.Count); // Insert new nodes at this index (the index of the parent) to preserve list order
                Node curNode = unprocessedNodes[curNodeIndex];

                unprocessedNodes.Remove(curNode); // mark new node as processed

                // Determine split or merge
                bool split = false;
                if (Random.value >= 0.5f)
                {
                    split = true;
                }

                if (split)
                {
                    int numCreate = Random.Range(1, splitMax + 1); // Number of nodes to create
                    //Debug.Log("level:" + (i + 1) + " node: " + curNode.Name + " split: " + numCreate);

                    tempNode = null;
                    for (int nCount = 0; nCount < numCreate; nCount++) // create nodes until the random amount is made or the level is full
                    {
                        if (nCount != 0) // Only add 1 to count if it is not the first node being created
                        {
                            numNewNodes += 1;
                        }

                        Node newNode = new Node(i, SelectRandomEvent());
                        nextLevel.Add(newNode);
                        curNode.AddNext(newNode);

                        newNode.Name = name.ToString();
                        name++;

                        if (tempNode != null) // Set the left and right variables for the children
                        {
                            tempNode.Right = newNode;
                            newNode.Left = tempNode;
                        }
                        tempNode = newNode;

                        if (numNewNodes >= maxNodes)
                        {
                            break;
                        }
                    }
                }
                else // Merge
                {
                    //Debug.Log("level:" + (i + 1) + " node: " + curNode.Name + " merge");
                    if (numNewNodes > minNodes) // If there are enough nodes to merge two
                    {
                        bool left = false; // Determine left or right (which to check first)
                        if (Random.value >= 0.5f)
                        {
                            left = true;
                        }
                        Node mergeNode = null;


                        for (int j = 0; j < 2; j++) // Check both sides to see if valid
                        {
                            if (left && curNode.Left != null) // Set the merge node to the node on the left if it exists
                            {
                                mergeNode = curNode.Left;
                                //Debug.Log("left merge... merging to " + mergeNode.Name);
                                break;
                            }
                            else if (!left && curNode.Right != null) // Set the merge node to the node on the right if it exists
                            {
                                mergeNode = curNode.Right;
                                //Debug.Log("right merge... merging to " + mergeNode.Name);
                                break;
                            }
                            left = !left; // Check the other side on the second if no valid position is found
                        }

                        if (mergeNode != null) // If merging left or right
                        {
                            numNewNodes--;
                            // Note: if left or right merging and they already have children then do not have to remove from processed nodes
                            if (mergeNode.Next.Count > 0) // If the node being merged to already has a child
                            {

                                if (mergeNode == curNode.Left) // If merging left, get the rightmost child
                                {
                                    curNode.AddNext(mergeNode.Next[mergeNode.Next.Count - 1]);
                                }
                                else // If merging right, get the leftmost child
                                {
                                    curNode.AddNext(mergeNode.Next[0]);
                                }
                            }
                            else // If node being merged to doesnt have a child then create a new one
                            {
                                //Debug.Log("Creating child for merge");
                                unprocessedNodes.Remove(mergeNode); // Remove since it has not been processed yet

                                Node newNode = new Node(i, SelectRandomEvent());
                                nextLevel.Add(newNode);
                                curNode.AddNext(newNode);
                                mergeNode.AddNext(newNode);

                                newNode.Name = name.ToString();
                                name++;
                            }
                        }
                        else // No node on left or right
                        {
                            //Debug.Log("No node to merge to...");
                            // Create a direct child node
                            Node newNode = new Node(i, SelectRandomEvent());
                            nextLevel.Add(newNode);
                            curNode.AddNext(newNode);

                            newNode.Name = name.ToString();
                            name++;
                        }
                    }
                    else
                    {
                        // Not enough nodes to merge (create direct child)
                        Node newNode = new Node(i, SelectRandomEvent());
                        nextLevel.Add(newNode);
                        curNode.AddNext(newNode);

                        newNode.Name = name.ToString();
                        name++;
                    }
                }
            }

            // Set the left and right for the new level
            foreach (Node node in curLevelNodes)
            {
                Node checkNode = node.Left;
                while (checkNode != null)
                {
                    if (node.Next[0].Left != null) // Left already set
                    {
                        break;
                    }
                    // If there is a node on the left set left on the child of current node to the right most child of the left node
                    Node leftNode = checkNode.Next[checkNode.Next.Count - 1];
                    //Debug.Log("Node: " + node.Name + "Left checknode: " + checkNode.Name);
                    if (leftNode != node.Next[0])
                    {
                        node.Next[0].Left = leftNode;
                        break;
                    }
                    checkNode = checkNode.Left;
                }

                checkNode = node.Right;
                while (checkNode != null)
                {
                    if (node.Next[node.Next.Count - 1].Right != null) // Right already set
                    {
                        break;
                    }
                    // Opposite for right
                    Node rightNode = checkNode.Next[0];
                    //Debug.Log("Node: " + node.Name + " Right check node: " + checkNode.Name);
                    if (rightNode != node.Next[node.Next.Count - 1])
                    {
                        node.Next[node.Next.Count - 1].Right = rightNode;
                        break;
                    }
                    checkNode = checkNode.Right;
                }
            }


            map.Add(nextLevel);
            curLevelNodes = nextLevel;
            //Debug.Log(numNewNodes + " nodes in new level");
        }

        nextLevel = new List<Node>();

        Node finalNode = new Node(numLevels - 1, bossEvent);
        nextLevel.Add(finalNode);

        finalNode.Name = "Final";

        // For last level make all paths converge
        foreach (Node node in curLevelNodes)
        {
            node.AddNext(finalNode);
        }

        map.Add(nextLevel);
    }

    private void Split()
    {

    }

    private void Merge()
    {

    }

    private NodeEvent SelectRandomEvent()
    {
        int roll = Random.Range(0, rateSum + 1);

        int curSum = 0;
        for (int i = 0; i < nodeEvents.Length; i++)
        {
            curSum += nodeEvents[i].spawnWeight;
            if (roll <= curSum)
            {
                return nodeEvents[i];
            }
        }
        return null;
    }

    public void DebugMap()
    {
        for (int i = 0; i < map.Count; i++)
        {
            Debug.Log("____________________________");
            for (int j = 0; j < map[i].Count; j++)
            {
                Debug.Log("Level: " + i + " Node :" + map[i][j].Name);
                if (map[i][j].Left != null)
                {
                    Debug.Log("|   Left: " + map[i][j].Left.Name + "   |");
                }

                if (map[i][j].Right != null)
                {
                    Debug.Log("|   Right: " + map[i][j].Right.Name + "   |");
                }
                //foreach (Node node in map[i][j].Next)
                //{
                //    Debug.Log("Child: " + node.Name);
                //}
            }
        }
    }


    public IEnumerator DrawMap()
    {
        List<GameObject> levelPanels = new List<GameObject>();

        for (int i = 0; i < map.Count; i++)
        {
            GameObject levelPanel = Instantiate(LevelPrefab, MapPanel.transform);
            levelPanels.Add(levelPanel);
        }

        // Run DFS to create the nodes on the map
        DFS(map[0][0], levelPanels);

        yield return new WaitForEndOfFrame(); // Wait for positions to update

        // After nodes all created and in position iterate again to draw the lines
        foreach (List<Node> level in map)
        {
            foreach (Node node in level)
            {
                node.NodeDisplay.DrawLine(LineParent);
                node.NodeDisplay.SetupTooltip();
            }
        }

        // Move the lines so they do not clip with the nodes
        Vector3 lineParentPos = LineParent.transform.position;
        LineParent.transform.position = new Vector3(lineParentPos.x, lineParentPos.y, lineParentPos.z + 1);
    }

    private void DFS(Node curNode, List<GameObject> panels)
    {
        curNode.Processed = true;
        GameObject nodeObject = Instantiate(NodePrefab, panels[curNode.Level].transform);
        if (curNode.NodeEvent != null)
        {
            nodeObject.GetComponentInChildren<Image>().sprite = curNode.NodeEvent.icon;
        }

        NodeDisplay nodeDisplay = nodeObject.GetComponent<NodeDisplay>();
        nodeDisplay.Node = curNode;
        curNode.NodeDisplay = nodeDisplay;


        //// Determine if the grandchild of node is already processed, if so then create those nodes first
        //List<Node> first = new List<Node>();
        //List<Node> second = new List<Node>();

        //foreach (Node child in curNode.Next)
        //{
        //    bool shortPath = false;
        //    foreach (Node grandChild in child.Next)
        //    {
        //        if (grandChild.Processed)
        //        {
        //            first.Add(child);
        //            shortPath = true;
        //            break;
        //        }
        //    }
        //    if (shortPath)
        //    {
        //        first.Add(child);
        //    }
        //    else
        //    {
        //        second.Add(child);
        //    }

        //}


        foreach (Node child in curNode.Next)
        {
            if (!child.Processed)
            {
                DFS(child, panels);
            }
        }
    }

    private void DeactivateNextNodes()
    {
        foreach (Node child in currentNode.Next)
        {
            child.DeactivateNode();
        }
    }

    private void ActivateNextNodes()
    {
        // Mark the current node as visited
        Instantiate(visitedPrefab, currentNode.NodeDisplay.transform);

        foreach (Node child in currentNode.Next)
        {
            child.ActivateNode();
        }
    }

    public void MoveToNode(Node node)
    {
        DeactivateNextNodes();
        currentNode = node;
        ActivateNextNodes();
    }

    public void OpenMap()
    {
        if (mapAnimator.OpenMap())
        {
            mapOpen = true;
        }

    }

    public void CloseMap()
    {
        if (mapAnimator.CloseMap())
        {
            mapOpen = false;
        }

    }

    public void ToggleMap()
    {
        if (mapOpen)
        {
            CloseMap();
        }
        else
        {
            OpenMap();
        }
    }
}