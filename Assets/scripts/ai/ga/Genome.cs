using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Genome {

    public string[] genome = new string[2];
    public float score;
    public float lowerProportion;
    public float upperProportion;

    public Genome(float obstacleJump, float platformJump)
    {
        String strObstacleJump = "";
        String strPlatformJump = "";
        foreach (byte b in BitConverter.GetBytes(obstacleJump))
        {
            strObstacleJump += Convert.ToString(b, 2); 
        }
        foreach (byte b in BitConverter.GetBytes(platformJump))
        {
            strPlatformJump += Convert.ToString(b, 2); 
        }

        genome[0] = strObstacleJump;
        genome[1] = strPlatformJump;
        score = 0;
    }

    public Genome(String[] genome)
    {
        score = 0;
        this.genome = genome;
    }

    public float obstacleToFloat()
    {
        int f = Convert.ToInt32(genome[0], 2);
        byte[] b = BitConverter.GetBytes(f);
        return BitConverter.ToSingle(b, 0);
    }

    public float platformToFloat()
    {
        int f = Convert.ToInt32(genome[1], 2);
        byte[] b = BitConverter.GetBytes(f);
        return BitConverter.ToSingle(b, 0);
    }

    public Genome Clone()
    {
        return new Genome(obstacleToFloat(), platformToFloat());
    }
}
