using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickupHealth : Pickup {

	[SerializeField] int hpAmount;

	public override void Give(GameObject recipient) {
		
		if(recipient.tag.Equals(tag)) { //TODO check tag proeprly 
			//TODO cast to player 
			//TODO add hpAmount
		}
	}

}