using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Map
{
    public int levelLength;
    //Ground generation values
    public int maxGroundHeight = 50;
    public int maxGroundHeightDifference = 3;
    public int minGroundLevelLength = 3;
    public int levelness = 75; //percentage
    //Feature generation values
    public int chasmChance = 5;
    public int platformChance = 5;
    public int platformMaxLength = 5;
    //Mob generation values
    public int enemyChance = 2; // n enemeies every 100 tiles
    public int coinChance = 3; // n coin runs every 100 tiles
    public int coinRunLength = 5; // avg size of coin runs

    public Map(int levelLength)
    {
        this.levelLength = levelLength;
    }
     
    public void GenerateMap()
    {
        List<int> groundHeights = GenerateGroundHeights();
    }

    public List<MapFeature> GenerateMobs(List<int> groundheights, List<MapFeature> features)
    {
        List<MapFeature> mobs = new List<MapFeature>();

        for(int i = 0; i <= levelLength; i ++)
        {
            if(i > 5 && i < levelLength - 5)
            {
                int roll = Random.Range(0, 100);
                if(roll <= enemyChance)
                {
                    TryCreateEnemy(i, groundheights, features);
                    {
                        mobs.Add(MapFeature.ENEMY);
                    }
                    
                }
                else if(roll > enemyChance && roll <= coinChance)
                {
                    bool gettingLength = true;
                    int currentLength = coinRunLength - 1;
                    while (gettingLength)
                    {
                        int coinRoll = Random.Range(0, 100);

                        if(coinRoll > 75/currentLength)
                        {
                            currentLength++;
                        }
                        else
                        {
                            gettingLength = false;
                        }
                    }

                    List<MapFeature> feature = CreateFeature(MapFeature.COIN, i, currentLength);
                    mobs.AddRange(feature);
                    i += feature.Count - 1;
                }
                else
                {
                    mobs.Add(MapFeature.EMPTY);
                }
            }
        }

        return mobs;
    }

    public List<MapFeature> GenerateFeatures(List<int> groundHeights)
    {
        List<MapFeature> features = new List<MapFeature>();

        for(int i = 0; i <= levelLength; i++){
            if(i <= 5)
            {
                features.Add(MapFeature.GROUND);
            }
            else if(i >= levelLength - 5)
            {
                features.Add(MapFeature.GOAL);
            }
            else
            {
                int chance = Random.Range(0, 100);
                if (chance > 0 && chance < chasmChance)
                {
                    if (TryCreateChasm(i, groundHeights, minGroundLevelLength))
                    {
                        List<MapFeature> feature = CreateFeature(MapFeature.CHASM, i, minGroundLevelLength);
                        features.AddRange(feature);
                        i += feature.Count;
                    }
                }
                else if(chance > chasmChance && chance < chasmChance + platformChance)
                {
                    bool buildingPlatform = true;
                    int currentLength = 2;
                    while (buildingPlatform)
                    {
                        int platformRoll = Random.Range(0, 100);
                        if (platformRoll >= 75 / currentLength && currentLength < platformMaxLength)
                        {
                            currentLength++;
                        }
                        else
                        {
                            buildingPlatform = false;
                        }
                    }

                    List<MapFeature> feature = CreateFeature(MapFeature.PLATFORM, i, currentLength);
                    features.AddRange(feature);
                    i += feature.Count - 1;
                }
                else
                {
                    features.Add(MapFeature.GROUND);
                }
            }
        }
        return features;
    }

    public List<MapFeature> CreateFeature(MapFeature f, int index, int size)
    {
        List<MapFeature> currentFeature = new List<MapFeature>();
        for (int j = index; j <= size + index; j++)
        {
            currentFeature.Add(f);
        }
        return currentFeature;
    }
    
    public bool TryCreateChasm(int index, List<int> heights, int length)
    {
        for (int i = index; i <= index + length; i++)
        {
            if(!CheckHeightsValid(i, heights))
            {
                return false;
            }
        }
        return true;
    }

    public bool TryCreateEnemy(int index, List<int> heights, List<MapFeature> features)
    {
        for (int i = index - 1; i <= index + 2; i++)
        {
            if (!CheckHeightsValid(i, heights) || features[i] == MapFeature.CHASM)
            {
                return false;
            }
        }
        return true;
    }

    public bool CheckHeightsValid(int index, List<int> heights)
    {
        int beforeDelta = heights[index] - heights[index - 1];
        int afterDelta = heights[index] - heights[index + 1];

        if (beforeDelta >= 2 || beforeDelta <= -2 || afterDelta >= 2 || afterDelta <= -2)
        {
            return false;
        }
        else
        {
            return true;
        }
    }

    public List<int> GenerateGroundHeights()
    {
        List<int> groundHeights = new List<int>();

        for(int i = 0; i <= levelLength; i++)
        {
            List<int> possible;
            if (i < 5)
            {
                possible = new List<int>();
                possible.Add(0);
            }
            else
            {
                possible = getPossibleGroundHeights(groundHeights[i - 1]);
            }

            if (Random.Range(0, 100) <= levelness && i > 0)
            {
                groundHeights.Add(groundHeights[i - 1]);
                
            }
            else
            {
                int next = Random.Range(0, possible.Count);
                groundHeights.Add(possible[next]);
            }
        }

        return groundHeights;
    }

    public List<int> getPossibleGroundHeights(int previous)
    {
        List<int> possibleHeights = new List<int>();

        int minHeight = -maxGroundHeightDifference + previous;
        int maxHeight = previous + maxGroundHeightDifference;

        for(int i = minHeight; i <= maxHeight; i++)
        {
            if(i <= maxGroundHeight && i >= -maxGroundHeight) 
            {
                possibleHeights.Add(i);
            }
        }
        return possibleHeights;
    }
    // Use this for initialization
}
