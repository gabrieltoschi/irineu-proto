using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

[System.Serializable]
public class Instruction {
	public string title;
	public string text;
}

public class InstructionsManager : MonoBehaviour {

	public Instruction[] texts;

	public Text uiTitle;
	public Text uiText;
	public Text uiCounter;
	public string mainMenuScene;

	private int current;

	void Start () {
		current = 0;
		updateText();
	}

	public void updateText(){
		uiTitle.text = texts[current].title;
		uiText.text = texts[current].text;
		int count = current + 1;
		uiCounter.text = count.ToString() + "/" + texts.Length.ToString();
	}

	public void forward(){
		current++;
		current = current % texts.Length;
		updateText();
	}

	public void back(){
		current--;
		if (current < 0) current = texts.Length - 1;
		updateText();
	}

	public void mainMenu(){
		Destroy(DeckManager.instance.gameObject);
		Destroy(PlayerManager.instance.gameObject);
		SceneManager.LoadScene(mainMenuScene);
	}
}
