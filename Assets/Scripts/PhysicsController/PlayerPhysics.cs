using Microsoft.Unity.VisualStudio.Editor;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Jobs;

public class PlayerPhysics : MonoBehaviour
{

    [Header("Functional Controls")]
    [SerializeField] private bool canGrapple = true;
    public bool CanGrapple { get => canGrapple; }

    [Header("Look")]
    [SerializeField, Range(0.1f, 10)] private float lookSpeedX = 2;
    [SerializeField, Range(0.1f, 10)] private float lookSpeedY = 2;
    [SerializeField, Range(1, 100)] private float upperLookLimit = 80;
    [SerializeField, Range(1, 100)] private float lowerLookLimit = 80;
    [SerializeField] Transform cameraHolder = null;

    [Header("Movement")]
    [SerializeField] private float runSpeed = 10f;
    [SerializeField] private float groundDrag = 1f;
    [SerializeField] private float sprintSpeed = 10f;
    [SerializeField] private float gravity = 2f;
    [SerializeField] private float jumpForce = 5f;
    [SerializeField] private float jumpCooldown = 2f;
    [SerializeField] private float airMultiplier = 2f;
    [SerializeField] private float crouchSpeed = 3f;
    [SerializeField] private float slopeSpeed = 6f;
    [SerializeField] private float slideForce = 5f;


    [Header("Ground Check")]
    [SerializeField] private float playerHeight = 1;
    [SerializeField] private LayerMask whatIsGround;

    [Header("Headbob")]
    [SerializeField] private float walkBobSpeed = 14f;
    [SerializeField] private float walkBobAmount = 0.05f;
    [SerializeField] private float sprintBobSpeed = 18f;
    [SerializeField] private float sprintBobAmount = 0.1f;
    [SerializeField] private float crouchBobSpeed = 8f;
    [SerializeField] private float crouchBobAmount = 0.025f;

    [Header("Crouch")]
    [SerializeField] private float crouchHeight = 0.5f;
    [SerializeField] private float standingHeight = 2f;
    [SerializeField] private float crouchSeed = 0.25f;
    [SerializeField] private float timeToCrouch = 0.25f;
    [SerializeField] private Vector3 crouchingCenter = new Vector3(0, 0.5f, 0);
    [SerializeField] private Vector3 standingCenter = new Vector3(0, 0, 0);



    [Header("Slope Handling")]
    [SerializeField] private float maxSlopeAngle = 25f;
    private RaycastHit slopeHit;


    bool isGrounded = true;
    bool duringCrouchAnimation = false;

    private Rigidbody rb = null; 
    private CapsuleCollider collder = null;
    private Camera camera = null;
    float rotationX = 0;
    Vector3 inputDir = Vector3.zero;
    Vector3 currentMovement = Vector3.zero;
    public enum MovementState { Running, Sprinting, Crouching, Sliding, SlopeSliding, Grappling, Frozen }
    private MovementState state = MovementState.Running;

    private float jumpTimer = 0;
    private float headbobTimer = 0;
    private float defaultCamYPos = 0;
    private float crouchAmt = 0;
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponentInChildren<Rigidbody>();
        camera = GetComponentInChildren<Camera>();
        rotationX = camera.transform.localPosition.y;
        defaultCamYPos = camera.transform.localPosition.y;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        rb.drag = groundDrag;
        collder = GetComponentInChildren<CapsuleCollider>();
        crouchAmt = transform.position.y - crouchHeight;
        playerHeight = collder.height;
    }


    public void OnMove(InputAction.CallbackContext context) {
        // Don't change directions while sliding
        if (state == MovementState.Sliding)
            return;


        inputDir = context.ReadValue<Vector3>();
    }

    public void OnLookRight(InputAction.CallbackContext context) {
        float rotation = context.ReadValue<float>() * lookSpeedX;
        transform.rotation *= Quaternion.Euler(0, rotation, 0);

    }

    public void OnLookUp(InputAction.CallbackContext context) {
        if (context.phase == InputActionPhase.Performed) {
            rotationX -= context.ReadValue<float>() * lookSpeedY;
            rotationX = Mathf.Clamp(rotationX, -upperLookLimit, lowerLookLimit);
            camera.transform.localRotation = Quaternion.Euler(rotationX, 0, 0);
        }
    }

    public void OnJump(InputAction.CallbackContext context) {
        if (jumpTimer > 0)
            return;

        if (context.phase == InputActionPhase.Performed) {
            Jump();
            jumpTimer = jumpCooldown;
        }
    }

    public void OnCrouch(InputAction.CallbackContext context) {

        if (context.phase == InputActionPhase.Performed) {
            if (duringCrouchAnimation)
                return;

            //if (state == MovementState.Sprinting ||
            //    (state == MovementState.Running && IsMoving)) {

            //    Debug.Log(currentMovement.sqrMagnitude);
            //    SlideCrouch();
            //}
            //else {
            //    ToggleCrouch();
            //}

            ToggleCrouch();
            if (state != MovementState.Crouching) {
                state = MovementState.Crouching;

            }
            else {
                state = MovementState.Running;
            }
        }
    }

    private void ToggleCrouch() {
        if (state == MovementState.Crouching) {
            // check for ceiling
            if (Physics.Raycast(camera.transform.up, Vector3.up, 1f)) {
                return;
            }

        }

        bool isCrouching = state != MovementState.Crouching ? true : false;
        StartCoroutine(CrouchCollider(isCrouching));
        //StartCoroutine(CrouchStand(isCrouching, crouchHeight, standingHeight, timeToCrouch, crouchingCenter, standingCenter));
    }

    //// Handle crouch and sliding position transitions
    //private IEnumerator CrouchStand(bool isCrouching, float crouchHeight,
    //    float standingHeight, float timeToCrouch, Vector3 crouchingCenter, Vector3 standingCenter) {


    //    duringCrouchAnimation = true;
    //    float timeElapsed = 0;
    //    float targetHeight = isCrouching ? crouchHeight : standingHeight;
    //    float currentHeight = characterController.height;
    //    Vector3 targetCenter = isCrouching ? crouchingCenter : standingCenter;
    //    Vector3 currentCenter = characterController.center;

    //    while (timeElapsed < timeToCrouch) {
    //        characterController.height = Mathf.Lerp(currentHeight, targetHeight, timeElapsed / timeToCrouch);
    //        characterController.center = Vector3.Lerp(currentCenter, targetCenter, timeElapsed / timeToCrouch);
    //        timeElapsed += Time.deltaTime;
    //        yield return null;
    //    }
    //    characterController.height = targetHeight;
    //    characterController.center = targetCenter;
    //    duringCrouchAnimation = false;
    //}

    private IEnumerator CrouchCollider(bool isCrouching) {
        duringCrouchAnimation = true;
        float elapsed = 0;
        float scaleAmt = isCrouching ? -crouchAmt : crouchAmt;


        Vector3 startHeight = transform.position;
        Vector3 targetHeight = transform.position;
        targetHeight.y += scaleAmt;
        Vector3 startScale = collder.transform.localScale;
        Vector3 targetScale = collder.transform.localScale;
        targetScale.y += scaleAmt;

        Vector3 startCam = cameraHolder.localPosition;
        Vector3 endCam = startCam;
        endCam.y += scaleAmt;

        if (!isCrouching) {
            rb.AddForce(currentMovement * slideForce, ForceMode.Impulse);
        }

        while (elapsed < timeToCrouch) {
            collder.transform.localScale = Vector3.Lerp(startScale, targetScale, elapsed / timeToCrouch);
            //transform.position = Vector3.Lerp(startHeight, targetHeight, elapsed / timeToCrouch);
            cameraHolder.localPosition = Vector3.Lerp(startCam, endCam, elapsed / timeToCrouch);
            elapsed += Time.deltaTime;
            yield return null;
        }

        collder.transform.localScale = targetScale;
       // transform.position = targetHeight;
        cameraHolder.transform.localPosition = endCam;
        duringCrouchAnimation = false;

    }


    // Update is called once per frame
    void Update() {
        //SpeedControl();
        CheckGrouded(out bool needsUpdate);

        rb.drag = isGrounded ? groundDrag : 0;

        if (jumpTimer > 0) {
            jumpTimer -= Time.deltaTime;
        }
    }

    private void FixedUpdate() {
        HandleHeadbob();
        HandleMovement();
    }

    private void CheckGrouded(out bool changed) {
        bool grounded = isGrounded;
        isGrounded = Physics.Raycast(transform.position, Vector3.down, playerHeight *1 + 0.2f, whatIsGround);
        changed = grounded != isGrounded;
    }


    private void HandleMovement() {
        var dir = (transform.forward* inputDir.z) + (transform.right * inputDir.x);

        currentMovement.x = dir.x;
        currentMovement.z = dir.z;


        float speed = GetSpeedFromState();

        if (OnSlope) {
            rb.AddForce(GetSlopeMoveDirection() * speed *5, ForceMode.Force);
        }

        if (isGrounded) {
            rb.AddForce(currentMovement.normalized * speed * 10, ForceMode.Force);
        }
        else {
            rb.AddForce(currentMovement.normalized * speed * 10 * airMultiplier, ForceMode.Force);
        }

        //if (willSlideOnSlopes && IsSliding) {
        //    currentMovement += new Vector3(hitPointNormal.x, -hitPointNormal.y, hitPointNormal.z) * slopeSpeed;
        //}

    }

    private float BobSpeedFromState(out float amount) {
        switch (state) {
            default:
            case MovementState.Running:
                amount = walkBobAmount;
                return walkBobSpeed;
            case MovementState.Sprinting:
                amount = sprintBobAmount;
                return sprintBobSpeed;
            case MovementState.Crouching:
                amount = crouchBobAmount;
                return crouchBobSpeed;
            case MovementState.Sliding:
                amount = 1;
                return 1;
            case MovementState.Frozen:
                amount = 0;
                return 0;
        }
    }

    private void HandleHeadbob() {

        if (state == MovementState.Sliding || state == MovementState.SlopeSliding) {
            return;
        }

        if (Mathf.Abs(currentMovement.x) > 0.1f || Mathf.Abs(currentMovement.z) > 0.1f) {
            float speed = BobSpeedFromState(out float amt);

            headbobTimer = (headbobTimer + (Time.deltaTime * speed)) % float.MaxValue;
            camera.transform.localPosition = new Vector3(
                camera.transform.localPosition.x,
                defaultCamYPos + Mathf.Sin(headbobTimer) * amt,
                camera.transform.localPosition.z);
        }
    }


    private void SpeedControl() {
        Vector3 flatVel = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

        //limit velocity if needed
        float speed = GetSpeedFromState();
        if (flatVel.sqrMagnitude > speed*speed) {
            Vector3 limitedVel = flatVel.normalized * speed;
            rb.velocity = new Vector3(limitedVel.x, rb.velocity.y, limitedVel.z);
        }
    }

    private void Jump() {
        rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
        rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);
    }

    private bool OnSlope { get {

            //Debug.DrawRay(transform.position, Vector3.down * (playerHeight * 0.5f + 0.3f), Color.green, 0.1f);
            if (Physics.Raycast(transform.position, Vector3.down, out slopeHit, playerHeight * 0.5f + 0.3f)) {
                float angle = Vector3.Angle(Vector3.up, slopeHit.normal);
                return angle < maxSlopeAngle && angle != 0;
            }
            return false;
        } 
    }

    private Vector3 GetSlopeMoveDirection() {
        return Vector3.ProjectOnPlane(currentMovement, slopeHit.normal).normalized;
    }


    private float GetSpeedFromState() {
        switch (state) {
            default:
            case MovementState.Running:
                return runSpeed;
            case MovementState.Sprinting:
                return sprintSpeed;
            case MovementState.Crouching:
                return crouchSpeed;
            case MovementState.Sliding:
                return slideForce;
            case MovementState.Frozen:
                return 0;
        }
    }

    public void ToggleFreeze(bool isOn) {
        if (isOn) {
            state = MovementState.Frozen;
        }
        else {
            state = MovementState.Running;
        }
    }


    private bool IsMoving { get => Mathf.Abs(rb.velocity.x) + Mathf.Abs(rb.velocity.z) > 0; }
}
