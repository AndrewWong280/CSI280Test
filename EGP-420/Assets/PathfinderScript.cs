using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathfinderScript : MonoBehaviour
{
    Pathfinder p;
    Dictionary<int, Node> mNodeList = new Dictionary<int, Node>();
    bool pathExists = false;

    // Start is called before the first frame update
    void Start()
    {
        mNodeList = GameObject.Find("Grid").GetComponent<GridScript>().grid.getNodeList();
        p = new Pathfinder(mNodeList);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public bool doesPathExist()
    {
        return p.doesPathExist();
    }
}

//separate pathfinder class for making sure a valid route from start to exit exists
public class Pathfinder
{
    Node mStart, mExit;
    List<Node> mPath = new List<Node>();
    Dictionary<int, Node> mGrid = new Dictionary<int, Node>();
    Dictionary<int, Node> visitedNodes = new Dictionary<int, Node>();
    bool pathExists = false;

    public Pathfinder(Dictionary<int, Node> mNodeList)
    {
        mGrid = mNodeList;
        findEndPoints();
        pathExists = Pathfind(mStart);
    }

    public void findEndPoints()
    {
        foreach (KeyValuePair<int, Node> node in mGrid)
        {
            if (node.Value.getType() == NodeType.START)
            {
                mStart = node.Value;
            }
            if (node.Value.getType() == NodeType.EXIT)
            {
                mExit = node.Value;
            }
        }
    }

    public bool Pathfind(Node start)
    {
        Debug.Log("pathfind called");
        //start at the node passed in
        Node currentNode = start;
        visitedNodes.Add(currentNode.getID(), currentNode);

        while (currentNode != mExit)
        {
            //get all adjacent nodes
            List<Node> possibleNodes = new List<Node>();      
            foreach (KeyValuePair<int, Node> node in currentNode.getAdjacentNodes())
            {
                possibleNodes.Add(node.Value);
            }
            //for all adjacent nodes, 
            foreach (Node node in possibleNodes)
            {
                //if the node is a floor and hasn't already been visited
                if (node.getType() == NodeType.FLOOR && !visitedNodes.ContainsKey(node.getID()))
                {
                    //call pathfind recursively
                    //this should only be true if some brach from currentNode connects to exit
                    if (Pathfind(node))
                    {
                        //inserts at 0 bc it adds to the path inside-out, in reverse
                        mPath.Insert(0, node);
                        Debug.Log(mPath);
                    }
                }
                else if (node.getType() == NodeType.EXIT)
                {
                    //if it's found a path to the exit, add the exit to the path & return
                    mPath.Insert(0, node);
                    Debug.Log(mPath);
                    return true;
                }
            }
        }
        return false;
    }

    public bool doesPathExist()
    {
        return pathExists;
    }
}

