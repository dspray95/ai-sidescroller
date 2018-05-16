using System.Collections.Generic;
using UnityEngine;

public class Perceptor : MonoBehaviour {

    public List<Vector3> terrain;
    public List<Vector3> chasms;
    public List<Vector3> elevationIncrease;
    public List<Vector3> platformStarts;
    public List<Vector3> coins;
    private Camera mainCamera;
    public int tick = 10;
    public bool geneticAlgorithm = false;

	// Use this for initialization
	void Start () {
        terrain = new List<Vector3>();
        elevationIncrease = new List<Vector3>();
        chasms = new List<Vector3>();
        mainCamera = Camera.main;
	}
	
	// Update is called once per frame
	void Update () {
        if (!geneticAlgorithm)
        {
            TerrainCheck();
        }
    }
    
    void TerrainCheck()
    {
        terrain = new List<Vector3>();
        chasms = new List<Vector3>();
        elevationIncrease = new List<Vector3>();
        platformStarts = new List<Vector3>();
        coins = new List<Vector3>();

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
                        //Raycast has seen a ground tile, lets see if we're looking at a platform...
                        //We check for a platform by seeing if theres terrain below the ground tile that we can pass through
                        Ray platformRay = mainCamera.ViewportPointToRay(new Vector3(x, y - (step * 2), 0));
                        RaycastHit2D platformHit = Physics2D.Raycast(platformRay.origin, platformRay.direction);
                        bool platformDetected = false;
                        if (platformHit) 
                        {
                            if (platformHit.transform.name.Contains("platform"))
                            {
                                //So we've found a platform. We need to know if its the beggining of the platform now. 
                                platformDetected = true;
                                Ray platformBeginningRay = mainCamera.ViewportPointToRay(new Vector3(x - (step), y, 0));
                                RaycastHit2D platformBeginningHit = Physics2D.Raycast(platformBeginningRay.origin, platformBeginningRay.direction);
                                if (!platformBeginningHit)
                                {
                                    //If we can see the beggining of a platform, we need to remember this for later. 
                                    platformStarts.Add(hit.transform.position);
                                    Debug.Log("platform seen");
                                    Debug.DrawRay(platformRay.origin, platformRay.direction * 20, Color.blue);
                                    Debug.DrawRay(platformBeginningRay.origin, platformBeginningRay.direction * 20, Color.blue);
                                }
                            }
                        }
                        if(!platformDetected)
                        {
                            //The ground tile we're looking at isnt part of a platform... so lets check for a chasm
                            //Do this by looking forward one tile and down a few, and seeing if anythings there
                            Ray chasmRay = mainCamera.ViewportPointToRay(new Vector3(x + step, y - (step * 10), 0));
                            Debug.DrawRay(ray.origin, ray.direction * 20, Color.white);
                            RaycastHit2D chasmHit = Physics2D.Raycast(chasmRay.origin, chasmRay.direction);
                            if (!chasmHit)
                            {
                                //We've found a chasm! Dont fall down...
                                chasms.Add(hit.transform.position);
                                Debug.DrawRay(chasmRay.origin, chasmRay.direction * 20, Color.red);
                            }
                            else
                            {
                                //We've hit something again, that means it isnt a chasm we're coming up onto
                                //So check and see if theres an obstacle coming up?
                                //We do this by looking forward one tile and up one tile
                                Ray obstacleRay = mainCamera.ViewportPointToRay(new Vector3(x + step, y + (step * 1.5f), 0));
                                RaycastHit2D obstacleHit = Physics2D.Raycast(obstacleRay.origin, obstacleRay.direction);
                                if (obstacleHit)
                                {
                                    //We seem to have hit something in front of us, is it terrain?
                                    Debug.DrawRay(obstacleRay.origin, obstacleRay.direction * 20, Color.magenta);
                                    if (obstacleHit.transform.name.Contains("ground"))
                                    {
                                        //Its ground! we can see a terrain obstacle
                                        hit.transform.gameObject.GetComponent<SpriteRenderer>().color = Color.green;
                                        elevationIncrease.Add(hit.transform.position);
                                    }

                                }
                            }
                        }
                        //Coins can appear directly above ground tiles, so we should also check for that too
                        Ray coinRay = mainCamera.ViewportPointToRay(new Vector3(x, y + (step), 0));
                        RaycastHit2D coinHit = Physics2D.Raycast(coinRay.origin, coinRay.direction);
                        Debug.DrawRay(coinRay.origin, coinRay.direction * 20, Color.yellow);

                        if (coinHit)
                        {
                            if (coinHit.transform.name.Contains("coin"))
                            {
                                //We found a coin, lets take a note of that
                                Debug.Log("found a coin!");
                                coins.Add(coinHit.transform.position);
                            }
                        }
                    }
                }
            }
        }
    }
}
