using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

[System.Serializable]
public class DeckFile {
	public string name;
	public string file;
}

public class ChooseDeck : MonoBehaviour {

	public DeckFile[] decks;
	public Text deckText;
	public string helpScene;
	private int current;

	void Start () {
		current = 0;
		updateDeck();
	}

	void updateDeck(){
		deckText.text = decks[current].name;
		DeckManager.instance.deckFilename = decks[current].file;
	}
	
	public void forward(){
		current++;
		current = current % decks.Length;
		updateDeck();
	}

	public void back(){
		current--;
		if (current < 0) current = decks.Length - 1;
		updateDeck();
	}

	public void help(){
		SceneManager.LoadScene(helpScene);
	}
}
