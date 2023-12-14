using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : FPSObject
{

	//probably one attack per enemy? or do we want to support multiple? 
	//maybe attacks are scripts? not sure 

	Color color = Color.magenta; //debug and potentially more 
	[SerializeField] bool backstabbable = false;
	[SerializeField] bool gardenable = false;

	//TODO probably enemy senses settings, if they try to navigate to the player, ect

	bool aggroed = false; //are they aware of the player + seeking them 

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
		if(aggroed) {
			SeekPlayer();
		} else {
			Idle();
		}
    }

	//these would be overridden by specific enemies' implementations 
	public virtual void Idle() {
		//TODO check for circumstances that would make this enemy aggroed 
	}

	public virtual void SeekPlayer() {
		//TODO turn to face the player if applicable 
		//TODO try to move towards the player if applicable 
		//TODO try to attack the player if applicable 
	}

}
