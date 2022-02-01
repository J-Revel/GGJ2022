using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Events;
using System.Collections;

public class Player : MonoBehaviour
{
    public Rigidbody skullPrefab;
    public static Player instance;
    private enum Facing { Left, Right };
    private bool permaDead = false;
    private bool isLiving = true;
    public bool isObserved = false;
    private Facing facing = Facing.Right;

    [SerializeField]
    private PlayerProperties deadProperties;

    [SerializeField]
    private PlayerProperties livingProperties;

    [SerializeField]
    private SoundPlayer soundPlayer;

    private PlayerProperties properties => !isLiving ? deadProperties : livingProperties;

    [SerializeField]
    private Rigidbody rigidbody;

    [SerializeField]
    private Collider collider;

    public bool IsObserved {
        get => IsObserved;
        set
        {
            if(isObserved = value)
            {
                return;
            }

            if (!isLiving)
            {
                this.permaDeadTime = 0f;
                this.animator.UpdatePermaRisk(value, 0f);
            }

            isObserved = value;
        }
    }

    [SerializeField]
    private SpriteRenderer renderer;

    [SerializeField]
    PlayerAnimator animator;

    //Raycast Cache
    Vector3 raycastPosition;
    float raycastLength = 0.2f;
    RaycastHit hit;
    Vector3 raycastDirection;
    Vector3 raycastNormal;
    float raycastWidth;

    [SerializeField]
    private LayerMask groundedLayerMask;

    //Inputs
    public InputAction jumpAction;
    public InputAction interactionAction;
    public InputAction switchStateAction;
    public InputAction horizontalAction;

    // Logic Input 
    private bool wantedJump;
    private bool wantedLongJump;
    private bool wantedInteraction;
    private bool wantedSwitch;
    private float wantedTranslation;

    //Logic State
    private bool isGrounded;
    private float timeSinceGrounded;
    private bool hasLeftWall;
    private bool hasRightWall;
    private bool isJumping;
    private bool isClimbing;

    private float permaDeadTime;

    private Vector2 currentJumpingDirection;
    private bool currentIsSideJump;
    private double currentJumpingTime;

    private float currentJumpForce => this.currentIsSideJump ? properties.sideJumpForce : properties.jumpForce;
    private float currentJumpImpulsion => this.currentIsSideJump ? properties.sideJumpImpulsion : properties.jumpImpulsion;
    private float currentMaxJumpingTime => this.currentIsSideJump ? properties.maxSideJumpingTime : properties.maxJumpingTime;
    private float currentJumpingNoneControlLimit => this.currentIsSideJump ? properties.sideJumpingNoneControlLimit : properties.jumpingNoneControlLimit;
    private float currentMinJumpingTime => this.currentIsSideJump ? properties.minSideJumpingTime : properties.minJumpingTime;
    private bool isInAir => !(this.isGrounded && !isJumping);

    public GameObject[] livingElements;
    public GameObject[] deadElements;
    public float riskAnimRatio { get { return this.permaDeadTime / properties.permaTime; }}

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
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

    private void Update()
    {
        if (permaDead)
        {
            return;
        }

        //Visual update according to state
        if (this.isGrounded)
        {
            if(Mathf.Abs(this.rigidbody.velocity.x) > 1f)
            {
                animator.SpriteState = PlayerAnimator.State.Walk;
                soundPlayer.WalkEvent?.Invoke(Mathf.Clamp(Mathf.Abs((this.rigidbody.velocity.x) - 1f) / 6f,0f,1f));
            }
            else
            {
                soundPlayer.WalkEvent?.Invoke(0f);
                animator.SpriteState = PlayerAnimator.State.Idle;
            }
        }
        else
        {
            soundPlayer.WalkEvent?.Invoke(0f);
            float speed = rigidbody.velocity.y;
            if(speed > 0.1f)
            {
                animator.SpriteState = PlayerAnimator.State.Jump;
            }
            else if (this.isClimbing)
            {
                animator.SpriteState = PlayerAnimator.State.Wall;
            }
            else if(speed < 0.1f)
            {
                animator.SpriteState = PlayerAnimator.State.Fall;
            }
            else
            {
                animator.SpriteState = PlayerAnimator.State.Float;
            }
        }
    }

    private void FixedUpdate()
    {
        float gravityForce = properties.gravityForce * Time.fixedDeltaTime;
        rigidbody.AddForce(Vector3.down * gravityForce);

        this.CheckFacing();
        this.CheckGround();

        if (this.isLiving)
        {
            LivingControl();
        }
        else
        {
            DeadControl();
        }
    }

    private void DeadControl()
    {
        this.CheckPermaDie();
        if (permaDead)
        {
            return;
        }

        //Try user interactions
        this.TrySwitch();
        this.TryDeadJump();

        if (isJumping)
        {
            float deltaJumpForce = this.currentJumpForce * Time.fixedDeltaTime;
            rigidbody.AddForce(currentJumpingDirection * deltaJumpForce);
        }

        if (!(isJumping && currentJumpingTime < currentJumpingNoneControlLimit))
        {
            float translationSpeed = this.isInAir ? this.properties.airSpeed : this.properties.speed;
            float deltaTranslationForce = this.wantedTranslation * translationSpeed * Time.fixedDeltaTime;
            rigidbody.AddForce(Vector3.right * deltaTranslationForce);
        }

        if (this.isClimbing && rigidbody.velocity.y < 0f)
        {
            float frictionForce = properties.gravityForce * Time.fixedDeltaTime * properties.frictionGravityRatio;
            rigidbody.AddForce(Vector3.up * frictionForce);
        }
    }

    private void LivingControl()
    {
        if (permaDead)
        {
            return;
        }

        //Check states
        this.CheckWall();

        //Try user interactions
        this.TrySwitch();
        this.TryLivingJump();

        if (!(isJumping && currentJumpingTime < currentJumpingNoneControlLimit))
        {
            float translationSpeed = this.isInAir ? this.properties.airSpeed : this.properties.speed;
            float deltaTranslationForce = this.wantedTranslation * translationSpeed * Time.fixedDeltaTime;
            rigidbody.AddForce(Vector3.right * deltaTranslationForce);
        }
        else
        {
            Debug.Log("BLOCK");
        }

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
    }

    private void TrySwitch()
    {
        if (this.wantedSwitch)
        {
            if (!isObserved)
            {
                this.isLiving = !this.isLiving;
                LivingStateManager.TriggerLifeChanges(this.isLiving);

                for(int i=0; i<livingElements.Length; i++)
                {
                    livingElements[i].SetActive(this.isLiving);
                }
                for(int i=0; i<deadElements.Length; i++)
                {
                    deadElements[i].SetActive(!this.isLiving);
                }
                soundPlayer.SwitchSoundEvent?.Invoke();

                //switch jump
                if (isLiving)
                {
                    this.isJumping = false; // Stop jump
                }
                else
                {
                    if (this.wantedLongJump) // Start floating if player continue pressing jump
                    {
                        this.isJumping = true;
                        this.wantedJump = false;
                    }
                }
            }
            else
            {
                animator.DisplayCantSwitchFeedback();
                soundPlayer.CannotSwitchSoundEvent?.Invoke();
            }
            this.wantedSwitch = false;
        }
    }

    private void CheckFacing()
    {
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

        if(facingFactor > 0.2f && this.facing == Facing.Left)
        {
            this.ChangeFacing(Facing.Right);
        }
        else if(facingFactor < -0.2f && this.facing == Facing.Right)
        {
            this.ChangeFacing(Facing.Left);
        }
    }

    private void ChangeFacing(Facing actualFacing)
    {
        this.facing = actualFacing;
        this.renderer.flipX = (this.facing == Facing.Left);
    }

    private void CheckWall()
    {
        raycastNormal = Vector3.up;
        raycastWidth = properties.height * 1.2f;

        // Left Wall detection
        raycastPosition = this.transform.position + Vector3.left * properties.width * 0.4f; ;
        raycastDirection = Vector3.left;
        this.RaycastGroundable(out float minHitDistance);

        this.hasLeftWall = minHitDistance < raycastLength;

        // Right Wall detection
        raycastPosition = this.transform.position + Vector3.right * properties.width * 0.4f; ;
        raycastDirection = Vector3.right;
        this.RaycastGroundable(out minHitDistance);

        this.hasRightWall = minHitDistance < raycastLength;

        //Check Climbing 
        this.isClimbing = this.hasLeftWall && facing == Facing.Left || this.hasRightWall && facing == Facing.Right;
    }

    private void TryLivingJump()
    {
        if (!this.isJumping)
        {
            if (!this.isGrounded && timeSinceGrounded > properties.coyoteTime && !this.isJumping && !(this.hasLeftWall || this.hasRightWall)) 
            {
                // Player has finished jumping and is falling
                // Player interactions should not not interpreted
            }
            else if(this.isGrounded || timeSinceGrounded < properties.coyoteTime)
            {
                // Player is not jumping and is grounded
                // Player interactions can trigger a jump
                if (this.wantedJump)
                {
                    this.currentJumpingDirection = Vector2.up;
                    this.currentIsSideJump = false;
                    this.Jump();
                    soundPlayer.JumpSoundEvent.Invoke();
                }
            }
            else if (!this.isGrounded && (this.hasLeftWall || this.hasRightWall))
            {
                // Player is not jumping, not grounded, but is near a wall surface
                // Player interactions can trigger a side jump
                if (this.wantedJump)
                {
                    this.currentJumpingDirection = (this.hasLeftWall) ? new Vector2(0.5f,1f): new Vector2(-0.5f, 1f);
                    this.currentIsSideJump = true;
                    soundPlayer.WallJumpSoundEvent.Invoke();
                    this.Jump();
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
            }
        }
    }

    private void TryDeadJump()
    {
        if (!this.isJumping)
        {
            if (this.isGrounded || timeSinceGrounded < properties.coyoteTime)
            {
                // Player is not jumping and is grounded
                // Player interactions can trigger a jump
                if (this.wantedJump)
                {
                    this.currentJumpingDirection = Vector2.up;
                    this.currentIsSideJump = false;
                    this.Jump();
                    soundPlayer.JumpSoundEvent.Invoke();
                }
            }
            else if (!this.isGrounded)
            {
                // Player has finished jumping and is falling
                // Player can float
                if (this.wantedJump)
                {
                    this.currentJumpingDirection = Vector2.up;
                    this.currentIsSideJump = true;
                    this.Jump();
                }
            }
        }
        else
        {
            // Should player continue jumping ?
            // Depends on player interaction and min and max time of jump.
            this.currentJumpingTime += Time.deltaTime;
            if (!this.wantedLongJump && this.currentJumpingTime > currentMinJumpingTime)
            {
                this.isJumping = false;
            }
        }
    }

    private void Jump()
    {
        this.wantedJump = false;
        this.isJumping = true;
        rigidbody.AddForce(currentJumpingDirection * currentJumpImpulsion, ForceMode.Impulse);
        this.currentJumpingTime = 0f;
        animator.SpriteState = PlayerAnimator.State.Jump;
    }

    private void CheckGround()
    {
        raycastPosition = this.transform.position + Vector3.down * properties.height * 0.4f;
        raycastDirection = Vector3.down;
        raycastNormal = Vector3.left;
        raycastWidth = properties.width * 1.2f;
        this.RaycastGroundable(out float minHitDistance);

        bool groundDetected = minHitDistance < raycastLength;

        if( groundDetected && !this.isGrounded)
        {
            soundPlayer.GroundSoundEvent?.Invoke();
        }

        this.isGrounded = groundDetected;
        if (isGrounded)
        {
            timeSinceGrounded = 0f;
        }
        else
        {
            timeSinceGrounded += Time.deltaTime;
        }
    }

    private void CheckPermaDie()
    {
        if (!isLiving && isObserved && !this.permaDead)
        {
            this.permaDeadTime += Time.deltaTime;
            this.animator.UpdatePermaRisk(true, this.permaDeadTime / properties.permaTime);
        }
        else
        {
            this.permaDeadTime -= Time.deltaTime;
            if(permaDeadTime < 0)
            {
                permaDeadTime = 0;
            }
        }

        if (permaDeadTime > properties.permaTime)
        {
            this.permaDeadTime = 0;
            PermaDie();
        }
    }

    private void PermaDie()
    {
        this.permaDead = true;
        StartCoroutine(PermaDeathAnimCoroutine());
        animator.TriggerPermaDeath();
    }
    public IEnumerator PermaDeathAnimCoroutine()
    {
        animator.gameObject.SetActive(false);
        Instantiate(skullPrefab, transform.position, Quaternion.identity).velocity = new Vector3(Random.Range(-2, 2), 5, 0);
        yield return new WaitForSeconds(3);
        
        LevelManager.Reset();
    }

    private bool RaycastGroundable(out float minHitDistance, int raycastCount = 11)
    {
        minHitDistance = raycastLength;

        for (int i = 0; i < raycastCount; i++)
        {
            float relativeOffset = (((float)raycastCount / 2f) - i) / (float)raycastCount;
            Vector3 offset = raycastNormal * relativeOffset * raycastWidth;
            bool hitted = Physics.Raycast(raycastPosition + offset, raycastDirection, out hit, raycastLength, groundedLayerMask);
            Debug.DrawRay(raycastPosition + offset, raycastDirection * raycastLength, hitted ? Color.green : Color.red, 0.05f, false);
            if (hitted && hit.distance < minHitDistance)
            {
                minHitDistance = hit.distance;
            }
        }

        return minHitDistance < raycastLength;
    }
}
