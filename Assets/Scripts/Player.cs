using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : FPSObject
{


	[SerializeField] Weapon[] weapons; 
	int currentWeaponIndex = 0; //index in the arraylist

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
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

	public bool Airborne() {
		//TODO returns true if player is airborne 
		//TODO use a bool instead of a function instead?
		//TODO may want a collider for this, positioned to have coyote time maybe 
		//ex. checks if airborne to check if you can jump, checks if airborne to determine if that rifle can scope 
		return false;
	}

	public override void Die() {
		//TODO retry screen, move camera, ect 
	}
}
