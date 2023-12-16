using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickupWeapon : Pickup {

	[SerializeField] Weapon weapon;
	[SerializeField] bool switchToOnPickup = true;

	public override void Give(GameObject recipient) {
		
		//cast to player 
		Player player = recipient.GetComponent<Player>();

		//add weapon
		player.AddWeapon(weapon);

		//TODO dispay on screen like picked up [weapon name] or sth 

		//switch to weapon if applicable 
		if(switchToOnPickup) {
			player.SwitchWeapon(weapon.number);
		}
		
	}

}