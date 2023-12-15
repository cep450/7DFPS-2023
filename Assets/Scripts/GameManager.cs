using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/*

	Controls high level game state: routing between scenes, pausing, unpausing, ect.
	Singleton.

*/

public class GameManager : MonoBehaviour
{

	public static bool paused { get; private set; }

	//TODO this class should probably also manage game speed if another class doesn't 
	//for both pausing and slow time 
	//though maybe another class? to keep game fucntionality away from high level stuff 
    
	public void LoadSceneGame() {
		SceneManager.LoadScene("Game");
	}

	public void LoadSceneMenu() {
		SceneManager.LoadScene("GameMenu");
	}

	public void ExitGame() {
		Application.Quit();
	}

	public void TogglePause() {

		if(paused) {
			UnPause();
		} else {
			Pause();
		}

	}

	void Pause() {
		paused = true;
		//TODO stop time 
		//TODO bring up pause menu ui
	}

	void UnPause() {
		//TODO close pause menu ui 
		//TODO restart time 
		paused = false;
	}

}
