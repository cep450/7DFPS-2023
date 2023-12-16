using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickupHealth : Pickup {

	[SerializeField] int hpAmount;

	public override void Give(GameObject recipient) {
		
		//cast to player 
		Player player = recipient.GetComponent<Player>();

		//add hp 
		player.ChangeHP(hpAmount);
	}

}