using System.Collections;
using System.Collections.Generic;
using UnityEditor.U2D.Sprites;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

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
    public GameObject RaycastRight;
    public GameObject RaycastLeft;
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
    public GameObject QuestWall;
    [Header("Dialogue")]
    public Sprite playersFace;
    public bool playerIsInteracting;

    public Animator animator;


    bool grounded = false;

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
        if (collision.gameObject.tag == "NPC" && collision.gameObject.GetComponent<Interact>().isBeingInteractedWith == false)
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

        if (collision.gameObject.tag == "CutsceneTrigger")
        {
            SceneManager.LoadScene("FallingCutscene");
        }
        if (collision.gameObject.tag == "DeathTag")
        {
            Debug.Log("Player should die and teleport back");
        }
        if (collision.gameObject.tag == "FinishedQuestTrigger")
        {
            Destroy(collision.gameObject);
            QuestWall.SetActive(false);
        }
    }
    private void OnTriggerStay2D(Collider2D other)
    {
        if(other.gameObject.tag == "NPC" && other.gameObject.GetComponent<Interact>().isBeingInteractedWith == false)
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
        if (!playerIsInteracting)
        {
            hoz = Input.GetAxisRaw("Horizontal") * speed;
            vert = Input.GetAxisRaw("Vertical");
            rig.velocity = new Vector2(hoz, rig.velocity.y);


          


        }
    }


    private void setAnimation(Vector2 vector)
    {
        Debug.Log("x"+rig.velocity.x);
        Debug.Log("y"+rig.velocity.y);

        if (vector.x < 0)
        {
            animator.SetBool("move", true);
        }
        else if (vector.x > 0)
        {
            animator.SetBool("move", true);
        }
        else 
        {
            animator.SetBool("move", false);
        }
    }

    private void flip()
    {
        if (!playerIsInteracting)
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
        if (!playerIsInteracting)
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
    }
    private void GroundCheck()
    {
       


        // Define the corners of the overlap area
        Vector2 corner1 = new Vector2(gameObject.transform.position.x - 0.5f, gameObject.transform.position.y - 0.1f);
        Vector2 corner2 = new Vector2(RaycastLeft.transform.position.x + 0.5f, gameObject.transform.position.y - raycastDistance);

        // Check for overlap in the defined area
        Collider2D[] colliders = Physics2D.OverlapAreaAll(corner1, corner2);

        // Check if any colliders are found
        if (colliders.Length > 0)
        {
            // Object is grounded
            Debug.Log("Grounded!");
        }
        else
        {
            // Object is not grounded
            Debug.Log("Not Grounded!");
        }


        isGrounded = Physics2D.Raycast(RaycastRight.transform.position, Vector2.down, raycastDistance, ground) || Physics2D.Raycast(RaycastLeft.transform.position, Vector2.down, raycastDistance, ground);
        Debug.DrawLine(gameObject.transform.position, new Vector2(gameObject.transform.position.x, gameObject.transform.position.y - raycastDistance), Color.green);
    }
    void Update()
    {

        //Debug.Log(rig.velocity);
        setAnimation(rig.velocity);


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
                speed = baseSpeed * 1.5f;
            }
        }
        if (Input.GetKeyDown(KeyCode.E) && mostRecentNPCinteraction != null && mostRecentNPCinteraction.gameObject.GetComponent<Interact>().isBeingInteractedWith == false)
        {
            mostRecentNPCinteraction.GetComponent<Interact>().interactWithNPC();
            mostRecentNPCinteraction.GetComponent<Interact>().eInteract.SetActive(false);
            playerIsInteracting = true;
        }
    }
}
