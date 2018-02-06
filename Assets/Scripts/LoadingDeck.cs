using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadingDeck : MonoBehaviour {

	public string gameScene;

	void Update () {
		if (DeckManager.instance.deckIsReady())
			SceneManager.LoadScene(gameScene);
	}
}
