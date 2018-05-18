using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameState : MonoBehaviour {

    public Text lblTime;
    public Text lblScore;
    public PlayerController pc;
    private float levelTime;
    public LevelBuilder builder;
    public Camera mainCamera;
    public bool createPlayer = false;
    public Genographer genographer;
    public Transform player;
    public List<Agent> agents;
    public CameraController cc;
    // Use this for initialization
    void Start () {
        CreateLevel();
        cc = (CameraController)mainCamera.GetComponent("CameraController");
        if (createPlayer)
        {
            GameObject player = GameObject.Find("player");
            this.pc = (PlayerController)player.GetComponent("PlayerController");
            mainCamera.transform.Translate(new Vector3(player.transform.position.x, player.transform.position.y, mainCamera.transform.position.z));
            cc.SetPlayer(player);
        }
        else
        {
            genographer = new Genographer(10, 30);
            CreateAgents();
        }
    }

    private void CreateLevel()
    {
        this.levelTime = 0;
        Level level = new Level(250);
            level.GenerateHeights();
            level.GenerateFeatures();
            level.GenerateMobs();
        builder.BuildLevel(level.heights, level.features, level.mobs, level.goalPos, false);
    }
	// Update is called once per frame
	void Update () {
        levelTime += Time.deltaTime;
        lblTime.text = Mathf.Round(levelTime).ToString();
        //lblScore.text = pc.GetScore().ToString();
        if (levelTime > genographer.generationTime)
        {
            genographer.CreateNextGeneration();
            DestroyAgents();
            CreateAgents();
            levelTime = 0;
        }
        else
        {
            //get the best agent and focus the camera on it
            Agent leadingAgent = agents[0];
            foreach (Agent agent in agents)
            {
                if (agent.transform.position.x > leadingAgent.transform.position.x)
                {
                    leadingAgent = agent;
                }
            }
            cc.SetPlayer(leadingAgent.transform.gameObject);
        }
    }

    void CreateAgents()
    {
        agents = new List<Agent>();
        foreach (Genome genome in genographer.genomes)
        {
            //For every genome we need to do a few things.
            //Firstly we need to give it an instance in the world
            //Then we need to set its perceptor up, this involves setting it's perceptor to function from
            //the main camera rather than from it's own perceptor
            Transform ai = Transform.Instantiate(player, new Vector3(4, 1, 0), Quaternion.identity);
            Agent agent = (Agent)ai.GetComponent("Agent");
            Perceptor perceptor = (Perceptor)ai.GetComponent("Perceptor");
            perceptor.geneticAlgorithm = true;
            agent.perceptor = (Perceptor)cc.GetComponent("Perceptor");
            agent.obstacleLeapDistance = genome.obstacleToInt();
            agent.platformLeapDistance = genome.platformToInt();
            agent.genome = genome;
            agents.Add(agent);
        }
        cc.SetPlayer(agents[agents.Count - 1].transform.gameObject);
    }

    void DestroyAgents()
    {
        foreach(Agent agent in agents)
        {
            Destroy(agent.transform.gameObject);
        }
    }
}
