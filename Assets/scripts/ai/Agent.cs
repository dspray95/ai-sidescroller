using System.Collections.Generic;
using UnityEngine;

public class Agent : MonoBehaviour {

    Perceptor perceptor;
    PlayerController controller;
    float obstacleLeapDistance = 1;
    public bool clearingChasm = false;
    Vector3 lastChasmEnd;
    public Transform trajactoryPrefab;
    public List<Trajectory> jumpPaths;
    public bool jumping = false;
    public bool leftGround = false;
    public float movespeed;
	// Use this for initialization
	void Start () {
        perceptor = (Perceptor)transform.GetComponent("Perceptor");
        controller = (PlayerController)transform.GetComponent("PlayerController");
        movespeed = 1f;
	}

    // Update is called once per frame
    void Update()
    {
        float target = getEndTarget();
        if (jumping && controller.grounded)
        {
            jumping = false;
            controller.movespeed = 15;
        }
        if (transform.position.x < target)
        {
            ChasmCheck();
            controller.move = movespeed;
        }
        if (transform.position.x >= target)
        {
            controller.move = 0f;
        } 
    }
    

    List<Vector3> GetTrajectory(int steps, float timeStep, Vector3 initial)
    {
        bool validLanding = false;
        List<Vector3> trajectoryPositions = new List<Vector3>();
        for (int i = 0; i < steps; i++)
        {
            float t = timeStep * i;
            Vector2 currentpos = initial * t;
            currentpos += 0.5f * Physics2D.gravity * (t * t);
            currentpos += new Vector2(transform.position.x, transform.position.y);

            if(i > 5)
            {
                RaycastHit2D cast = Physics2D.Linecast(trajectoryPositions[i - 1], currentpos);
                if(cast)
                {
                    if (cast.transform.name.Contains("ground_square")){
                        Debug.Log("Trajectory ground hit");
                        validLanding = true;
                        break;
                    }
                    else if (cast.transform.name.Contains("below_ground")) //Invalid if we aren't going to land on top of a ground tile
                    {
                        Debug.Log("Trajectory below ground hit: " + initial.x);
                        validLanding = false;
                        break;
                    }
                }
            }
            trajectoryPositions.Add(currentpos);
        }
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

    void CaluculateTrajectory()
    {


    }
    void ChasmCheck()
    {
        if (!jumping)
        {
            foreach (Vector3 chasmStart in perceptor.chasms)
            {
                float chasmDistance = chasmStart.x - transform.position.x;
                if (chasmDistance < obstacleLeapDistance && chasmDistance > 0 && controller.grounded)
                {
                    Debug.Log("approaching chasm");
                    jumpPaths = new List<Trajectory>();
                    controller.ControllerJump();
                    jumping = true;
                    Vector3 initialVelocity = new Vector2(controller.rb.velocity.x, controller.jumpheight);
                    GetTrajectory(25, 0.1f, initialVelocity);
                    GetTrajectory(25, 0.1f, new Vector2(controller.rb.velocity.x / 2, controller.rb.velocity.y));
                }
            }
        }
        if(jumping)
        {
            Trajectory bestJump = jumpPaths[0];
            foreach (Trajectory t in jumpPaths)
            {
                if (t.score > bestJump.score)
                {
                    bestJump = t;
                }
            }
            Debug.Log("Best Jump: " + bestJump.speed);
            controller.movespeed = bestJump.speed;
        }
    }

    float getEndTarget()
    {
        List<Vector3> terrain = perceptor.terrain;
        //get highest X
        float highestX = 0f;
        for (int i = 0; i < terrain.Count; i++)
        {
            if (terrain[i].x > highestX)
            {
                highestX = terrain[i].x;
            }
        }
        return highestX;
    }
}
