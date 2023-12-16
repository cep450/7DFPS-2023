using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : FPSObject
{


	[SerializeField] Weapon[] weapons;

	[Header("Functional Options")]
	[SerializeField] private bool canSprint = true;
	[SerializeField] private bool canJump = true;
	[SerializeField] private bool canCrouch = true;
	[SerializeField] private bool canHeadbob = true;
	[SerializeField] private bool willSlideOnSlopes = true;
	[SerializeField] private bool canGrapple = true;

	[Header("Movement")]
	[SerializeField] private float walkSpeed = 5f;
    [SerializeField] private float sprintSpeed = 10f;
    [SerializeField] private float gravity = 2f;
	[SerializeField] private float jumpForce = 5f;
	[SerializeField] private float crouchSpeed = 3f;
	[SerializeField] private float slopeSpeed = 6f;
	[SerializeField] private float slideSpeed = 15f;
	

    [Header("Look")]
	[SerializeField, Range(0.1f, 10)] private float lookSpeedX = 2;
    [SerializeField, Range(0.1f, 10)] private float lookSpeedY = 2;
	[SerializeField, Range(1, 100)] private float upperLookLimit = 80;
    [SerializeField, Range(1, 100)] private float lowerLookLimit = 80;


	[Header("Crouch")]
	[SerializeField] private float crouchHeight = 0.5f;
	[SerializeField] private float standingHeight = 2f;
	[SerializeField] private float timeToCrouch = 0.25f;
	[SerializeField] private Vector3 crouchingCenter = new Vector3(0, 0.5f, 0);
	[SerializeField] private Vector3 standingCenter = new Vector3(0, 0, 0);

	[Header("Headbob")]
	[SerializeField] private float walkBobSpeed = 14f;
	[SerializeField] private float walkBobAmount = 0.05f;
	[SerializeField] private float sprintBobSpeed = 18f;
	[SerializeField] private float sprintBobAmount = 0.1f;
	[SerializeField] private float crouchBobSpeed = 8f;
	[SerializeField] private float crouchBobAmount = 0.025f;

	[Header("Sliding")]
	[SerializeField] private float maxSlideTime = 1;
	[SerializeField] private float slideForce = 1;
	[SerializeField] private float slideHeight = 0.35f;
	[SerializeField] private float timeToSlide = 0.12f;
	[SerializeField] private float slideYScale;
	[SerializeField] private Vector3 slidingCenter = new Vector3(0, 0.35f, 0);

	private float timer = 0;

	private Vector3 hitPointNormal;

	public enum MovementState { Running, Sprinting, Crouching, Sliding, SlopeSliding, Grappling, Freeze }
	private MovementState state = MovementState.Running;
	private bool IsSliding
	{
		get {
			if (characterController.isGrounded && Physics.Raycast(transform.position, Vector3.down, out RaycastHit slopeHit, 2f)) {
				hitPointNormal = slopeHit.normal;
				return Vector3.Angle(hitPointNormal, Vector3.up) > characterController.slopeLimit;
			}
			else {
				return false;
			}
		}
	}


    private float slideTimer = 0;
    private float startYScale = 0;
    private bool isSliding = false;
    private bool isFrozen = false;
    private float defaultYPos = 0;

    private bool isCrouching = false;
	private bool duringCrouchAnimation = false;


    private CharacterController characterController = null;
    private Camera camera = null;
    private Vector3 currentMovement = Vector3.zero;
    private float rotationX = 0;
    private bool isSprinting = false;
    private Vector3 inputDir;
    int currentWeaponIndex = 0; //index in the arraylist
	private WaitForSeconds slideDelay = null;
    private void Start() {
        characterController = GetComponent<CharacterController>();
		camera = GetComponentInChildren<Camera>();
		rotationX = camera.transform.localRotation.y;
		standingCenter = characterController.center;
		Cursor.lockState = CursorLockMode.Locked;
		Cursor.visible= false;

        defaultYPos = camera.transform.localPosition.y;
		startYScale = transform.localScale.y;
		slideDelay = new WaitForSeconds(maxSlideTime);

		SwitchWeapon(0);
    }
	

	public void OnJump(InputAction.CallbackContext context)
	{
		if (!canJump)
			return;

		if (!characterController.isGrounded || duringCrouchAnimation) {
			return;
		}

		if (context.phase == InputActionPhase.Performed) {

            if (state == MovementState.Crouching) {
                ToggleCrouch();
                state = MovementState.Running;
                return;
            }


            currentMovement.y = jumpForce;
			
		}
	}
	public void OnMove(InputAction.CallbackContext context) {
		// Don't change directions while sliding
		if (state == MovementState.Sliding || state == MovementState.Grappling)
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

	public void OnSprint(InputAction.CallbackContext context) {
		if (!canSprint)
			return;
		

		if (context.phase == InputActionPhase.Started) {
			if (state == MovementState.Crouching) {
				ToggleCrouch();
			}

			state = MovementState.Sprinting;
		}
		else if (context.phase == InputActionPhase.Canceled){
			state = MovementState.Running;
		}
	}

	public void OnCrouch(InputAction.CallbackContext context) {
		if (!canCrouch)
			return;

		if (context.phase == InputActionPhase.Performed) {
			if (duringCrouchAnimation)
				return;

			if (state == MovementState.Sprinting || 
				(state == MovementState.Running  && IsMoving)){

				Debug.Log(currentMovement.sqrMagnitude);
				SlideCrouch();
			}
			else {
				ToggleCrouch();
			}


			if (state != MovementState.Crouching) {
				state = MovementState.Crouching;
			}
			else {
				state = MovementState.Running;
			}
		}
	}
	private bool IsMoving
	{
		get => Mathf.Abs(currentMovement.x) + Mathf.Abs(currentMovement.z) > 0;
	}



    // Update is called once per frame
    void Update()
    {
		if (canHeadbob) {
			HandleHeadbob();
		}

		HandleMovement();
    }

	// Take the input direction and apply it to the character controller
	// Subtract from the y value if currently in a jump to apply gravity
	private void HandleMovement() {
        var dir = (transform.TransformDirection(Vector3.forward) * inputDir.z) + (transform.TransformDirection(Vector3.right) * inputDir.x);
		
		currentMovement.x = dir.x;
		currentMovement.z = dir.z;
        if (!characterController.isGrounded)
            currentMovement.y -= gravity * Time.deltaTime;

		float speed = GetSpeedFromState();

        if (willSlideOnSlopes && IsSliding) {
            currentMovement += new Vector3(hitPointNormal.x, -hitPointNormal.y, hitPointNormal.z) * slopeSpeed;
        }

        characterController.Move(currentMovement * speed * Time.deltaTime);
    }

	private float GetSpeedFromState() {
		switch (state) {
			default:
			case MovementState.Running:
				return walkSpeed;
			case MovementState.Sprinting:
				return sprintSpeed;
			case MovementState.Crouching:
				return crouchSpeed;
			case MovementState.Sliding:
				return slideSpeed;
			case MovementState.Freeze:
				return 0;
		}
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
            case MovementState.Freeze:
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

            timer = (timer + (Time.deltaTime * speed)) % float.MaxValue;
			camera.transform.localPosition = new Vector3(
				camera.transform.localPosition.x,
				defaultYPos + Mathf.Sin(timer) * amt,
				camera.transform.localPosition.z);
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
		StartCoroutine(CrouchStand(isCrouching, crouchHeight, standingHeight, timeToCrouch, crouchingCenter, standingCenter));
    }

	// Handle crouch and sliding position transitions
	private IEnumerator CrouchStand(bool isCrouching, float crouchHeight,
		float standingHeight, float timeToCrouch, Vector3 crouchingCenter, Vector3 standingCenter) {
		

		duringCrouchAnimation = true;
		float timeElapsed = 0;
		float targetHeight = isCrouching ? crouchHeight : standingHeight;
		float currentHeight = characterController.height;
		Vector3 targetCenter = isCrouching ? crouchingCenter: standingCenter;
		Vector3 currentCenter = characterController.center; 

		while (timeElapsed < timeToCrouch) {
			characterController.height = Mathf.Lerp(currentHeight, targetHeight, timeElapsed / timeToCrouch);
			characterController.center = Vector3.Lerp(currentCenter, targetCenter, timeElapsed / timeToCrouch);
			timeElapsed += Time.deltaTime;
			yield return null;
		}
		characterController.height = targetHeight;
		characterController.center = targetCenter;
		duringCrouchAnimation = false;
	}

	private void SlideCrouch() {
		StartCoroutine(SlideThenCrouch());
	}
	private IEnumerator SlideThenCrouch() {
		yield return CrouchStand(true, slideHeight, standingHeight, timeToSlide, slidingCenter, standingCenter);
		state = MovementState.Sliding;
		yield return slideDelay;
		state = MovementState.Crouching;
		// Transition to crouch
		yield return CrouchStand(false, slideHeight, crouchHeight, timeToSlide * 1.2f, slidingCenter, crouchingCenter);

	}


	// weapon stuff

	//current weapon player has out 
	public Weapon GetWeapon() {
		return weapons[currentWeaponIndex];
	}

	//try firing the current weapon- but may fail due to fire speed, switching, ect 
	//the weapon knows if it can fire itself right now, it will handle that check 
	public void TryFire() {
		GetWeapon().TryFire(0);
	}
	public void TryFireAlt() {
		GetWeapon().TryFire(1);
	}

	public void SwitchWeaponForward() {
		int newIndex = currentWeaponIndex + 1;
		if(newIndex >= weapons.Length){
			newIndex = 0;
		}
		SwitchWeapon(newIndex);
	}
	public void SwitchWeaponBack() {
		int newIndex = currentWeaponIndex - 1;
		if(newIndex < 0){
			newIndex = weapons.Length - 1;
		}
		SwitchWeapon(newIndex);
	}
	//switch to weapon at this index
	public void SwitchWeapon(int index) {
		weapons[currentWeaponIndex].SwitchAway();
		weapons[index].SwitchTo();
		currentWeaponIndex = index;
	}

	public void ToggleFreeze() {
		if (state != MovementState.Freeze) {
			state = MovementState.Freeze;
		}
		else {
			state = MovementState.Running;
		}
	}

	public void ToggleFreeze(bool isOn) {
        if (isOn) {
            state = MovementState.Freeze;
        }
        else {
            state = MovementState.Running;
        }
    }



	public void GrappleJump(Vector3 targetPosition, float trajectoryHeight) {
		currentMovement = CalculateJumpVelocity(transform.position, targetPosition, trajectoryHeight);
	}

	// 
	public Vector3 CalculateJumpVelocity(Vector3 startPoint, Vector3 endPoint, float trajectoryHeight) {
		float displacementY = endPoint.y - startPoint.y;
		Vector3 displacementXZ = new Vector3(endPoint.x - startPoint.x, 0, endPoint.z - startPoint.z);

		Vector3 velocityY = Vector3.up * Mathf.Sqrt(-2 * gravity * trajectoryHeight);
		Vector3 velocityXZ = displacementXZ / (Mathf.Sqrt(-2 * gravity * trajectoryHeight / gravity) + Mathf.Sqrt(2 * (displacementY - trajectoryHeight) / gravity));
		return velocityXZ + velocityY;
	}


	public bool Airborne {
		get => !characterController.isGrounded;
		//TODO returns true if player is airborne 
		//TODO use a bool instead of a function instead?
		//TODO may want a collider for this, positioned to have coyote time maybe 
		//ex. checks if airborne to check if you can jump, checks if airborne to determine if that rifle can scope 
	}

	public bool CanGrapple { get => canGrapple; }

    private const float MOVETOLERANCE = 0.0001f;

    public override void Die() {
		//TODO retry screen, move camera, ect 
	}
}