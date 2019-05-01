using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelSpecs: MonoBehaviour
{
    public int mNumNodes;
    public float mAspectRatio;
    public bool mEnclosed;
    public int mMinRoomArea;
    public int mMaxRoomArea;
    public int mNumExits;

    public int getNumNodes()
    {
        return mNumNodes;
    }
    public float getAspectRatio()
    {
        return mAspectRatio;
    }
    public bool isEnclosed()
    {
        return mEnclosed;
    }
    public int getMinRoomArea()
    {
        return mMinRoomArea;
    }
    public int getMaxRoomArea()
    {
        return mMaxRoomArea;
    }
    public int getNumExits()
    {
        return mNumExits;
    }
}
