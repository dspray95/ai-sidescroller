using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameState : MonoBehaviour {

    public Text lblTime;
    public Text lblScore;
    public PlayerController pc;
    private float levelTime;
    public Map map; 
    public MapBuilder builder;
    public Camera mainCamera;

    // Use this for initialization
    void Start () {
        this.levelTime = 0;
        this.map = new Map(250);
        BuildMap();
        GameObject player = GameObject.Find("player");
        this.pc = (PlayerController) player.GetComponent("PlayerController");
        mainCamera.transform.Translate(new Vector3(player.transform.position.x, player.transform.position.y, mainCamera.transform.position.z));

        CameraController cc = (CameraController)mainCamera.GetComponent("CameraController");
        cc.SetPlayer(player);
	}
	
    private void BuildMap()
    { 
        List<int> heights = map.GenerateGroundHeights();
        List<MapFeature> mobs = map.GenerateMobs();
        List<MapFeature> features = map.GenerateFeatures();
        builder.BuildMap(heights, features, mobs);
    }
	// Update is called once per frame
	void Update () {
        levelTime += Time.deltaTime;
        lblTime.text = Mathf.Round(levelTime).ToString();
        lblScore.text = pc.GetScore().ToString();
    }
}
