using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;
using UnityEngine.UI;

public class GameUI : MonoBehaviour
{

	//Other classes tell this what to display.

	//Singleton. 
	static GameUI instance;

	[SerializeField] GameObject crosshair;
	Image crosshairImage;

    // Start is called before the first frame update
    void Start()
    {
        instance = this;
		crosshairImage = crosshair.GetComponent<Image>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }


	
	//viewmodel and crosshair
	//pull out a weapon
	public static void SetWeapon(Weapon weapon) {

		//TODO hide the last weapon

		//set crosshair
		instance.crosshairImage.sprite = weapon.crosshair;
		//set crosshair color 
		instance.crosshairImage.color = weapon.color;

		//TODO change viewmodel 

		//TODO maybe- the viewmodels have an animation that plays once on activate to tuneable speed of it being pulled out

	}

	public static void SetHP(int hp) {
		//TODO 
		//if we do stuff like fighting game style hp changes not applying immediately, this is a place to reflect that
	}
}
