using System;
using System.Collections;
using UnityEngine;

public class PlayerMovement : MonoBehaviour {

    public bool isWallSliding;
    public float wallSlidingSpeed;
    public bool isWallJumping;
    private float wallJumpingDirection;
    public float wallJumpingTime = 0.05f;
    private float wallJumpingCounter;
    public float wallJumpingDuration = 0.15f;
    public Vector2 wallJumpingPower = new Vector2(2f, 5f);

    public bool isTouchingWall;
    public bool canDash = true;
    public bool isDashing;
    public float dashPower = 3f;
    public float dashTime = 0.2f;
    public float dashCooldown = 1f;

    public float acceleration;
    public float groundSpeed;
    public float jumpSpeed;
    [Range(0f, 1f)]
    public float dragDecay;
    public bool grounded;
    public Rigidbody2D body;
    public BoxCollider2D groundCheck;
    public LayerMask groundMask;
    public BoxCollider2D wallCheck;
    public LayerMask wallMask;
    float xInput;
    float yInput;

    void FixedUpdate() {
        if (isDashing || isWallJumping) return;
        CheckGround();
        CheckWallTouch();
        ApplyFriction();
        MoveWithInput();
    }

    void Update() {
        if (isDashing || isWallJumping) return;
        GetInput();
        HandleJump();
        HandleDash();
        WallSlide();
        WallJump();
    }

    private void MoveWithInput() {
        if (Mathf.Abs(xInput) > 0) {
            float increment = xInput * acceleration;
            float newSpeed = Mathf.Clamp(body.velocity.x + increment, -groundSpeed, groundSpeed);

            body.velocity = new Vector2(xInput * groundSpeed, body.velocity.y);

            float direction = Mathf.Sign(xInput);
            transform.localScale = new Vector3(direction, 1, 1);
        }

    }

    private void WallJump() {
        if (isWallSliding) {
            isWallJumping = false;
            wallJumpingDirection = -transform.localScale.x;
            wallJumpingCounter = wallJumpingTime;

            CancelInvoke(nameof(StopWallJumping));
        } else {
            wallJumpingCounter = 0;
            // wallJumpingCounter = Time.deltaTime; // double wall jump
        }

        if (Input.GetButtonDown("Jump") && wallJumpingCounter > 0f) {
            isWallJumping = true;
            body.velocity = new Vector2(wallJumpingDirection * wallJumpingPower.x, wallJumpingPower.y);
            wallJumpingCounter = 0f;

            if (transform.localScale.x != wallJumpingDirection) {
                transform.localScale = new Vector3(transform.localScale.x*-1, 1, 1);
            }

            Invoke(nameof(StopWallJumping), wallJumpingDuration);
        }
    }

    private void StopWallJumping() {
        isWallJumping = false;
    }


    private void CheckWallTouch() {
        isTouchingWall = Physics2D.OverlapAreaAll(wallCheck.bounds.min, wallCheck.bounds.max, wallMask).Length > 0;
    }

    private void WallSlide() {
        if (isTouchingWall && !grounded && xInput != 0) {
            isWallSliding = true;
            body.velocity = new Vector2(body.velocity.x, Math.Clamp(body.velocity.y, -wallSlidingSpeed, float.MaxValue));
        } else {
            isWallSliding = false;
        }
    }

    private void HandleJump() {
        if (Input.GetButtonDown("Jump") && grounded) {
            body.velocity = new Vector2(body.velocity.x, jumpSpeed);
        }
    }

    private void HandleDash() {
        if (Input.GetKeyDown(KeyCode.C) && canDash) {
            StartCoroutine(nameof(PlayerDash));
        }
    }

    private IEnumerator PlayerDash() {
        canDash = false;
        isDashing = true;
        float originalGravScale = body.gravityScale;
        body.gravityScale = 0;
        body.velocity = new Vector2(transform.localScale.x * dashPower, 0f);
        yield return new WaitForSeconds(dashTime);
        body.gravityScale = originalGravScale;
        isDashing = false;
        yield return new WaitForSeconds(dashCooldown);
        canDash = true;
    }

    void GetInput() {
        xInput = Input.GetAxis("Horizontal");
        yInput = Input.GetAxis("Vertical");
    }

    void CheckGround() {
        grounded = Physics2D.OverlapAreaAll(groundCheck.bounds.min, groundCheck.bounds.max, groundMask).Length > 0;
    }

    void ApplyFriction() {
        if (grounded && xInput == 0 && yInput == 0) {
            body.velocity *= dragDecay;
        }
    }

}
