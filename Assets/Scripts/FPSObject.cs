using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
	This object can kill or be killed. 
	Superclass. 
	1. Has health, can be damaged and die, upon which something happens. 
	2. Can damage other things or own projectiles that do, and get credit for killing them. Might only damage certain types of things.
	Player and enemies inherit from this, as might other props. 

	Plenty of things have hp. players, enemies, even level objects that might explode or break. 
	objects that [inherit? implement?] HP can be told they're taking damage and handle what happens as a result.
*/

//TODO: do we want this as an interface or an abstract superclass?
//TODO: where is extra damage modification handled? giver or reciever, thing that hit or thing that was hit? the box recieveing it? does something else multiply dmg, or does it happen here?


//TODO do we need this though? ideally, we have self-damage, enemies can damage each other, ect. though maybe there's a reduction on self-damage, but that could be managed elsewhere?
public enum Type {
	PLAYER, 
	ENEMY, 
	ENVIRONMENT
}

public class FPSObject : MonoBehaviour
{

	public string name = "DEFAULTNAME"; //printed, like, when you kill something, or when something kills you

	public int hp {get; private set;}
	public int hpMax = 1; //hp this thing starts with. this number = the cup is full. hp can potentially go over this value, but might be treated differently as overheal.


    // Start is called before the first frame update
    void Start()
    {
        hp = hpMax;
    }

	//example: something.TakeDmg(10) subtracts 10 from something's hp 
	public void TakeDmg(int dmg) {

		hp -= dmg;
		if(hp <= 0) {
			Die();
		}
	}

	//Called when reaches 0hp. Public so it can also be force-called if we need it.
    public virtual void Die() {

		//Can check how far negative the hp is for diff behavior depending on amount of overkill ex. gib vs not gibbing

		//TODO delete the object, in this default implementation

		//TODO set hp 0 in case force called or smth 
	}
}
