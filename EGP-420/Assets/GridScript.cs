using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Grid : Object
{
    private LevelSpecs mLevelSpecs;
    public GridScript gs;
    //length = the first number in the aspect ratio
    int mLength;
    //width = the second number in the aspect ratio
    int mWidth;

    int numFloors;
    int numWalls;

    System.Random rand = new System.Random();

    Dictionary<int, Node> mNodeList = new Dictionary<int, Node>();

    public Grid(GridScript gs, LevelSpecs ls)
    {
        Debug.Log("grid conscructed");
        this.gs = gs;
        mLevelSpecs = ls;
    }

    public void generateGrid()
    {
        Debug.Log("generate called");
        if (mNodeList != null && mNodeList.Count > 0)
        {
            Debug.Log("node list exists and isn't 0");
            reset();
        }
        mLevelSpecs = gs.GetLevelSpecs();
        //set up the dimensions of the level
        int numNodes = mLevelSpecs.getNumNodes();
        //numNodes = number of nodes in the level total
        float aspectRatio = mLevelSpecs.getAspectRatio();
        mLength = (int)Mathf.Floor(Mathf.Sqrt(aspectRatio * (float)numNodes));
        mWidth = (int)(numNodes / mLength);
        int tempID = 0;

        for (float z = 0 - Mathf.Floor(mWidth / 2.0f); z < Mathf.Ceil(mWidth / 2.0f); z+= 1.0f)
        {
            for (float x = 0 - Mathf.Floor(mLength/2.0f); x < Mathf.Ceil(mLength / 2.0f); x+= 1.0f)
            {
                NodeType tempType;
                //based on user-defined wall-floor ratio, make the walls more dense or more sparse
                int ratioCheck = rand.Next(0,100);
                if (ratioCheck <= (int)(100 * mLevelSpecs.getWallFloorRatio()))
                {
                    tempType = NodeType.WALL;
                }
                else
                {
                    tempType = NodeType.FLOOR;
                }

                //create nodes at each incremental position, with a random type
                Node tempNode = new Node(tempID, new Vector3(z, 0.0f, x), tempType);
                if (tempNode.getType() == NodeType.WALL)
                {
                    numWalls++;
                }
                else if (tempNode.getType() == NodeType.FLOOR)
                {
                    numFloors++;
                }
                //Debug.Log(tempID + tempNode.getType());
                //Debug.Log(tempID);
                mNodeList.Add(tempID, tempNode);
                tempID++;
            }
        }

        for (int i = 0; i < mNodeList.Count; i++)
        {
            float tempX = mNodeList[i].getPosition().x;
            float tempZ = mNodeList[i].getPosition().z;

            for (int j = 0; j < mNodeList.Count; j++)
            {
                float xDiff = Mathf.Abs(mNodeList[j].getPosition().x - tempX);
                float zDiff = Mathf.Abs(mNodeList[j].getPosition().z - tempZ);
                //if the x or z distance is exactly 1.0 (the size of a node) away, it is adjacent
                if (xDiff == 1.0f && zDiff == 0.0f || xDiff == 0.0f && zDiff == 1.0f)
                {
                    Debug.Log("it thinks that " + mNodeList[i].getID() + " (" + tempX + ", " + tempZ + ")[" + mNodeList[i].getType() + "] & " + mNodeList[j].getID() + " (" + mNodeList[j].getPosition().x + ", " + mNodeList[j].getPosition().z + ")[" + mNodeList[j].getType() + "] are adajcent");
                    //since adjacency is a mutual relationship, add i to j and j to i.
                    //duplicates will be rejected by the addAdjacenNode method
                    mNodeList[i].addAdjacentNode(mNodeList[j]);
                    mNodeList[j].addAdjacentNode(mNodeList[i]);
                }
            }
        }

        checkConstraints();
        foreach (KeyValuePair<int, Node> node in mNodeList)
        {
            gs.instantiateNode(node.Value);
        }
    }

    public void checkConstraints()
    {
        //ensure the level is surrounded by walls, if the user wants
        if (mLevelSpecs.enclosed == true)
        {
            foreach (KeyValuePair<int, Node> node in mNodeList)
            {
                if (node.Value.getType() != NodeType.WALL && node.Value.getAdjacentNodes().Count < 4)
                {
                    node.Value.setType(NodeType.WALL);
                }
            }
        }

        //ensure that there's only one entrance
        int numStarts = 0;
        while (numStarts < 1)
        {
            foreach (KeyValuePair<int, Node> node in mNodeList)
            {
                if (node.Value.getType() == NodeType.START)
                {
                    numStarts++;
                    if (numStarts > 1)
                    {
                        node.Value.setType(NodeType.WALL);
                        numStarts--;
                        numWalls++;
                    }
                }
            }
            //there must be a start somewhere
            if (numStarts == 0)
            {
                Node tempNode = mNodeList[rand.Next(0, mNodeList.Count)];
                if (tempNode.getType() == NodeType.FLOOR)
                {
                    tempNode.setType(NodeType.START);
                    numFloors--;
                    numStarts++;
                }
            }
        }

        //ensure there is only the user-specified number of exits
        int numExits = 0;
        while (numExits < mLevelSpecs.getNumExits())
        {
            foreach (KeyValuePair<int, Node> node in mNodeList)
            {
                if (node.Value.getType() == NodeType.EXIT)
                {
                    numExits++;
                    if (numExits > mLevelSpecs.getNumExits())
                    {
                        node.Value.setType(NodeType.WALL);
                        numExits--;
                        numWalls++;
                    }
                }
            }
            //there must be a start somewhere
            if (numExits == 0)
            {
                Node tempNode = mNodeList[rand.Next(0, mNodeList.Count)];
                if (tempNode.getType() == NodeType.FLOOR)
                {
                    tempNode.setType(NodeType.EXIT);
                    numFloors--;
                    numExits++;
                }
            }
        }

        //ensure there are no floor squares completely surrounded by walls, i.e. inaccessible
        foreach (KeyValuePair<int, Node> node in mNodeList)
        {
            if (node.Value.getType() == NodeType.FLOOR || node.Value.getType() == NodeType.EXIT || node.Value.getType() == NodeType.START)
            {
                bool surrounded = true;
                foreach (KeyValuePair<int, Node> adjacent in node.Value.getAdjacentNodes())
                {
                    if (adjacent.Value.getType() != NodeType.WALL)
                    {
                        surrounded = false;
                        break;
                    }
                }
                if (surrounded)
                {
                    int[] idList = new int[node.Value.getAdjacentNodes().Count];
                    node.Value.getAdjacentNodes().Keys.CopyTo(idList, 0);
                    int randex = rand.Next(0, node.Value.getAdjacentNodes().Count);
                    Node randNode = node.Value.getAdjacentNodes()[idList[randex]];
                    randNode.setType(NodeType.FLOOR);
                }
            }
        }

        //ensure that there is a path from start to finish
        Pathfinder p = new Pathfinder(mNodeList);
        if (!p.doesPathExist())
        {
            Debug.Log("no valid path");
            //generateGrid();
        }
    }

    //all code that has to do with removing the current level before generating a new one.
    public void reset()
    {
        numFloors = 0;
        numWalls = 0;
        gs.destroyNodes();
        gs.removeNodes();
        mNodeList.Clear();
    }

    public Dictionary<int, Node> getNodeList()
    {
        return mNodeList;
    }
}

[System.Serializable]
public class LevelSpecs
{
    public int numNodes = 32;
    public float aspectRatio = 1;
    public bool enclosed = true;
    public int minRoomArea;
    public int maxRoomArea;
    public int numExits = 1;
    public float wallFloorRatio;
    public int corridorLength;
    public int numRooms;

    public int getNumNodes()
    {
        return numNodes;
    }
    public float getAspectRatio()
    {
        return aspectRatio;
    }
    public bool isEnclosed()
    {
        return enclosed;
    }
    public int getMinRoomArea()
    {
        return minRoomArea;
    }
    public int getMaxRoomArea()
    {
        return maxRoomArea;
    }
    public int getNumExits()
    {
        return numExits;
    }
    public float getWallFloorRatio()
    {
        return wallFloorRatio;
    }
    public int getCorridorLength()
    {
        return corridorLength;
    }
    public int getNumRooms()
    {
        return numRooms;
    }
}


public class GridScript : MonoBehaviour
{
    [SerializeField]
    public Grid grid;

    public GameObject wall;
    public GameObject floor;
    public GameObject start;
    public GameObject exit;
    [SerializeField]
    public LevelSpecs levelSpecs;

    Dictionary<int, GameObject> physNodeList = new Dictionary<int, GameObject>();

    void Start()
    {
        grid = new Grid(this, levelSpecs);
        grid.generateGrid();
    }

    // Update is called once per frame
    void Update()
    {
        //check to see if levelspecs have changed       
        //update accordingly
    }
    
    [ExecuteInEditMode]
    public void OnValidate()
    {
        levelSpecs = levelSpecs;
        if (grid.gs != null)
        {
            grid.reset();
        }
        grid = new Grid(this, levelSpecs);
        grid.generateGrid();
    }

    [ExecuteInEditMode]
    public void instantiateNode(Node n)
    {
        //the enum is just the marker, this is where the actual different shape/color gets determined
        switch (n.getType())
        {
            case NodeType.WALL:
                GameObject wall = Instantiate((GameObject)Resources.Load("Wall", typeof(GameObject)), n.getPosition(), Quaternion.identity);
                Destroy(wall);
                physNodeList.Add(n.getID(), wall);                
                break;
            case NodeType.FLOOR:
                n.setPosition(new Vector3(n.getPosition().x, -0.45f, n.getPosition().z));
                GameObject floor = Instantiate((GameObject)Resources.Load("Floor", typeof(GameObject)), n.getPosition(), Quaternion.identity);
                physNodeList.Add(n.getID(), floor);
                break;
            case NodeType.START:
                n.setPosition(new Vector3(n.getPosition().x, -0.45f, n.getPosition().z));
                GameObject start = Instantiate((GameObject)Resources.Load("Start", typeof(GameObject)), n.getPosition(), Quaternion.identity);
                physNodeList.Add(n.getID(), start);
                break;
            case NodeType.EXIT:
                n.setPosition(new Vector3(n.getPosition().x, -0.45f, n.getPosition().z));               
                GameObject exit = Instantiate((GameObject)Resources.Load("Exit", typeof(GameObject)), n.getPosition(), Quaternion.identity);
                physNodeList.Add(n.getID(), exit);
                break;
            default:
                break;
        }
        
    }

    public void removeNodes()
    {
        physNodeList.Clear();
    }

    //destroy the physical instance of a node in the scene
    [ExecuteInEditMode]
    public void destroyNodes()
    {
        GameObject[] props = GameObject.FindGameObjectsWithTag("Prop");
        foreach (GameObject prop in props)
        {
            if (prop != null)
            {
                if (Application.isEditor)
                {
                    //prop.SetActive(false);
                    DestroyImmediate(prop);
                }
                else
                {
                    Destroy(prop);
                }
            }
        }
    }

    public Grid GetGrid()
    {
        return grid;
    }

    public LevelSpecs GetLevelSpecs()
    {
        return levelSpecs;
    }
}