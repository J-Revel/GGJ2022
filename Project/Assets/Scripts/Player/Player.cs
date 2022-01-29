using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    private enum Facing { None, Left, Right };
    private bool isLiving = true;
    private bool isObserved = false;
    private Facing facing = Facing.None;

    [SerializeField]
    private PlayerProperties deadProperties;

    [SerializeField]
    private PlayerProperties livingProperties;

    private PlayerProperties properties => !isLiving ? deadProperties : livingProperties;

    [SerializeField]
    private Rigidbody rigidbody;

    [SerializeField]
    private Collider collider;

    [SerializeField]
    private SpriteRenderer renderer;

    [SerializeField]
    private Transform groundDetector;
    [SerializeField]
    private Transform leftWallDetector;
    [SerializeField]
    private Transform rightWallDetector;

    //Raycast Cache
    private int layerMask;
    Vector3 raycastPosition;
    float raycastLength = 0.2f;
    RaycastHit hit;
    Vector3 raycastDirection;

    //Inputs
    public InputAction jumpAction;
    public InputAction interactionAction;
    public InputAction switchStateAction;
    public InputAction horizontalAction;

    private void Start()
    {
        this.layerMask = 1 << LayerMask.NameToLayer("Groundable");

        jumpAction.started += context =>
        {
            wantedJump = true;
            wantedLongJump = true;
        };
        jumpAction.canceled += context =>
        {
            wantedJump = false;
            wantedLongJump = false;
        };

        interactionAction.started += context => wantedInteraction = true;
        interactionAction.canceled += context => wantedInteraction = false;

        switchStateAction.started += context => wantedSwitch = true;
        switchStateAction.canceled += context => wantedSwitch = false;

        horizontalAction.performed += context => { this.wantedTranslation = context.ReadValue<float>(); };
        horizontalAction.canceled += context => this.wantedTranslation = 0f;

        jumpAction.Enable();
        interactionAction.Enable();
        switchStateAction.Enable();
        horizontalAction.Enable();
    }

    // Logic Input 
    private bool wantedJump;
    private bool wantedLongJump;
    private bool wantedInteraction;
    private bool wantedSwitch;
    private float wantedTranslation;

    //Logic State
    private bool isGrounded;
    private bool hasLeftWall;
    private bool hasRightWall;
    private bool isJumping;
    private bool isClimbing;

    private Vector2 currentJumpingDirection;
    private bool currentIsSideJump;
    private double currentJumpingTime;

    private float currentJumpForce => this.currentIsSideJump ? properties.sideJumpForce : properties.jumpForce;
    private float currentMaxJumpingTime => this.currentIsSideJump ? properties.maxSideJumpingTime : properties.maxJumpingTime;
    private float currentMinJumpingTime => this.currentIsSideJump ? properties.minSideJumpingTime : properties.minJumpingTime;
    private bool isInAir => !(this.isGrounded && !isJumping);

    private void Update()
    {
        Debug.Log(horizontalAction.ReadValue<float>() + " " + horizontalAction.ReadValue<float>());
    }

    private void FixedUpdate()
    {
        //Check states
        this.CheckFacing();
        this.CheckGround();
        this.CheckWall();
        this.CheckObserved();

        //Try user interactions
        this.TryInteract();
        this.TrySwitch();
        this.TryJump();

        //Apply Forces
        float translationSpeed = this.isInAir ? this.properties.airSpeed : this.properties.speed;
        float deltaTranslationForce = this.wantedTranslation * translationSpeed * Time.fixedDeltaTime;
        rigidbody.AddForce(Vector3.right * deltaTranslationForce);

        if (isJumping)
        {
            float deltaJumpForce = this.currentJumpForce * Time.fixedDeltaTime;
            rigidbody.AddForce(currentJumpingDirection * deltaJumpForce);
        }

        if (this.isClimbing && rigidbody.velocity.y < 0f)
        {
            float frictionForce = properties.gravityForce * Time.fixedDeltaTime * properties.frictionGravityRatio;
            rigidbody.AddForce(Vector3.up * frictionForce);
        }

        float gravityForce = properties.gravityForce * Time.fixedDeltaTime;
        rigidbody.AddForce(Vector3.down * gravityForce);
    }

    private void TrySwitch()
    {
        if (this.wantedSwitch)
        {
            if (!isObserved)
            {
                this.isLiving = !this.isLiving;
                LivingStateManager.TriggerLifeChanges(this.isLiving);
                //TODO Change visual state according to isDead value
                this.gameObject.GetComponent<SpriteRenderer>().color = !this.isLiving ? Color.black : Color.white;
            }
            else
            {
                //TODO Add impossible state switch feedback
            }
            this.wantedSwitch = false;
        }

    }

    private bool CheckObserved()
    {
        //TODO change this.isObserved state
        //TODO change sprite
        return false;
    }

    private void TryInteract()
    {
        if (this.wantedInteraction)
        {
            //TODO Raycast to find interaction
            this.wantedInteraction = false;
        }
    }

    private void CheckFacing()
    {
        Facing actualFacing;
        float facingFactor;

        // Switch facing depending on user interaction or velocity by default.
        if(this.wantedTranslation == 0f)
        {
            facingFactor = rigidbody.velocity.x;
        }
        else
        {
            facingFactor = this.wantedTranslation;
        }

        if(facingFactor > 0f)
        {
            actualFacing = Facing.Right;
        }
        else if(facingFactor < 0f)
        {
            actualFacing = Facing.Left;
        }
        else
        {
            actualFacing = Facing.None;
        }
        this.TryChangeFacing(actualFacing);
    }

    private bool TryChangeFacing(Facing actualFacing)
    {
        if (this.facing != actualFacing)
        {
            this.facing = actualFacing;
            //TODO Do Sprite Changes
            return true;
        }
        return false;
    }

    private void CheckWall()
    {
        // Left Wall detection
        raycastPosition = leftWallDetector.position;
        raycastDirection = Vector3.left;
        float minHitDistance = raycastLength;

        //TODO Do multiple raycast to enhance precision
        for (int i = 0; i < 1; i++)
        {
            bool hitted = Physics.Raycast(raycastPosition, raycastDirection, out hit, raycastLength, layerMask);
            Debug.DrawRay(raycastPosition, raycastDirection * raycastLength, hitted ? Color.green : Color.red, 0.2f, false);
            if (hitted && hit.distance < minHitDistance)
            {
                minHitDistance = hit.distance;
            }
        }

        this.hasLeftWall = minHitDistance < raycastLength;

        // Right Wall detection
        raycastPosition = rightWallDetector.position;
        raycastDirection = Vector3.right;

        minHitDistance = raycastLength;

        //TODO Do multiple raycast to enhance precision
        for (int i = 0; i < 1; i++)
        {
            bool hitted = Physics.Raycast(raycastPosition, raycastDirection, out hit, raycastLength, layerMask);
            Debug.DrawRay(raycastPosition, raycastDirection * raycastLength, hitted ? Color.green : Color.red, 0.2f, false);
            if (hitted && hit.distance < minHitDistance)
            {
                minHitDistance = hit.distance;
            }
        }

        this.hasRightWall = minHitDistance < raycastLength;

        //Check Climbing 
        this.isClimbing = this.hasLeftWall && facing == Facing.Left || this.hasRightWall && facing == Facing.Right;
    }

    private void TryJump()
    {
        if (!this.isJumping)
        {
            if (!this.isGrounded && !this.isJumping && !(this.hasLeftWall || this.hasRightWall)) 
            {
                // Player has finished jumping and is falling
                // Player interactions should not not interpreted
            }
            else if(this.isGrounded)
            {
                // Player is not jumping and is grounded
                // Player interactions can trigger a jump
                if (this.wantedJump)
                {
                    this.wantedJump = false;
                    this.isJumping = true;
                    this.currentJumpingDirection = Vector2.up;
                    this.currentIsSideJump = false;
                    float deltaJumpForce = this.currentJumpForce / 500f;
                    rigidbody.AddForce(currentJumpingDirection * deltaJumpForce, ForceMode.Impulse);
                    this.currentJumpingTime = 0f;
                }
            }
            else if (!this.isGrounded && (this.hasLeftWall || this.hasRightWall))
            {
                // Player is not jumping, not grounded, but is near a wall surface
                // Player interactions can trigger a side jump
                if (this.wantedJump)
                {
                    this.wantedJump = false;
                    this.isJumping = true;
                    this.currentJumpingDirection = (this.hasLeftWall) ? new Vector2(0.5f,1f): new Vector2(-0.5f, 1f);
                    this.currentIsSideJump = true;
                    float deltaJumpForce = this.currentJumpForce / 500f;
                    rigidbody.AddForce(currentJumpingDirection * deltaJumpForce, ForceMode.Impulse);
                    this.currentJumpingTime = 0f;
                }
            }
        }
        else
        {
            // Should player continue jumping ?
            // Depends on player interaction and min and max time of jump.
            this.currentJumpingTime += Time.deltaTime;
            if (this.currentJumpingTime > currentMaxJumpingTime || !this.wantedLongJump && this.currentJumpingTime > currentMinJumpingTime)
            {
                this.isJumping = false;
                this.wantedLongJump = false;
            }
        }
        if (this.isGrounded && !this.isJumping) // The player is grounded and not jumping yet, check input
        {

        }
        else if (!this.isGrounded && !this.isJumping) // Player has finished the jump and is falling
        {
            // Wait 
        }
        else
        {

        }
    }

    private void CheckGround()
    {
        raycastPosition = groundDetector.position;
        raycastDirection = Vector3.down;

        float minHitDistance = raycastLength;

        //TODO Do multiple raycast to enhance precision
        for (int i = 0; i < 1; i++)
        {
            bool hitted = Physics.Raycast(raycastPosition, raycastDirection, out hit, raycastLength, layerMask);
            Debug.DrawRay(raycastPosition, raycastDirection * raycastLength, hitted ? Color.green : Color.red, 0.2f, false);
            if (hitted && hit.distance < minHitDistance)
            {
                minHitDistance = hit.distance;
            }
        }
        
        this.isGrounded = minHitDistance < raycastLength;
    }
}
