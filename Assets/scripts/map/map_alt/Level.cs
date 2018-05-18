using System.Collections.Generic;
using UnityEngine;

public class Level{

    private int levelSize;
    private int chasmLength;

    public List<int> heights;
    public List<MapFeature> features;
    public List<MapFeature> mobs;
    public int goalPos = 0;

	public Level(int levelSize)
    {
        this.levelSize = levelSize;
        heights = new List<int>();
        features = new List<MapFeature>();
        mobs = new List<MapFeature>();
        chasmLength = 3;
        goalPos = levelSize - 5;
        Debug.Log("Level goal pos " + goalPos);
    }

    public void GenerateHeights()
    {
        int maxDiff = 2;
        for(int i = 0; i <= levelSize; i++)
        {
            if(i <= 10)
            {
                heights.Add(0);
            }
            else if(i >= levelSize - 10)
            {
                heights.Add(heights[i - 1]);
            }
            else
            {
                //Get previous square height and set upper/lower bounds;
                int previous = heights[i - 1];
                int upper = previous + maxDiff;
                int lower = previous + -maxDiff;
                //Random roll for current square height
                //50% continue
                //20% + 1
                //5% + 2
                //20% - 1
                //5% -2
                int roll = Random.Range(0, 100); 
                if(roll <= 70)
                {
                    heights.Add(previous);
                }
                else if(roll <= 80)
                {
                    heights.Add(previous + 1);
                }
                else if(roll <= 85)
                {
                    heights.Add(previous + 2);
                }
                else if(roll <= 95)
                {
                    heights.Add(previous - 1);
                }
                else if(roll <= 100)
                {
                    heights.Add(previous - 2);
                }
                else
                {
                    //Shouldn't get to this point, failsafe incase roll goes wonky
                    heights.Add(previous);
                }
            }
        }
        ValidateHeights();
    }

    /**
     * Used to remove 1x1 square ditches that the player sprite can get stuck in
    */
    public void ValidateHeights()
    {
        for(int i = 0; i < heights.Count; i++)
        {
            if(i > 10 && i < heights.Count - 10)
            {
                if(heights[i - 1] > heights[i] && heights[i + 1] > heights[i])
                {
                    heights[i] = heights[i - 1];
                }
            }
        }
    }
    public void GenerateFeatures()
    {
        for(int i = 0; i <= levelSize; i++)
        {
           
            //Roll for map feature generation
            //5% chasm
            //20% platform
            //else ground
            int roll = Random.Range(0, 100);
            if (i <= 10)
            {
                features.Add(MapFeature.GROUND);
            }
            else if (i >= heights.Count - 10)
            {
                features.Add(MapFeature.GROUND);
            }
            else if (roll <= 5)
            {
                if (IsValidFeature(i, chasmLength, 1))
                {
                    CreateFeature(MapFeature.CHASM, chasmLength);
                    i += chasmLength - 1;
                }
                else
                {
                    features.Add(MapFeature.GROUND);
                }
            }
            else if (roll > 5 && roll <= 25)
            {
                if (IsValidFeature(i, chasmLength, 0))
                {
                    CreateFeature(MapFeature.PLATFORM, chasmLength);
                    i += chasmLength - 1;
                }
                else
                {
                    features.Add(MapFeature.GROUND);
                }
            }
            else
            {
                features.Add(MapFeature.GROUND);
            }
        }
    }

    public void GenerateMobs()
    {

        for(int i = 0; i <= levelSize; i++)
        { 
        
            //roll for mob generation
            //5% enemy
            //5% coin run
            //Remainder empty
            int roll = Random.Range(0, 100);
            if(i <= 10)
            {
                mobs.Add(MapFeature.EMPTY);
            }
            else if(i > levelSize - 10)
            {
                mobs.Add(MapFeature.EMPTY);
            }
            else if(roll <= 10)
            {
                if(features[i] == MapFeature.CHASM || features[i-1] == MapFeature.CHASM || features[i+1] == MapFeature.CHASM)
                {
                    mobs.Add(MapFeature.EMPTY);
                }
                else if(IsValidFeature(i, 4, 1))
                {
                    mobs.Add(MapFeature.ENEMY);
                }
                else
                {
                    mobs.Add(MapFeature.EMPTY);
                }
            }
            else if(roll <= 15 && roll > 5 && mobs[i-1] != MapFeature.COIN)
            {
                //create coin run
                //Get random length
                bool gettingLength = true;
                int currentLength = chasmLength;
                while (gettingLength)
                {
                    int lengthRoll = Random.Range(0, 100);
                    if(lengthRoll > currentLength * 20)
                    {
                        currentLength++;
                    }
                    else
                    {
                        gettingLength = false;
                    }
                }

                if (IsValidFeature(i, currentLength, 2))
                {
                    for(int j = 0; j < currentLength; j++)
                    {
                        mobs.Add(MapFeature.COIN);
                    }
                    i += currentLength - 1;
                }
                else
                {
                    mobs.Add(MapFeature.EMPTY);
                }
            }
            else
            {
                mobs.Add(MapFeature.EMPTY);
            }
        }
    }

    public bool IsValidFeature(int location, int length, int tolerance)
    {
        if (features[location - 1] == MapFeature.CHASM || features[location - 1] == MapFeature.PLATFORM)
        {
            return false;
        }

        for (int i = location - 1; i <= location + length + 1; i++)
        {
            int beforeHeight = heights[i - 1];
            int afterHeight = heights[i + 1];
            if (beforeHeight - heights[i] > tolerance || beforeHeight - heights[i] < -tolerance)
            {
                return false;
            }
            else if (afterHeight - heights[i] > tolerance || afterHeight - heights[i] < -tolerance)
            {
                return false;
            }
            
        }
        return true;
    }

    public void CreateFeature(MapFeature feature, int length)
    {
        for (int i = 0; i < length; i++)
        {
            features.Add(feature);
        }
    }
}
