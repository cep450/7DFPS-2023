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
