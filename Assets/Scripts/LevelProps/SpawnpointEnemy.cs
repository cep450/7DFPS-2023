using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnpointEnemy : Spawnpoint
{

	[SerializeField] Enemy enemyToSpawn;
	public string tag = "";

	//TODO look up how to make stuff update in editor 
	//set the color based on the enemy type maybe 

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

	public override void Spawn() {
		//instantiate that enemy here
		//let's say telefragging is possible- if something is in the space, kill it with Die()
	}
}
