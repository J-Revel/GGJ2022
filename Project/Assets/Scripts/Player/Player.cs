using System;
using UnityEngine;

public class Player : MonoBehaviour
{
    private bool isDead;
    private bool isObserved;

    [SerializeField]
    private PlayerProperties deadProperties;

    [SerializeField]
    private PlayerProperties livingProperties;

    private PlayerProperties properties => isDead ? deadProperties : livingProperties;

    [SerializeField]
    private Rigidbody rigidbody;

    [SerializeField]
    private Collider collider;

    [SerializeField]
    private SpriteRenderer renderer;

    [SerializeField]
    private Transform groundDetector;

    //Utils
    private int layerMask;

    private void Start()
    {
        this.layerMask = 1 << LayerMask.NameToLayer("Groundable");
    }

    // Logic Input 
    private bool wantedJump;
    private float wantedTranslation;

    //Logic State
    private bool isGrounded;
    private bool isJumping;
    private float jumpingTime;
    private bool isInAir() => !(this.isGrounded && !isJumping);

    private void Update()
    {
        this.wantedTranslation = Input.GetAxis("Horizontal");

        float jumpValue = Input.GetAxis("Vertical");
        if(jumpValue > 0)
        {
            this.wantedJump = true;
        }
        else
        {
            this.wantedJump = false;
        }
    }

    private void FixedUpdate()
    {
        //Check if Grounded
        CheckGround();
        CheckJump();

        float translationSpeed = this.isInAir() ? this.properties.airSpeed : this.properties.speed;
        float deltaTranslationForce = this.wantedTranslation * translationSpeed * Time.fixedDeltaTime;
        rigidbody.AddForce(Vector3.right * deltaTranslationForce);

        if (isJumping)
        {
            float deltaJumpForce = properties.jumpForce * Time.fixedDeltaTime;
            rigidbody.AddForce(Vector3.up * deltaJumpForce);
        }

        float gravityForce = properties.gravityForce * Time.fixedDeltaTime;
        rigidbody.AddForce(Vector3.down * gravityForce);
    }

    private void CheckJump()
    {
        if (this.isGrounded && !isJumping) // The player is grounded and not jumping yet, check input
        {
            if (wantedJump)
            {
                isJumping = true;
                jumpingTime = 0f;
            }
        }
        else if (!this.isGrounded && !isJumping) // Player has finished the jump and is falling
        {
            // Wait 
        }
        else if(isJumping)
        {
            jumpingTime += Time.fixedDeltaTime;
            if (jumpingTime > this.properties.maxJumpingTime || !this.wantedJump && jumpingTime > this.properties.minJumpingTime)
            {
                isJumping = false;
            }
        }
    }

    private void CheckGround()
    {
        Vector3 raycastPosition = groundDetector.position;
        float raycastLength = 0.2f;
        RaycastHit hit;

        float minHitDistance = raycastLength;

        //TODO do multiple raycast to enhance precision
        for (int i = 0; i < 1; i++)
        {
            bool hitted = Physics.Raycast(raycastPosition, Vector3.down, out hit, raycastLength, layerMask);
            Debug.DrawRay(raycastPosition, Vector3.down * raycastLength, hitted ? Color.green : Color.red, 0.2f, false);
            if (hitted && hit.distance < minHitDistance)
            {
                minHitDistance = hit.distance;
            }
        }
        
        this.isGrounded = minHitDistance < raycastLength;
    }
}
