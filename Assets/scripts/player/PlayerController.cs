using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {

    public Rigidbody2D rb;
    public float movespeed;
    public bool grounded;
    public Transform top_left;
    public Transform bottom_right;
    public Transform camera_pos;
    public LayerMask ground_layer;
    public int score;
    public Vector3 startPos;

    public int jumpheight;
        
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        score = 0;
        startPos = new Vector3(transform.position.x, transform.position.y + 3, transform.position.z);
    }

    void Update()
    {
        grounded = Physics2D.OverlapArea(top_left.position, bottom_right.position, ground_layer);
        
        float move = Input.GetAxis("Horizontal");
        rb.velocity = new Vector2(move * movespeed, rb.velocity.y);

        if (Input.GetKeyDown(KeyCode.UpArrow) && grounded)
        {
            rb.AddForce(new Vector2(0, jumpheight));
        }
        if(transform.position.y <= -50)
        {
            transform.position = startPos;
            score -= 10;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.tag == "coin")
        {
            score += 1;
            Destroy(collision.gameObject);
        }
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.tag == "enemy")
        {
            transform.position = startPos;
            //TODO: lost game state... 
        }
    }
    public int GetScore()
    {
        return this.score;
    }
}
