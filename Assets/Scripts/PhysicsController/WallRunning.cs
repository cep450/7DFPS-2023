using System.Collections;
using System.Collections.Generic;
using Unity.Profiling;
using UnityEditor.Experimental.GraphView;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.InputSystem;

public class WallRunning : MonoBehaviour
{
    [Header("Wall Running")]
    [SerializeField] LayerMask whatIsWall;
    [SerializeField] LayerMask whatIsGround;
    [SerializeField] float wallRunForce = 1;
    [SerializeField] float maxWallRunTime = 2;


    private float wallRunTimer = 0;

    [Header("Wall Run Detection")]
    [SerializeField] float wallCheckMaxDistance = 1;
    [SerializeField] float minJumpHeight = 1;

    private RaycastHit leftWallHit;
    private RaycastHit rightWallHit;
    private bool wallLeft = false;
    private bool wallRight = false;

    private Rigidbody rb = null;
    private PlayerPhysics player = null;
    private float horizontalInput = 0;
    private float verticalInput = 0;

    private void Start() {
        rb = GetComponent<Rigidbody>();
        player = GetComponent<PlayerPhysics>();

    }

    public void HorizontalInput(InputAction.CallbackContext context) {
        if (context.phase == InputActionPhase.Started) {
            horizontalInput = context.ReadValue<float>();
        }
        else if (context.phase == InputActionPhase.Canceled){
            horizontalInput = 0;
        }
    }
    public void VerticalInput(InputAction.CallbackContext context) {
        if (context.phase == InputActionPhase.Started) {
            verticalInput = context.ReadValue<float>();
        }
        else if (context.phase == InputActionPhase.Canceled) {
            verticalInput = 0;
        }
    }


    private void Update() {
        CheckForWall();
        StateMachine();
    }

    private void FixedUpdate() {
        if (player.IsWallRunning) {
            WallRunningMovement();
        }
    }

    private void CheckForWall() {
        wallRight = Physics.Raycast(transform.position, transform.right, out rightWallHit, wallCheckMaxDistance, whatIsWall);
        wallLeft = Physics.Raycast(transform.position, -transform.right, out leftWallHit, wallCheckMaxDistance, whatIsWall);
    }

    private void StateMachine() {
        if ((wallLeft || wallRight) && AboveGround && verticalInput > 0) {
            if (!player.IsWallRunning)
                StartWallRun();
        }
        else if (player.IsWallRunning) {
            StopWallRun();
        }
    }


    private void StartWallRun() {
        player.IsWallRunning = true;
    }

    private void WallRunningMovement() {
        rb.useGravity = false;
        rb.velocity = new Vector3(rb.velocity.x, 0, rb.velocity.z);

        Vector3 wallNormal = wallRight ? rightWallHit.normal : leftWallHit.normal;

        Vector3 wallForward = Vector3.Cross(wallNormal, transform.up);

        if ((transform.forward - wallForward).magnitude > (transform.forward - -wallForward).magnitude) {
            wallForward = -wallForward;
        }


        rb.AddForce(wallForward * wallRunForce, ForceMode.Force);
    }


    private void StopWallRun() {
        player.IsWallRunning = false;
        rb.useGravity = true;
    }


    private bool AboveGround { get => !player.IsGrounded; }
}
