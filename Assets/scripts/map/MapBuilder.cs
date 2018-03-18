using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapBuilder : MonoBehaviour {

    public Transform ground;
    public Transform belowGround;
    public Transform player;
    public Transform belowPlatform;
    public Transform coin;
    public Transform enemy;
    public Transform goal;

    public void BuildMap(List<int> groundHeights, List<MapFeature> features, List<MapFeature> mobs)
    {
        BuildTerrain(groundHeights, features);
        BuildMobs(groundHeights, mobs, features);
    }

    public void BuildMobs(List<int> groundHeights, List<MapFeature> mobs, List<MapFeature> features)
    {
        for(int i = 0; i < mobs.Count; i++)
        {
            switch (mobs[i])
            {
                case (MapFeature.ENEMY):
                    if (features[i] != MapFeature.CHASM)
                    {
                        Instantiate(enemy, new Vector3(i, groundHeights[i] + 1, 0), Quaternion.identity);
                    }
                    break;
                case (MapFeature.COIN):
                    if (features[i] == MapFeature.CHASM)
                    {
                        Instantiate(coin, new Vector3(i, groundHeights[i] + 1.5f, 0), Quaternion.identity);
                    }
                    else
                    {
                        Instantiate(coin, new Vector3(i, groundHeights[i] + 0.5f, 0), Quaternion.identity);
                    }
                    break;
            }
        }   
    }

    public void BuildTerrain(List<int> groundHeights, List<MapFeature> features) 
    {
        int featureStart = 0;

        for (int i = 0; i < groundHeights.Count; i++)
        {
            if (i == 0)
            {
                Transform objPlayer = Instantiate(player, new Vector3(i, groundHeights[i] + 1, 0), Quaternion.identity);
                objPlayer.name = "player";
            }

            if (features[i] == MapFeature.GROUND)
            {
                BuildGroundTile(i, groundHeights);
            }
            else if (features[i] == MapFeature.PLATFORM)
            {
                BuildGroundTile(i, groundHeights);
                if (featureStart == 0)
                {
                    featureStart = i;
                }
                else if (features[i + 1] != MapFeature.PLATFORM)
                {
                    BuildPlatform(featureStart, i, groundHeights);
                    featureStart = 0;
                }
            }
            else if (features[i] == MapFeature.CHASM) //Need to make sure that the chasm is actually jumpable
            {
                if (featureStart == 0)
                {
                    featureStart = i;
                }
                else if (features[i + 1] != MapFeature.CHASM)
                {
                    BuildChasm(featureStart, i, groundHeights);
                    featureStart = 0;
                }
            }
            else if(features[i] == MapFeature.GOAL)
            {
                BuildGroundTile(i, groundHeights);
                if (i == features.Count - 3)
                {
                    Instantiate(goal, new Vector3(i, groundHeights[i], 0), Quaternion.identity);
                }
            }
        }
    }
    public void BuildGroundTile(int i, List<int> heights)
    {
        Instantiate(ground, new Vector3(i, heights[i], 0), Quaternion.identity);

        for (int j = heights[i] - 10; j<= heights[i] - 1; j++)
        {
            Instantiate(belowGround, new Vector3(i, j, 0), Quaternion.identity);

        }
    }

    public void BuildChasm(int chasmStart, int chasmEnd, List<int> heights)
    {
        if(heights[chasmStart - 1] < heights[chasmEnd] + 1)
        {
            heights[chasmEnd + 1] = heights[chasmStart - 1] + 2;
        }
    }
    public void BuildPlatform(int platformStart, int platformEnd, List<int> heights)
    {

        float highestPoint = -999;
        bool hasCoinRun = Random.Range(0, 100) <= 50 ? true : false;
        bool hasCoinBelow = Random.Range(0, 100) <= 5 ? true : false;
        int runLength = Random.Range(0, platformEnd - platformStart);
        int runStart = 0;
        int runCurrent = 0;

        for (int i = platformStart; i <= platformEnd; i++)
        {
            if (heights[i] > highestPoint)
            {
                highestPoint = heights[i];
            }
        }

        for(int i = platformStart; i <= platformEnd; i++)
        {
            Instantiate(ground, new Vector3(i, highestPoint + 3, 0), Quaternion.identity);

            if (hasCoinRun)
            { 
                if(runStart <= 0)
                {
                    runStart = i + (runLength / 2);
                    runCurrent = runStart;
                }
                if (runCurrent < platformEnd - (runLength/2))
                {
                    Instantiate(coin, new Vector3(runCurrent, highestPoint + 3.5f, 0), Quaternion.identity);
                    runCurrent++;
                }
            }

            if (hasCoinBelow)
            {
                Instantiate(coin, new Vector3(i, highestPoint + 0.5f, 0), Quaternion.identity);
            }

            for (int j = heights[i] + 1; j <= highestPoint + 3 - 1; j++)
            {
                Instantiate(belowPlatform, new Vector3(i, j, 0), Quaternion.identity);
            }

        }
    }
}
