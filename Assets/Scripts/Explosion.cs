using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Explosion : MonoBehaviour
{

	// reuseable 
	// manages radius, damage, pushback force 

	//should this be its own thing, or should it be part of Projectile?
	//like maybe a projectile or enemy can spawn this on death 
	//this could be useful to have as a standalone thing


	//TODO animation
	//TODO sound 
	[SerializeField] float radius; //TODO falloff settings ect 
	//TODO damage / physics force (these should be alike/scale to each other)
	public FPSObject owner;
	//TODO do self damage reduction % 
	//TODO leave marks/decals on geometry 

	


}