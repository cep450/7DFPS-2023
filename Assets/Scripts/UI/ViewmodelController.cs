using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ViewmodelController : MonoBehaviour
{

	//singleton 
	static ViewmodelController instance;

	[SerializeField] GameObject viewmodelParent;

	static GameObject[] viewmodels = new GameObject[10];

	static int currentIndex = 1;


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

		if(viewmodels[weapon.number] == null) {
			Debug.Log("No viewmodel at number " + weapon.number + ", creating");
			AddModel(weapon);
		}

		//set the model 
		if(viewmodels[currentIndex] != null) {
			viewmodels[currentIndex].SetActive(false);
		}
		currentIndex = weapon.number;
		viewmodels[currentIndex].SetActive(true);

		//TODO the viewmodels have an animation that plays once on activate to tuneable speed of it being pulled out
		//use weapon.switchedToCooldown as the length of time of the animation. 
		//todo this could go in like set active function in that viewmodel object 

	}

	public static void AddModel(Weapon weapon) {
		GameObject model = Instantiate(weapon.model);
		model.transform.SetParent(instance.viewmodelParent.transform, false);
		viewmodels[weapon.number] = model;
	}


}
