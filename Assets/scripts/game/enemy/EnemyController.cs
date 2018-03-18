using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour {


    private EnemyState state;
    private int patrolLocation = 0;
    private Rigidbody2D rb;
    public bool grounded;
    public int patrolSize;
    public int moveSpeed;
    public int jumpHeight;
    public float detectionRange;
    public Transform raycastRight;
    public Transform raycastRightHigh;
    public Transform raycastRightGround;
    public Transform raycastLeft;
    public Transform raycastLeftHigh;
    public Transform raycastLeftGround;
    public Transform overlapTopLeft;
    public Transform overlapBottomRight;
    public LayerMask raycastMask;
    public LayerMask groundedMask;
    bool direction = true; //f = l, t = r;

                           // Use this for initialization
    void Start () {
        state = EnemyState.PATROL; //Default state is patrol left to right
        rb = GetComponent<Rigidbody2D>();
    }
	
	// Update is called once per frame
	void Update () {

        grounded = Physics2D.OverlapArea(overlapTopLeft.position, overlapBottomRight.position, groundedMask);

        switch (state)
        {
            case (EnemyState.PATROL):

                Patrol();
                break;
        }
    }

    public void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.tag == "enemy")
        {
            direction = !direction;
        }
    }
    /**Moves left and right regularly
     * 
    */

    private void Patrol()
    {
        RaycastHit2D hitMid;
        RaycastHit2D hitGround;

        //Depending on the direction;
        //set velocity to move in direction
        //raycasts both for wall checks and for edge of platform checks
        //increment/decrement patrol location
        if (direction)
        {
            rb.velocity = new Vector2(moveSpeed, rb.velocity.y);
            hitMid = Physics2D.Raycast(raycastRight.transform.position, -Vector2.right, 0.085f, raycastMask); 
            hitGround = Physics2D.Raycast(raycastRightGround.transform.position, -Vector2.right, 0.085f, groundedMask);
            patrolLocation--;
        }
        else
        {
            rb.velocity = new Vector2(-moveSpeed, rb.velocity.y);
            hitMid = Physics2D.Raycast(raycastLeft.transform.position, -Vector2.left, 0.085f, raycastMask);
            hitGround = Physics2D.Raycast(raycastLeftGround.transform.position, -Vector2.right, 0.085f, groundedMask);
            patrolLocation++;
        }
        //Platform edge check; turn around if our ground raycast doenst detect any ground ahead
        if(hitGround.collider == null && grounded)
        {
            direction = !direction;
        }

        //Checking if there is a wall we need to jump over
        if (hitMid.collider != null)
        {
            //This uses a raycast from a point above the enemy sprite to see if the wall is too high
            RaycastHit2D highHit;
            if (direction)
            {
                highHit = Physics2D.Raycast(raycastRightHigh.transform.position, -Vector2.right, 0.085f, groundedMask);
            }
            else
            {
                highHit = Physics2D.Raycast(raycastLeftHigh.transform.position, -Vector2.left, 0.085f, groundedMask);
            }
            //If the wall is too high turn around
            //Otherwise jump up onto it;
            if(highHit.collider != null)
            {
                direction = !direction;
            }
            else
            {
                rb.velocity = new Vector2(rb.velocity.x, jumpHeight);
            }
        }
    }

  
}
