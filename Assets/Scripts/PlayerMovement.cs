using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    //VARIABLES
    public float speed;
    private Rigidbody2D rig;
    private float hoz;
    private float vert;
    public float jumpStrength;
    private bool isGrounded;
    public LayerMask ground;
    public float raycastDistance;
    private bool hasCollectedDoublejumpCollectable;
    private GameObject doublejumpCollectable;
    private int amountOfExtraJumpsLeft = 1;
    private bool isInAir;
    public int amountOfExtraJumps;
    void Start()
    {
        rig = GetComponent<Rigidbody2D>();
        amountOfExtraJumpsLeft = amountOfExtraJumps;
        doublejumpCollectable = GameObject.Find("DoubleJumpCollectable");
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "DoubleJumpCollectable")
        {
            amountOfExtraJumps += 1;
            Destroy(collision.gameObject);
        }
    }
    private void FixedUpdate()
    {
        hoz = Input.GetAxisRaw("Horizontal") * speed;
        vert = Input.GetAxisRaw("Vertical");
        rig.velocity = new Vector2 (hoz, rig.velocity.y);
    }
    private void Jump()
    {
        rig.AddForce(Vector2.up * jumpStrength);
        --amountOfExtraJumpsLeft;
    }
    private void GroundCheck()
    {
        isGrounded = Physics2D.Raycast(gameObject.transform.position, Vector2.down, raycastDistance, ground);
        Debug.DrawLine(gameObject.transform.position, new Vector2(gameObject.transform.position.x, gameObject.transform.position.y - raycastDistance), Color.green);
    }
    void Update()
    {
        GroundCheck();
        if (Input.GetKeyDown(KeyCode.Space))
        {

            if (isGrounded == true)
            {
                Jump();
            }
            else if(isInAir == true && amountOfExtraJumpsLeft != 0)
            {
                Jump();
                if (amountOfExtraJumpsLeft < 0)
                {
                    amountOfExtraJumpsLeft = 0;
                }
            }
        }
        if(isGrounded == false)
        {
            isInAir = true;
        }
        else if (isGrounded == true)
        {
            isInAir = false;
            amountOfExtraJumpsLeft = amountOfExtraJumps;
        }
        if(isGrounded == true && isInAir == true)
        {
            amountOfExtraJumps = amountOfExtraJumpsLeft;
        }
    }
}
