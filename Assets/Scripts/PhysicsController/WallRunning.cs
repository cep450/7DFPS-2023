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
    [SerializeField] float wallJumpSideForce = 1;
    [SerializeField] float wallJumpUpForce = 1;


    private float wallRunTimer = 0;

    [Header("Wall Run Detection")]
    [SerializeField] float wallCheckMaxDistance = 1;
    [SerializeField] float minJumpHeight = 1;

    [Header("Gravity")]
    [SerializeField] bool useGravity = false;
    [SerializeField] float gravityCounterForce = 0;

    [Header("Exiting")]
    [SerializeField] float exitWallTime = 0.25f;
    private bool exitingWall = false;
    private float exitTimer = 0;

    [Header("Camera Parameters")]
    [SerializeField] float fovChange = 6f;
    [SerializeField] float tiltChange = 5f;

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

    public void WallJump(InputAction.CallbackContext context) {
        if (!player.IsWallRunning )
            return;

        if (context.phase == InputActionPhase.Performed) {
            Vector3 wallNormal = wallRight ? rightWallHit.normal : leftWallHit.normal;
            Vector3 forceToApply = transform.up * wallJumpUpForce + wallNormal * wallJumpSideForce;

            // reset y velocity and add force
            rb.velocity = new Vector3(rb.velocity.x, 0, rb.velocity.z);
            rb.AddForce(forceToApply, ForceMode.Impulse);
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
        if ((wallLeft || wallRight) && AboveGround && verticalInput > 0 && !exitingWall) {
            if (!player.IsWallRunning)
                StartWallRun();

            if (wallRunTimer > 0) {
                wallRunTimer -= Time.deltaTime;
            }
            else {
                exitingWall = true;
                exitTimer = exitWallTime;
            }
        }
        else if (exitingWall) {
            if (player.IsWallRunning) {
                StopWallRun();
            }
            if (exitTimer > 0) {
                exitTimer -= Time.deltaTime;
            }
            if (exitTimer < 0) {
                exitingWall = false;
            }
        }
        else if (player.IsWallRunning) {
            StopWallRun();
        }
    }


    private void StartWallRun() {
        player.IsWallRunning = true;
        wallRunTimer = maxWallRunTime;

        rb.useGravity = useGravity;
        // reset y velocity and add force
        rb.velocity = new Vector3(rb.velocity.x, 0, rb.velocity.z);

        player.AdjustFOV(-fovChange);
        if (wallLeft) {
            player.DoTilt(-tiltChange);
        }
        else if (wallRight) {
            player.DoTilt(tiltChange);
        } 
    }

    private void WallRunningMovement() {
        rb.velocity = new Vector3(rb.velocity.x, 0, rb.velocity.z);

        Vector3 wallNormal = wallRight ? rightWallHit.normal : leftWallHit.normal;

        Vector3 wallForward = Vector3.Cross(wallNormal, transform.up);

        if ((transform.forward - wallForward).magnitude > (transform.forward - -wallForward).magnitude) {
            wallForward = -wallForward;
        }

        // push forward
        rb.AddForce(wallForward * wallRunForce, ForceMode.Force);


        //// push to wall
        if (!(wallLeft && horizontalInput <= 0) && !(wallRight && horizontalInput >= 0)) {
            rb.AddForce(-wallNormal * 50, ForceMode.Force);
        }

        // weaken gravity
        if (useGravity)
            rb.AddForce(transform.up * gravityCounterForce, ForceMode.Force);
    }


    private void StopWallRun() {
        player.IsWallRunning = false;
        rb.useGravity = true;
        exitingWall = true;
        exitTimer = exitWallTime;
        player.AdjustFOV(0);
        player.DoTilt(0f);

    }


    private bool AboveGround { get => !player.IsGrounded; }
}
