using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelBuilder : MonoBehaviour
{

    public Transform player;
    //terrain prefabs
    public Transform ground;
    public Transform belowGround;
    public Transform belowPlatform;
    //mob prefabs
    public Transform enemy;
    public Transform coin;
    //Maps
    public List<int> heights;
    public List<MapFeature> features;
    public List<MapFeature> mobs;

    public void BuildLevel(List<int> heights, List<MapFeature> features, List<MapFeature> mobs, bool createPlayer)
    {
        this.heights = heights;
        this.features = features;
        this.mobs = mobs;
        BuildHeights();
        if (createPlayer)
        { 
        CreatePlayer();
        }
     //   BuildMobs();
    }

    public void CreatePlayer()
    {
        Transform objPlayer = Transform.Instantiate(player, new Vector3(4, heights[4] + 1, 0), Quaternion.identity);
        objPlayer.name = "player";
    }

    public void BuildHeights()
    {
        for (int i = 0; i < heights.Count - 1; i++)
        {
            if (features[i] == MapFeature.GROUND)
            {
                Transform.Instantiate(ground, new Vector3(i, heights[i], 0), Quaternion.identity);
                for (int y = heights[i] - 1; y >= heights[i] - 16; y--)
                {
                    Transform.Instantiate(belowGround, new Vector3(i, y, 0), Quaternion.identity);
                }
            }
            if (features[i] == MapFeature.PLATFORM)
            {
                //Create platform
                Transform.Instantiate(ground, new Vector3(i, heights[i] + 3, 0), Quaternion.identity);
                for(int y = heights[i] + 2; y > heights[i]; y--)
                {
                    Transform.Instantiate(belowPlatform, new Vector3(i, y, 0), Quaternion.identity);
                }
                //Create ground
                Transform.Instantiate(ground, new Vector3(i, heights[i], 0), Quaternion.identity);
                for (int y = heights[i] - 1; y >= -15; y--)
                {
                    Transform.Instantiate(belowGround, new Vector3(i, y, 0), Quaternion.identity);
                }
                //Random roll to see if coins will be on the platform
                //75% coin
                //25% empty
                int roll = Random.Range(0, 100);
                if (roll <= 75)
                {
                    Transform.Instantiate(coin, new Vector3(i, heights[i] + 3.5f, 0), Quaternion.identity);
                }
            }
        }
    }

    public void BuildMobs()
    {
        for (int i = 0; i < mobs.Count - 1; i++)
        {
            if(mobs[i] == MapFeature.ENEMY)
            {
                Transform.Instantiate(enemy, new Vector3(i, heights[i] + 1, 0), Quaternion.identity);
            }
            if(mobs[i] == MapFeature.COIN)
            {
                Transform.Instantiate(coin, new Vector3(i, heights[i] + 0.5f, 0), Quaternion.identity);
            }
        }
    }
}
