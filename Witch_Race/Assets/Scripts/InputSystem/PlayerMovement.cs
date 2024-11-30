using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;


public class PlayerMovement : MonoBehaviour
{
    public float topSpeed;
    public float acceleration;
    public float friction; // factor in deceleration
    public float diveBoostIncrease;
    public float diveSpeedIncrease;
    public float diveCoolDown;
    public float spinCoolDown;
    public Color testColor; // TODO: placeholder

    private Rigidbody2D myRigidbody;
    private SpriteRenderer mySprite;
    private Color ogColor;
    public Vector3 movementDirection;
    public float currentVelocity;
    public bool movingUp;
    public bool movingDown; 
    /*
    private bool movingLeft;
    private bool movingRight;
    */
    private float _topSpeed;
    private bool isDiving;
    private float diveStart;

    private void Awake() {
        myRigidbody = GetComponent<Rigidbody2D>();
        mySprite = GetComponent<SpriteRenderer>();
        ogColor = mySprite.color;
        movementDirection = Vector3.zero;
        movingUp = false;
        movingDown = false;
        /*
        movingLeft = false;
        movingRight = false;
        */
        currentVelocity = 0.0f;

        _topSpeed = topSpeed;
        isDiving = false;
    }

    public void Up(InputAction.CallbackContext context) {
        if (context.performed)
            movingUp = true;
        if (context.canceled)
            movingUp = false;
    }

    public void Down(InputAction.CallbackContext context) {
        if (context.performed)
            movingDown = true;
        if (context.canceled)
            movingDown = false;
    }
/*
    public void Left(InputAction.CallbackContext context) {
        if (context.performed)
            movingLeft = true;
        if (context.canceled)
            movingLeft = false;
    }

    public void Right(InputAction.CallbackContext context) {
        if (context.performed)
            movingRight = true;
        if (context.canceled)
            movingRight = false;
    }
*/

    IEnumerator SmallDelay() {
        mySprite.color = testColor;
        Debug.Log("Spinning...");
        yield return new WaitForSeconds(spinCoolDown);
        mySprite.color = ogColor;
        Debug.Log("Done!");
    }

    public void Spin(InputAction.CallbackContext context) {
        if (context.performed)
            StartCoroutine(SmallDelay());
    }

    public bool OkayToDive() {
        float timeDiff = Time.time - diveStart;
        if (Mathf.Approximately(timeDiff, diveCoolDown) || (timeDiff < diveCoolDown))
            return false;
        return true;
    }

    public void Dive(InputAction.CallbackContext context) {
        if (context.performed) {
            if (isDiving)
                return;
            diveStart = Time.time;
            isDiving = true;
            Debug.Log("DIVE!!!!!");
            topSpeed += diveSpeedIncrease;
            currentVelocity += (diveBoostIncrease * Time.deltaTime * movementDirection.y);
        }
    }

    public void Accelerate() {
        float tempVelocity;
        tempVelocity = currentVelocity + (acceleration * Time.deltaTime * movementDirection.y);
        if (Mathf.Approximately(Mathf.Abs(tempVelocity), topSpeed) ||
                (Mathf.Abs(tempVelocity) > topSpeed)) {
            currentVelocity = topSpeed * Mathf.Sign(tempVelocity);
            return;
        }
        currentVelocity = tempVelocity;
    }

    public void ApplyFriction() {
        float tempVelocity;

        if (Mathf.Approximately(currentVelocity, 0.0f))
            return;
        
        tempVelocity = currentVelocity - (friction * Time.deltaTime * Mathf.Sign(currentVelocity));
        if (Mathf.Approximately(tempVelocity, 0.0f)) {
            currentVelocity = 0.0f;
            return;
        }
        currentVelocity = tempVelocity;
        return;
    }

    public void UpdatePosition() {
        if (Mathf.Approximately(movementDirection.y, 0.0f))
            ApplyFriction();
        else
            Accelerate();
        transform.position += new Vector3(0, currentVelocity, 0) * Time.deltaTime;
    }

    private void FixedUpdate() {
        if (isDiving && OkayToDive()) {
            topSpeed = _topSpeed;
            isDiving = false;
        }
        movementDirection = Vector3.zero;

        if (movingUp)
            movementDirection += Vector3.up;
        if (movingDown)
            movementDirection += Vector3.down;
        /*
        if (movingLeft)
            movementDirection += new Vector3(-1, 0);
        if (movingRight)
            movementDirection += new Vector3(1, 0);
        */
        
        // FIXME: check for collision first
        UpdatePosition();
    }
}
