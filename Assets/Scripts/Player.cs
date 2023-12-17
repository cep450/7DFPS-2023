using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : FPSObject
{

	/*
		everything about the player BESIDES movement (weapons, hp, ect)
	*/

	[SerializeField] Weapon[] startingWeapons;
	[SerializeField] int startingWeaponId = 1;
	Weapon[] weapons = new Weapon[10];
    int currentWeaponIndex = 0; //index in the arraylist

    private void Start() {

		foreach(Weapon w in startingWeapons) {
			AddWeapon(w);
		}
		SwitchWeapon(startingWeaponId);
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

	//pick up a weapon 
	public void AddWeapon(Weapon newWeapon) {
		weapons[newWeapon.number] = newWeapon;
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
		if(!SwitchWeapon(newIndex)) { //if nothing at this index, keep going
			SwitchWeaponForward();
		}
	}
	public void SwitchWeaponBack() {
		int newIndex = currentWeaponIndex - 1;
		if(newIndex < 0){
			newIndex = weapons.Length - 1;
		}
		if(!SwitchWeapon(newIndex)) { //ditto
			SwitchWeaponBack();
		}
	}
	//switch to weapon at this index
	//returns true if successful, false if nothing there
	public bool SwitchWeapon(int index) {
		if(weapons[index] == null || !weapons[index].useable) {
			return false;
		}
		weapons[currentWeaponIndex].SwitchAway();
		weapons[index].SwitchTo();
		currentWeaponIndex = index;
		return true;
	}

	protected override void Damaged(int amount) {
		
		//TODO update health ui 

		//TODO sfx 

		//TODO actually should sfx go on the FPSObject base class? cause enemies might have the same. or do we leave it open to work differntly player/enemy?

	}

	protected override void Healed(int amount) {
		 
		 //TODO update health ui 

		 //TODO sfx 
	}

	public override void Die() {
		//TODO retry screen, move camera, ect 
	}
}