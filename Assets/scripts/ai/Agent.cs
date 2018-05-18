using System.Collections.Generic;
using UnityEngine;

public class Agent : MonoBehaviour {

    public Perceptor perceptor;
    PlayerController controller;
    public float obstacleLeapDistance = 1;
    public float platformLeapDistance = 3;
    public bool clearingChasm = false;
    Vector3 lastChasmEnd;
    public Transform trajactoryPrefab;
    public List<Trajectory> jumpPaths;
    public bool jumping = false;
    public bool leftGround = false;
    public float move;
    public float target;
    public Genome genome;
	// Use this for initialization
	void Start () {
        controller = (PlayerController)transform.GetComponent("PlayerController");
        move = 1f;
        target = 100f;
	}

    // Update is called once per frame
    void Update()
    {
        target = getEndTarget();

        if (jumping && controller.grounded)
        {
            jumping = false;
            controller.movespeed = 15f;
            move = 1f;
        }
        else if (jumping)
        {
            Trajectory bestJump = jumpPaths[0];
            foreach (Trajectory t in jumpPaths)
            {
                if (t.score > bestJump.score)
                {
                    bestJump = t;
                }
            }
            if(bestJump.speed < 0)
            {
                move = -1f;
            }
            else
            {
                move = 1f;
            }
            //target = bestJump.positions[bestJump.positions.Count - 1].x;
            controller.movespeed = bestJump.speed;
        }

        if (transform.position.x < target)
        {
            ObstacleCheck(perceptor.chasms);
            ObstacleCheck(perceptor.elevationIncrease);
            PlatformCheck(perceptor.platformStarts);
            controller.move = move;
        }
        if (transform.position.x >= target)
        {
           controller.move = 0f;
        }
        UpdateGenome();
    }

    void ObstacleCheck(List<Vector3> obstacleList)
    {
        if (!jumping)
        {
            foreach (Vector3 obstacle in obstacleList)
            {
                float obsDistance = obstacle.x - transform.position.x;
                if (obsDistance < obstacleLeapDistance && obsDistance > 0 && controller.grounded)
                {
                    jumpPaths = new List<Trajectory>();
                    controller.ControllerJump();
                    jumping = true;
                    GetTrajectory(25, 0.1f, new Vector2(15f, controller.jumpheight));
                    GetTrajectory(25, 0.1f, new Vector2(15 / 2, controller.jumpheight));
                    GetTrajectory(25, 0.1f, new Vector2(15 / 4, controller.jumpheight));
                }
            }
        }
    }

    void PlatformCheck(List<Vector3> platforms)
    {
        bool upcomingPlatform = false;
        Vector3 platformStart = transform.position;
        int platformScore = -1;
        int belowScore = 0;
        //Lets see if theres a platform coming up...
        foreach (Vector3 platform in platforms)
        {
            float obsDistance = platform.x - transform.position.x;
            if (obsDistance < platformLeapDistance && obsDistance > 0 && controller.grounded)
            {
                //Theres a platform immediately ahead of us. Now we have to decide if we're going to try and 
                //jump up onto it.
                upcomingPlatform = true;
                platformStart = platform;
                //Lets check to see if it has any coins on it first.  
                //This is very rudamentary at the moment. If there is a coin on the first block of the platform it is
                //assumed that the rest have coins. Likewise for below the platform. 
                foreach (Vector3 coin in perceptor.coins)
                {
                    if (coin.x == platform.x)
                    {
                        if (coin.y > platform.y)
                        {
                            platformScore += 3;
                        }
                        else if (coin.y < platform.y)
                        {
                            belowScore += 3;
                        }
                    }
                }
            }
        }

        if (upcomingPlatform && platformScore > belowScore)
        {
            //Platforms coming up and we want to jump up onto it.
            //So we need to calculate our trajectories...
            if (!jumping)
            {
                controller.ControllerJump();
                jumping = true;
            }
            Vector3 initialVelocity = new Vector2(controller.rb.velocity.x, controller.jumpheight);
            GetTrajectory(25, 0.1f, new Vector2(15f, controller.jumpheight));
            GetTrajectory(25, 0.1f, new Vector2(15 / 2, controller.jumpheight));
            GetTrajectory(25, 0.1f, new Vector2(15 / 4, controller.jumpheight));
            //We have some trajectories. Now we need to set the score of each based on which is closest to 
            //the start of the platform...
            Trajectory closest = jumpPaths[0];
            foreach (Trajectory trajectory in jumpPaths)
            {
                Vector3 jumpEnd = trajectory.positions[trajectory.positions.Count - 1];
                trajectory.distToTarget = platformStart.x - jumpEnd.x;
                if (trajectory.distToTarget < closest.distToTarget)
                {
                    trajectory.score = 10;
                    closest.score = -1;
                }
            }
        }
    }

    List<Vector3> GetTrajectory(int steps, float timeStep, Vector3 initial)
    {
        bool validLanding = false;
        List<Vector3> trajectoryPositions = new List<Vector3>();
        for (int i = 0; i < steps; i++)
        {
            float arcPositionTime = timeStep * i;
            Vector2 arcPosition = initial * arcPositionTime;
            arcPosition += 0.5f * Physics2D.gravity * (arcPositionTime * arcPositionTime);
            arcPosition += new Vector2(transform.position.x, transform.position.y);

            if(i > 1) //Skip the first few segments to avoid collisions with the ground/self
            {
                RaycastHit2D cast = Physics2D.Linecast(trajectoryPositions[i - 1], arcPosition);
                if(cast)
                {
                    if (cast.transform.name.Contains("ground_square")){
                        validLanding = true;
                        break;
                    }
                    else if (cast.transform.name.Contains("below_ground")) //Invalid if we aren't going to land on top of a ground tile
                    {
                        validLanding = false;
                        break;
                    }
                }
            }
            trajectoryPositions.Add(arcPosition);
        }
        //Creates a trajectory game object for jump path visualisation - for the user only, the agent doesnt use the line
        Transform trajectoryInstance = Transform.Instantiate(trajactoryPrefab, trajectoryPositions[0], Quaternion.identity);
        Trajectory trajectoryScript = (Trajectory)trajectoryInstance.GetComponent("Trajectory");
            trajectoryScript.positions = trajectoryPositions;
            trajectoryScript.validLanding = validLanding;
            trajectoryScript.speed = initial.x;
        LineRenderer lineRenderer = (LineRenderer)trajectoryInstance.GetComponent<LineRenderer>();
            lineRenderer.positionCount = trajectoryPositions.Count;
            lineRenderer.SetPositions(trajectoryPositions.ToArray());
        jumpPaths.Add(trajectoryScript);

        if (validLanding)
        {
            trajectoryScript.score = 1;//TODO assign score values 
        }
        return trajectoryPositions;
    }

    float getEndTarget()
    {
        if(transform.position.x < perceptor.goalPos)
        {
            return transform.position.x + 100;
        }
        else
        {
            return perceptor.goalPos;
        }
    }

    void UpdateGenome()
    {
        float score = transform.position.x + controller.score;
        if(score > genome.score)
        {
            genome.score = score;
        }
    }
}
