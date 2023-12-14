using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;
using UnityEngine.UIElements;

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




	public static void SetCrosshair(Sprite img) {
		//weapon will use this 
		instance.crosshairImage.sprite = img;
	}

	public static void SetHP(int hp) {
		//TODO 
		//if we do stuff like fighting game style hp changes not applying immediately, this is a place to reflect that
	}
}
