using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour {

	[Header("Game UI References")]
	public GameObject playerUI;
	public GameObject themeUI;
	public GameObject questionUI;
	public GameObject answerUI;
	public Text turnText;

	[Header("Game Configuration")]
	public int wrongAnswer = 1;
	public int firstChallenge = 1;
	public int secondChallenge = 2;
	public string endGameScene;

	[Header("Color Configuration")]
	public Color answeringColor;
	public Color wrongColor;
	public Color correctColor;
	public Color challenge1Color;
	public Color challenge2Color;

	[Header("Game Data")]
	public int currentCard;
	public int turns;
	public int turnMax;
	public int playerAnswering;
	public int playerChallenger1;
	public int playerChallenger2;

	public void Start () {
		currentCard = -1;
		turns = 0;
		nextCard();

		turnMax = PlayerManager.instance.turnsPerPlayer * PlayerManager.instance.playerCount;
		updatePlayerNames();
		startTheme();
	}

	private void nextCard(){
		do {
			if (currentCard >= 0) DeckManager.instance.useCard(DeckManager.instance.cards[currentCard].id);
			currentCard++;
			if (currentCard >= DeckManager.instance.cards.Length) return;
			Debug.Log("checking card #" + currentCard + " (id " + DeckManager.instance.cards[currentCard].id + ")");
		} while (DeckManager.instance.isCardUsed(DeckManager.instance.cards[currentCard].id));
	}

	public void updatePlayerNames(){
		for (int i = 0; i < 6; i++){
			GameObject playerObj = playerUI.transform.Find("Player " + i).gameObject;
			if (i >= PlayerManager.instance.playerCount){
				playerObj.SetActive(false);
				continue;
			}

			playerObj.transform.Find("Name").GetComponent<Text>().text = PlayerManager.instance.players[i].name;
			playerObj.transform.Find("Points").GetComponent<Text>().text = PlayerManager.instance.players[i].points.ToString();
		}
	}

	public void startTheme(){
		themeUI.transform.Find("Theme Text").gameObject.GetComponent<Text>().text = DeckManager.instance.cards[currentCard].theme;
		turnText.text = "Turno " + (turns + 1) + "/" + turnMax;

		playerAnswering = turns % PlayerManager.instance.playerCount;
		playerChallenger1 = -1;
		playerChallenger2 = -1;

		changeAllPlayerButtonsStatus(false);
		playerUI.transform.Find("Player " + playerAnswering).Find("Point Space").GetComponent<Image>().color = answeringColor;

		themeUI.transform.Find("Answer Button").gameObject.SetActive(true);
		themeUI.transform.Find("Challenge Button").gameObject.SetActive(true);

		themeUI.SetActive(true);
		playerUI.SetActive(true);
	}

	private void changeAllPlayerButtonsStatus(bool newStatus){
		for (int i = 0; i < PlayerManager.instance.playerCount; i++)
			playerUI.transform.Find("Player " + i).gameObject.GetComponent<Button>().enabled = newStatus;
	}

	public void startChallenge(){
		changeAllPlayerButtonsStatus(true);
		themeUI.transform.Find("Answer Button").gameObject.SetActive(false);
		themeUI.transform.Find("Challenge Button").gameObject.SetActive(false);

		if (playerChallenger1 == -1){ // 1st challenge
			playerUI.transform.Find("Player " + playerAnswering).GetComponent<Button>().interactable = false;
		} else { // 2nd challenge
			playerUI.transform.Find("Player " + playerAnswering).GetComponent<Button>().interactable = false;
			playerUI.transform.Find("Player " + playerChallenger1).GetComponent<Button>().interactable = false;
		}
	}

	public void finishChallenge(int challenged){
		changeAllPlayerButtonsStatus(false);

		if (playerChallenger1 == -1){ // 1st challenge
			playerChallenger1 = playerAnswering;
			playerAnswering = challenged;

			playerUI.transform.Find("Player " + playerChallenger1).GetComponent<Button>().interactable = true;
			playerUI.transform.Find("Player " + playerChallenger1).Find("Point Space").GetComponent<Image>().color = challenge1Color;
			playerUI.transform.Find("Player " + playerAnswering).Find("Point Space").GetComponent<Image>().color = answeringColor;

			themeUI.transform.Find("Answer Button").gameObject.SetActive(true);
			themeUI.transform.Find("Challenge Button").gameObject.SetActive(true);
		} else { // 2nd challenge
			playerChallenger2 = playerAnswering;
			playerAnswering = challenged;

			playerUI.transform.Find("Player " + playerChallenger1).GetComponent<Button>().interactable = true;
			playerUI.transform.Find("Player " + playerChallenger2).GetComponent<Button>().interactable = true;
			playerUI.transform.Find("Player " + playerChallenger2).Find("Point Space").GetComponent<Image>().color = challenge2Color;
			playerUI.transform.Find("Player " + playerAnswering).Find("Point Space").GetComponent<Image>().color = answeringColor;

			themeUI.transform.Find("Answer Button").gameObject.SetActive(true);
		}
	}

	public void startQuestion(){
		themeUI.SetActive(false);
		playerUI.SetActive(false);

		questionUI.transform.Find("Question Text").gameObject.GetComponent<Text>().text = DeckManager.instance.cards[currentCard].question;
		for (int i = 0; i < 4; i++)
			questionUI.transform.Find("Answer " + i).Find("Text").GetComponent<Text>().text = DeckManager.instance.cards[currentCard].answers[i];
		questionUI.SetActive(true);
	}

	public void startAnswer(int answerSelected){
		questionUI.SetActive(false);

		checkAnswer(answerSelected);

		playerUI.transform.Find("Player " + playerAnswering).Find("Points").GetComponent<Text>().text = PlayerManager.instance.players[playerAnswering].points.ToString();
		if (playerChallenger1 != -1)
			playerUI.transform.Find("Player " + playerChallenger1).Find("Points").GetComponent<Text>().text = PlayerManager.instance.players[playerChallenger1].points.ToString();
		if (playerChallenger2 != -1)
			playerUI.transform.Find("Player " + playerChallenger2).Find("Points").GetComponent<Text>().text = PlayerManager.instance.players[playerChallenger2].points.ToString();

		answerUI.transform.Find("Answer Text").gameObject.GetComponent<Text>().text = DeckManager.instance.cards[currentCard].answers[DeckManager.instance.cards[currentCard].correct];
		answerUI.transform.Find("Answer Fact").gameObject.GetComponent<Text>().text = DeckManager.instance.cards[currentCard].fact;
		answerUI.SetActive(true);
		playerUI.SetActive(true);
	}

	private bool checkAnswer(int selected){
		if (selected != DeckManager.instance.cards[currentCard].correct){
			// WRONG ANSWER
			PlayerManager.instance.changePoints(playerAnswering, -wrongAnswer);
			playerUI.transform.Find("Player " + playerAnswering).Find("Point Space").GetComponent<Image>().color = wrongColor;

			if (playerChallenger2 != -1){ // 2nd challenge exists, Challenger 2 wins 1nd challenge
				PlayerManager.instance.changePoints(playerChallenger2, firstChallenge);
				PlayerManager.instance.changePoints(playerChallenger1, -firstChallenge);
				playerUI.transform.Find("Player " + playerChallenger1).Find("Point Space").GetComponent<Image>().color = wrongColor;
				playerUI.transform.Find("Player " + playerChallenger2).Find("Point Space").GetComponent<Image>().color = correctColor;
			} else if (playerChallenger1 != -1){ // 1st challenge exists, Challenger 1 wins 1nd challenge
				playerUI.transform.Find("Player " + playerChallenger1).Find("Point Space").GetComponent<Image>().color = correctColor;
			}

			return false;
		}

		// CORRECT ANSWER
		playerUI.transform.Find("Player " + playerAnswering).Find("Point Space").GetComponent<Image>().color = correctColor;

		if (playerChallenger2 != -1){ // 2nd challenge exists, Answering wins 2nd challenge
			PlayerManager.instance.changePoints(playerAnswering, secondChallenge);
			PlayerManager.instance.changePoints(playerChallenger2, -secondChallenge);
			playerUI.transform.Find("Player " + playerChallenger1).Find("Point Space").GetComponent<Image>().color = correctColor;
			playerUI.transform.Find("Player " + playerChallenger2).Find("Point Space").GetComponent<Image>().color = wrongColor;
		} else if (playerChallenger1 != -1){ // 1st challenge exists, Answering wins 1nd challenge
			PlayerManager.instance.changePoints(playerAnswering, firstChallenge);
			PlayerManager.instance.changePoints(playerChallenger1, -firstChallenge);
			playerUI.transform.Find("Player " + playerChallenger1).Find("Point Space").GetComponent<Image>().color = wrongColor;
		}

		return true;
	}
	 
	public void startNewTurn(){
		answerUI.SetActive(false);
		playerUI.SetActive(false);

		playerUI.transform.Find("Player " + playerAnswering).Find("Point Space").GetComponent<Image>().color = Color.white;
		if (playerChallenger1 != -1)
			playerUI.transform.Find("Player " + playerChallenger1).Find("Point Space").GetComponent<Image>().color = Color.white;
		if (playerChallenger2 != -1)
			playerUI.transform.Find("Player " + playerChallenger2).Find("Point Space").GetComponent<Image>().color = Color.white;

		nextCard();
		turns++;
		if (turns >= turnMax) {
			SceneManager.LoadScene(endGameScene);
		}
		else {
			startTheme();
		}
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
