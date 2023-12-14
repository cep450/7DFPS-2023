using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*

	Controls high level game state: routing between scenes, pausing, unpausing, ect.
	Singleton.

*/

public class GameManager
{

	public static bool paused { get; private set; }
    
	public static void LoadSceneGame() {
		//TODO 
	}

	public static void LoadSceneMenu() {
		//TODO
	}

	public static void TogglePause() {

		if(paused) {
			UnPause();
		} else {
			Pause();
		}

	}

	static void Pause() {
		//TODO stop time 
		//TODO bring up pause menu ui
	}

	static void UnPause() {
		//TODO close pause menu ui 
		//TODO restart time 
	}

}
