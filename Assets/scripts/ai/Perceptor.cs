using System.Collections.Generic;
using UnityEngine;

public class Perceptor : MonoBehaviour {

    public List<Vector3> terrain;
    public List<Vector3> chasms;
    public List<Vector3> elevationIncrease;
    private Camera mainCamera;
    public int tick = 10;

	// Use this for initialization
	void Start () {
        terrain = new List<Vector3>();
        elevationIncrease = new List<Vector3>();
        chasms = new List<Vector3>();
        mainCamera = Camera.main;
	}
	
	// Update is called once per frame
	void Update () {
        TerrainCheck();
    }
    
    void TerrainCheck()
    {
        terrain = new List<Vector3>();
        chasms = new List<Vector3>();
        elevationIncrease = new List<Vector3>();

        float step = 0.025f;
        for (float y = 0; y <= 1; y += step)
        {
            for (float x = 0; x <= 1; x += step)
            {
                //First see if our raycast has seen a ground tile
                Ray ray = mainCamera.ViewportPointToRay(new Vector3(x, y, 0));
                RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction);
                if (hit)
                {
                    if (hit.transform.name.Contains("ground_square"))
                    {
                        //Raycast has seen a ground tile, lets make sure there isnt a chasm coming up
                        //Do this by looking forward one tile and down a few, and seeing if anythings there
                        Ray chasmRay = mainCamera.ViewportPointToRay(new Vector3(x + step, y - (step*3), 0));
                        Debug.DrawRay(ray.origin, ray.direction * 20, Color.white);
                        RaycastHit2D chasmHit = Physics2D.Raycast(chasmRay.origin, chasmRay.direction);
                        Debug.DrawRay(chasmRay.origin, chasmRay.direction * 20, Color.red);
                        if (!chasmHit)
                        {
                            //We've found a chasm! Dont fall down...
                            chasms.Add(hit.transform.position);
                        }
                        else
                        {
                            //We've hit something again, that means it isnt a chasm we're coming up onto
                            //So check and see if theres an obstacle coming up?
                            //We do this by looking forward one tile and up one tile
                            Ray obstacleRay = mainCamera.ViewportPointToRay(new Vector3(x + step, y + step, 0));
                            RaycastHit2D obstacleHit = Physics2D.Raycast(obstacleRay.origin, obstacleRay.direction);
                            Debug.DrawRay(obstacleRay.origin, obstacleRay.direction * 20, Color.magenta);
                            if (obstacleHit)
                            {
                                //We seem to have hit something in front of us, is it terrain?
                                if (obstacleHit.transform.gameObject.layer.Equals("ground"))
                                {
                                    //Its ground! we can see a terrain obstacle
                                    elevationIncrease.Add(hit.transform.position);
                                }

                            }
                        }
                        //Finally make sure to add the terrain so we know where our target is
                        terrain.Add(hit.transform.position);
                    }
                }
            }
        }
        terrain.Add(new Vector3(-1, -1, -1));
    }
}
