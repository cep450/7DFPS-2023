using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*

	Controls high level game state: routing between scenes, pausing, unpausing, ect.
	Singleton.

*/

public class GameManager : MonoBehaviour
{

	public static bool paused { get; private set; }
    
	public void LoadSceneGame() {
		//TODO 
	}

	public void LoadSceneMenu() {
		//TODO
	}

	public void ExitGame() {
		//TODO
	}

	public void TogglePause() {

		if(paused) {
			UnPause();
		} else {
			Pause();
		}

	}

	void Pause() {
		//TODO stop time 
		//TODO bring up pause menu ui
	}

	void UnPause() {
		//TODO close pause menu ui 
		//TODO restart time 
	}

}
