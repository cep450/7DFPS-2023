using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : FPSObject
{


	[SerializeField] Weapon[] weapons;

	[Header("Movement")]
	[SerializeField] private float walkSpeed = 5f;
    [SerializeField] private float sprintSpeed = 10f;
    [SerializeField] private float gravity = 2f;
	[SerializeField] private float jumpForce = 5f;
	[SerializeField] private float crouchSpeed = 3f;

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

	private bool isCrouching = false;
	private bool duringCrouchAnimation = false;


    private CharacterController characterController = null;
    private Camera camera = null;
    private Vector3 currentMovement = Vector3.zero;
    private float rotationX = 0;
    private bool isSprinting = false;
    private Vector3 inputDir;
    int currentWeaponIndex = 0; //index in the arraylist

    private void Awake() {
        characterController = GetComponent<CharacterController>();
		camera = GetComponentInChildren<Camera>();
		rotationX = camera.transform.localRotation.y;
		standingCenter = characterController.center;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

	public void OnJump(InputAction.CallbackContext context)
	{
		if (!characterController.isGrounded || duringCrouchAnimation) {
			return;
		}

		if (context.phase == InputActionPhase.Performed) {
			if (isCrouching) {
				ToggleCrouch();
				return;
			}
			currentMovement.y = jumpForce;
			
		}
	}
	public void OnMove(InputAction.CallbackContext context) {
		 inputDir = context.ReadValue<Vector3>();
    }

	public void OnLookRight(InputAction.CallbackContext context) {
		float rotation = context.ReadValue<float>();
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
		if (context.phase == InputActionPhase.Started) {
			Debug.Log("Enabling");
			isSprinting = true;
		}
		else if (context.phase == InputActionPhase.Canceled){
			Debug.Log("Disabling");
			isSprinting = false;
		}
	}

	public void OnCrouch(InputAction.CallbackContext context) {
		if (context.phase == InputActionPhase.Performed) {
			if (duringCrouchAnimation)
				return;
			ToggleCrouch();
		}
	}

    // Update is called once per frame
    void Update()
    {
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

		float speed = isSprinting ? sprintSpeed : walkSpeed;
		speed = isCrouching ? crouchSpeed : speed;
        characterController.Move(currentMovement * speed * Time.deltaTime);
    }

	private void ToggleCrouch() {
        if(isCrouching) {
            // check for ceiling
            if (Physics.Raycast(camera.transform.up, Vector3.up, 1f)) {
				return;
            }

        }

		isCrouching = !isCrouching;
		StartCoroutine(CrouchStand());
    }


	private IEnumerator CrouchStand() {
		

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

	//check: can we actually switch right now?
	private bool CanSwitchWeapon() {
		//TODO do we want there to be limitations to this? like if you're in the middle of firing? in many games you can switch cancel 
		return true;
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

	


	// 


	public bool Airborne {
		get => !characterController.isGrounded;
		//TODO returns true if player is airborne 
		//TODO use a bool instead of a function instead?
		//TODO may want a collider for this, positioned to have coyote time maybe 
		//ex. checks if airborne to check if you can jump, checks if airborne to determine if that rifle can scope 
	}

	

	public override void Die() {
		//TODO retry screen, move camera, ect 
	}
}