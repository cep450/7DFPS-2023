using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//what it fires. settings for both main fire and potential alt fire.
[System.Serializable]
public struct FireMode {
	public float firingSpeed;		//in seconds, time after this was fired before you can fire again 
	public bool hitscan; 			//hitscan or projectile?
	public float hitscanRange; 		//default (0) is infinte, otherwise determines length of raycast 
	public Projectile projectile; 	//if not hitscan, what it fires
	//TODO how to model falloff? could apply to both projectile and hitscan distance.
	public int count; 				//how many projectiles/bullets to fire each shot 
	public float spreadX, spreadY; 	//TODO how do we want to implement, do we want random spread or fixed, what shape, how to determine, ect
	public bool automatic; 			//if holding down the button continues firing or if it's one and done. designwise we probably want to avoid automatic weapons but maybe there's some use for an energy beam type thing, idk
	public float recoilPhysForce;
	public float recoilAnimation; 
	public AudioClip sfxFired; 		//automatically played once if single fire and looped while firing if automatic 
}

public class Weapon : MonoBehaviour
{

	//REFERENCE: josh godwalker's devlogs. iirc he had one weapon class and made a bunch of diff ones by changing the settings 
	//but he was also making a roguelike with a large number of small differences between the weapons 
	//vs i feel like ours will be better off with few weapons with large differences 

	//# id for the weapon 
	//unique 
	//also the number key that this weapon is 
	//also the order it is when you scroll thru it 
	public int number;
	public bool useable = true;

	public string name = "WEAPON"; 

	[SerializeField] FireMode[] fireModes;

	[SerializeField] float switchedToCooldown = 1f; //when switched to, time before it can be fired- this should also be the length of the animation (& can be enforced programmatically)


	//TODO wait hold on there's the whole concept of the altfire but how would scoping in be implemented 


	//TODO: 
	//2 options: 
	// 1. keep weapons as an array. authoratative lsit of all possible weapons that can be picked up. give each a bool of if the player has it. disallow switching to if not
	// 2. make weapons an arraylist. this way, weapons can be added and removed. this may fit better?


	//visual 
	public Color color; //color coding, useful for debug at least
	public Sprite crosshair;
	public GameObject model; //TODO i havent worked in 3d enough to know how to do these.
	//TODO animation played on fire 
	//TODO animation played when weapon switched to 
	//TODO animation played when switched away from 


	//sound  
	[SerializeField] AudioClip sfxSwitchedTo;


	float firingCooldown = 0; //used to wait til it can be fired again, by both firing speed, and switching to this weapon 

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
        if(firingCooldown > 0) {
			firingCooldown -= Time.deltaTime;
		} 
    }

	//try firing this weapon. actually fire it if we can do that. 
	//0 for regular, 1 for altfire-- seems clearer than true/false 
	public void TryFire(int fireMode) {

		if(firingCooldown > 0) {
			return;
		}

		if(fireMode >= fireModes.Length) {
			Debug.Log("Tried to fire mode " + fireMode + ", which weapon " + name + " doesn't have.");
			//TODO maybe a sound effect for trying to altfire on a gun that doesn't have one? or doesn't need it 
			return;
		}

		Fire(fireModes[fireMode]);
	}

	private void Fire(FireMode mode) {

		firingCooldown = mode.firingSpeed;

		//TODO play animation 

		//TODO play sound mode.sfxFired

		if(mode.hitscan) {
			FireHitscan(mode);
		} else {
			FireProjectile(mode);
		}
	}


	private void FireHitscan(FireMode mode) {

		//TODO raycast 

	}

	private void FireProjectile(FireMode mode) {

		//TODO instantiate at the right location 

		//TODO set the projectile's owner to the player 


	}


	//switch to this weapon 
	public void SwitchTo() {

		//bring up viewmodel and crosshair in UI
		GameUI.SetWeapon(this);

		//TODO play sound 

		firingCooldown = switchedToCooldown;
	}

	//switch away from this weapon 
	public void SwitchAway() {
		//TODO do we need to do anything here?
	}

	
}
