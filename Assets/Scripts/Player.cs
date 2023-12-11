using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : FPSObject
{


	[SerializeField] Weapon[] weapons;
	int currentWeaponIndex = 0; //index in the arraylist

	[Header("Movement")]
	[SerializeField] private float movementSpeed = 5f;
	[SerializeField] private float gravity = 2f;
	[SerializeField] private float jumpForce = 5f;

	private CharacterController characterController = null;
	private Camera camera = null;
	private Vector3 currentMovement = Vector3.zero;
	private float rotationX = 0;

	[Header("Look")]
	[SerializeField, Range(0.1f, 10)] private float lookSpeedX = 2;
    [SerializeField, Range(0.1f, 10)] private float lookSpeedY = 2;
	[SerializeField, Range(1, 100)] private float upperLookLimit = 80;
    [SerializeField, Range(1, 100)] private float lowerLookLimit = 80;


    private void Awake() {
        characterController = GetComponent<CharacterController>();
		camera = GetComponentInChildren<Camera>();
		rotationX = camera.transform.localRotation.y;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

	public void OnJump(InputAction.CallbackContext context)
	{
		if (!characterController.isGrounded) {
			Debug.Log("Not grounded!");
			return;
		}

		if (context.phase == InputActionPhase.Performed) {
			currentMovement.y = jumpForce;
			
		}
	}

	public void OnMove(InputAction.CallbackContext context) {
		var input = context.ReadValue<Vector3>();
		currentMovement = (transform.TransformDirection(Vector3.forward) * input.z) + (transform.TransformDirection(Vector3.right) * input.x);


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

    // Update is called once per frame
    void Update()
    {
		if (!characterController.isGrounded)
			currentMovement.y -= gravity * Time.deltaTime;

		characterController.Move(currentMovement * movementSpeed * Time.deltaTime);
    }




    //current weapon player has out 
    public Weapon GetWeapon() {
		return weapons[currentWeaponIndex];
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

	public override void Die() {
		//TODO retry screen, move camera, ect 
	}
}
