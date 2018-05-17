using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Genome {

    public string[] genome = new string[2];
    public float score;
    public float lowerProportion;
    public float upperProportion;

    public Genome(int obstacleJump, int platformJump)
    { 
        genome[0] = Convert.ToString(obstacleJump, 2);
        genome[1] = Convert.ToString(platformJump, 2);
        score = 0;
    }

    public Genome(String[] genome)
    {
        score = 0;
        this.genome = genome;
    }

    public int obstacleToInt()
    {
        return Convert.ToInt32(genome[0], 2);
    }

    public int platformToInt()
    {
        return Convert.ToInt32(genome[1], 2);
    }

    public Genome Clone()
    {
        return new Genome(obstacleToInt(), platformToInt());
    }
}
