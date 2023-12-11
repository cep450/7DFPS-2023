using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{

	//REFERENCE: josh godwalker's devlogs. iirc he had one weapon class and made a bunch of diff ones by changing the settings 
	//but he was also making a roguelike with a large number of small differences between the weapons 
	//vs i feel like ours will be better off with few weapons with large differences 


	//what it fires 
	[SerializeField] float firingSpeed = 1f;	//in seconds, time after this was fired before you can fire again 
	[SerializeField] bool hitscan = true; //hitscan or projectile?
	[SerializeField] Projectile projectile; //if not hitscan, what it fires
	[SerializeField] int count = 1; //how many projectiles/bullets to fire each shot 
	[SerializeField] float spreadX, spreadY; //TODO how do we want to implement, do we want random spread or fixed, what shape, how to determine, ect
	[SerializeField] bool hasAltfire = false;
	[SerializeField] bool automatic = false; //if holding down the button continues firing or if it's one and done. designwise we probably want to avoid automatic weapons but maybe there's some use for an energy beam type thing, idk
	[SerializeField] float recoilAnimation; 
	[SerializeField] float recoilPhysForce = 0;
	[SerializeField] AudioClip sfxFired; //automatically played once if single fire and looped while firing if automatic 

	//TODO wait hold on there's the whole concept of the altfire but how would scoping in be implemented 


	//visual 
	[SerializeField] Color color; //color coding, useful for debug at least
	[SerializeField] Sprite crosshair;
	[SerializeField] GameObject model; //TODO i havent worked in 3d enough to know how to do these.
	//TODO animation played on fire 
	//TODO animation played when weapon switched to 
	//TODO animation played when switched away from 


	//sound  
	[SerializeField] AudioClip sfxSwitchedTo;



	float firingSpeedCountdown = 0; //used to wait til it can be fired again 


	
	//TODO will we use animations, or do them programatically by moving the model around according to settings on this class? 


	//TODO how do we want to implement altfire? maybe some of thse can be grouped in like a serialized fire mode struct, & can have 2 of them, with diff for altfire


	//small note for later- grappling hook could possibly be implemented as a projectile & an altfire for something or as its own 'weapon' 


	//TODO: is customizing these enough, do we even need subclasses, or can we just use prefabs with diff settings? 


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

	public void Fire() {

	}


	public void FireHitscan() {

	}

	public void FireProjectile() {

		//TODO instantiate at the right location 

		//TODO set the projectile's owner to the player 


	}


	//switch to this weapon 
	public void SwitchTo() {
		//TODO play animation sound ect 
	}

	//switch away from this weapon 
	public void SwitchAway() {
		//TODO play animation ect 
	}

	
}
