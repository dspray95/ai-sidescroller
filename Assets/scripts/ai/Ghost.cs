using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ghost : MonoBehaviour
{
    public Vector3 target;
    public float move = 1f;
    public float jumpHeight;
    public float moveSpeed;
    public PlayerController controller;
    public Rigidbody2D rb;

    private void Start()
    {
        move = 1f;
    }
}
