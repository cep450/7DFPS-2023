using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickupWeapon : Pickup {

	[SerializeField] Weapon weapon;
	[SerializeField] bool switchToOnPickup = true;

	public override void Give(GameObject recipient) {
		
		if(recipient.tag.Equals("player")) { //TODO check tag proeprly 
			//TODO cast to player 
			//TODO add weapon
			//TODO dispay on screen like picked up [weapon name] or sth 
			//TODO make playe switch to weapon 
			if(switchToOnPickup) {
				
			}
		}
	}

}