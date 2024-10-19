using System;
using System.Collections;
using UnityEngine;

public class PlayerMovement : MonoBehaviour {

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
    float xInput;
    float yInput;

    void FixedUpdate() {
        if (isDashing) return;
        CheckGround();
        ApplyFriction();
        MoveWithInput();
    }

    void Update() {
        if (isDashing) return;
        GetInput();
        HandleJump();
        HandleDash();
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

    private void HandleJump() {
        if (Input.GetButtonDown("Jump") && grounded) {
            body.velocity = new Vector2(body.velocity.x, jumpSpeed);
        }
    }

    private void HandleDash() {
        /*
        if (Input.GetKeyDown(KeyCode.C) && !isDashing) {
            isDashing = true;
            float originalGravScale = body.gravityScale;
            body.gravityScale = 0;
            body.velocity = new Vector2(body.velocity.x + dashSpeed * Mathf.Sign(body.velocity.x), 0f);
            isDashing = false;
            body.gravityScale = originalGravScale;
        }*/
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
