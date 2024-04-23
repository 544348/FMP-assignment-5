using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    //VARIABLES
    [Header("Ground Movement")]
    public float speed;
    private Rigidbody2D rig;
    private float hoz;
    private float vert;
    private float baseSpeed;
    [Header("Jumping variables")]
    public float jumpStrength;
    private bool isGrounded;
    public LayerMask ground;
    public float raycastDistance;
    private bool hasCollectedDoublejumpCollectable;
    private GameObject doublejumpCollectable;
    public int amountOfExtraJumpsLeft;
    private bool isInAir;
    private bool hasJumped;
    public int amountOfExtraJumps;
    [Header("Wall jumping variables")]
    public float wallJumpTimerInterval;
    private bool isOnWall;
    private bool noControl;
    private bool wallJumpTimerActive;
    public float wallJumpForce;
    private bool flipped;
    private Vector3 defaultScale;
    [Header("Stage 2 speed")]
    public float stageTwoSpeedTimerInterval;
    private float stageTwoSpeedTimer;
    private bool stageTwoSpeedTimerActive;
    private Collider2D mostRecentNPCinteraction;
    void Start()
    {
        rig = GetComponent<Rigidbody2D>();
        amountOfExtraJumpsLeft = amountOfExtraJumps;
        doublejumpCollectable = GameObject.Find("DoubleJumpCollectable");
        defaultScale = transform.localScale;
        baseSpeed = speed;
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "DoubleJumpCollectable")
        {
            amountOfExtraJumps += 1;
            Destroy(collision.gameObject);
        }
        if (!isGrounded)
        {
            if(collision.gameObject.tag == "wall")
            {
                wallGrab();
            }
        }
        if (collision.gameObject.tag == "NPC")
        {
            collision.gameObject.GetComponent<Interact>().ActiveeInteract();
            mostRecentNPCinteraction = collision;
            if (Input.GetKeyDown(KeyCode.E))
            {
                collision.gameObject.GetComponent<Interact>().interactWithNPC();
                Debug.Log("player has interacted with NPC");
            }
        }
        Debug.Log("player has triggered " + collision);
    }
    private void OnTriggerStay2D(Collider2D other)
    {
        if(other.gameObject.tag == "NPC")
        {
            other.gameObject.GetComponent<Interact>().ActiveeInteract();
            if (Input.GetKeyDown(KeyCode.E))
            {
                other.gameObject.GetComponent<Interact>().interactWithNPC();
                Debug.Log("player has interacted with NPC");
            }
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "wall")
        {
            rig.gravityScale = 1;
            noControl = false;
            isOnWall = false;
        }
        Debug.Log("player has exited trigger " + collision);
        if (collision.gameObject.tag == "NPC")
        {
            collision.gameObject.GetComponent<Interact>().DisableeInteract();
            mostRecentNPCinteraction = null;
        }
    }
    private void FixedUpdate()
    {
        hoz = Input.GetAxisRaw("Horizontal") * speed;
        vert = Input.GetAxisRaw("Vertical");
        rig.velocity = new Vector2 (hoz, rig.velocity.y);
    }
    private void flip()
    {
        if (flipped == true)
        {
            gameObject.transform.localScale = defaultScale;
            flipped = false;
        }
        else if (flipped == false)
        {
            gameObject.transform.localScale = new Vector3(-1, 1, 1);
            flipped = true; 
        }
    }
    private void wallGrab()
    {
        rig.velocity = Vector2.zero;
        rig.gravityScale = 0;
        flip();
        isOnWall = true;
        noControl = true;
    }
    private void Jump()
    {
        if (isGrounded)
        {
            rig.velocity = new Vector2(rig.velocity.x, 0);
            rig.AddForce(Vector2.up * jumpStrength);
            Debug.Log("has jumped");
        }
        else if (amountOfExtraJumpsLeft > 0 && isOnWall == false)
        {
            rig.velocity = new Vector2(rig.velocity.x, 0);
            rig.AddForce(Vector2.up * jumpStrength);
             --amountOfExtraJumpsLeft;
            Debug.Log("has double jumped");
        }
        else if (isOnWall)
        {
            isOnWall = false;
            rig.gravityScale = 1;
            rig.AddForce(new Vector2((5 * wallJumpForce) * transform.localScale.x, 1 * jumpStrength));
            flip();
            hasJumped = true;
            Debug.Log("has wall jumped");
        }
        else if (!isGrounded && !isOnWall)
        {
            --amountOfExtraJumpsLeft;
            rig.velocity = new Vector2(rig.velocity.x, 0);
            rig.AddForce(Vector2.up * jumpStrength);
            Debug.Log("has double jumped");
        }
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
            else if(isInAir == true && amountOfExtraJumpsLeft != 0 || isOnWall)
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
        if (wallJumpTimerActive) 
        {
            float wallJumpTimer = 0;
            wallJumpTimer += Time.deltaTime;
            if (wallJumpTimer >= wallJumpTimerInterval) 
            {
                noControl = false;
                wallJumpTimer = 0;
                wallJumpTimerActive = false;
            }
        }
        if (noControl == false) 
        {
            if (Input.GetKeyDown(KeyCode.A)) 
            {
                flipped = true;
                gameObject.transform.localScale = new Vector3(-1, 1, 1);
            }
            else if (Input.GetKeyDown(KeyCode.D))
            {
                flipped = false;
                gameObject.transform.localScale = defaultScale;
            }
        }
        if (hoz != 0)
        {
            stageTwoSpeedTimerActive = true;
        }
        else if (hoz == 0)
        {
            stageTwoSpeedTimerActive = false;
            stageTwoSpeedTimer = 0;
            speed = baseSpeed;
        }
        if (stageTwoSpeedTimerActive)
        {
            stageTwoSpeedTimer += Time.deltaTime;
            if (stageTwoSpeedTimer >= stageTwoSpeedTimerInterval)
            {
                speed = baseSpeed * 2;
            }
        }
        if (Input.GetKeyDown(KeyCode.E) && mostRecentNPCinteraction != null)
        {
            mostRecentNPCinteraction.GetComponent<Interact>().interactWithNPC();
        }
    }
}
