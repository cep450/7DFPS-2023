using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{

	//this script goes on a prefab 

	//mechanical 
	[SerializeField] int dmg = 1; //default dmg- can give this other properties like ramp up or w/e by overriding CalculateDmg
	[SerializeField] Explosion explosion = null; //if null, doesn't explode
	
	//visual 
	//TODO particle effect played while moving 
	//TODO particle effect played when hit something 

	//sound 
	//TODO sound played when hit something


	public FPSObject owner; //who fired this?


	//TODO do we want to just create/destroy these or reuse them? 


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

	void HitSomething() {

	}

	//override for diff behavior 
	int CalculateDmg() {
		return dmg;
	}

	//Remove the projectile without its death happening or anything. 
	//Do this when it reaches the skybox, or we need to clean up projectiles for whatever reason 
	public void Remove() {
		//TODO probably just destroy itself 
	}

	
}
