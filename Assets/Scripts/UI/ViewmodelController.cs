using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ViewmodelController : MonoBehaviour
{

	//singleton 
	static ViewmodelController instance;

	[SerializeField] GameObject viewmodel;

	List<GameObject> viewmodels = new List<GameObject>();


    // Start is called before the first frame update
    void Start()
    {
        instance = this;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

	public static void ChangeModel(Weapon weapon) {

		//TODO set the model 

		//TODO the viewmodels have an animation that plays once on activate to tuneable speed of it being pulled out
		//use weapon.switchedToCooldown as the length of time of the animation. 

	}


}
