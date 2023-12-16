using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pickup : MonoBehaviour
{

	Color color = Color.red; //for debug 
	[SerializeField] String tagCanPickUp = "Player"; //tag on a game object that can pick this up
	//TODO probably want a reference to a child that's the actual pickup that can be turned on and off 
	[SerializeField] bool disappearsOnPickup = true;
	[SerializeField] float respawnTime = -1; //after this is picked up, how long til there's another one? if negative it's one-time
	[SerializeField] AudioClip sfx;
	[SerializeField] GameObject modelParent;
	BoxCollider trigger;
	
	float respawnCounter = 0;

    // Start is called before the first frame update
    void Start()
    {
        trigger = GetComponent<BoxCollider>();
    }

    // Update is called once per frame
    void Update()
    {
		//respawn the pickup once the timer is done 
        if(respawnCounter > 0 && respawnTime >= 0) {
			respawnCounter -= Time.deltaTime;
			if(respawnCounter <= 0) {
				Show();
			}
		}
    }

	private void OnTriggerEnter(Collider other)
    {
		//TODO filter like what can pick it up 
		//probably by tagCanPickup
        PickUp(other.gameObject);
    }

	//when something picks this up- this should ONLY be on ENTER! unless set interval for like stand in something keeps doing something to you TODO add interval
	public void PickUp(GameObject other) {

		if(disappearsOnPickup) {
			Hide();
			respawnCounter = respawnTime;
		}

		//TODO play pickup noise

		Give(other);
	}

	void Show() {
		modelParent.SetActive(true);
		trigger.enabled = true;
	}

	void Hide() {
		trigger.enabled = false;
		modelParent.SetActive(false);
	}

	//overridden for specific behavior ex. giving health to the player, a weapon to the player...
	public virtual void Give(GameObject recipient) {

	}
}