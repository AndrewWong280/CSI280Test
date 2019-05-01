using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum NodeType
{
    WALL,
    FLOOR,
    START,
    EXIT
}

public class Node : Object
{
    int mID;
    Vector3 mPos;
    NodeType mType;
    BoxCollider[] mConnections;
    Dictionary<int, Node> mAdjacents = new Dictionary<int, Node>();

    public Node()
    {

    }

    public Node(int idnum, Vector3 pos, NodeType type)
    {
        mID = idnum;
        mPos = pos;
        mType = type;
    }
    public BoxCollider[] getConnections()
    {
        return mConnections;
    }

    //add a neighboring node to the node's adjacent list, if it's not already there
    public void addAdjacentNode(Node n)
    {
        if (mAdjacents.Count < 4) //since this a grid, the max amount of adjacent nodes is 4
        {
            if (!mAdjacents.ContainsKey(n.getID()))
            {
                mAdjacents.Add(n.getID(), n);
            }
        }
    }

    public bool isAdjacent(Node n)
    {
        return mAdjacents.ContainsValue(n);
    }

    public int getID()
    {
        return mID;
    }
    public void setPosition(Vector3 pos)
    {
        this.mPos = pos;
    }
    public void clearAdjacents()
    {
        mAdjacents.Clear();
    }
    public Vector3 getPosition()
    {
        return mPos;
    }
    public void setType(NodeType t)
    {
        mType = t;
    }
    public NodeType getType()
    {
        return mType;
    }
    public Dictionary<int, Node> getAdjacentNodes()
    {
        return mAdjacents;
    }

    public static bool operator== (Node a, Node b)
    {
        return a.getID() == b.getID();
    }
    public static bool operator!= (Node a, Node b)
    {
        return a.getID() != b.getID();
    }
}

[ExecuteInEditMode]
public class NodeScript : MonoBehaviour
{
   
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
