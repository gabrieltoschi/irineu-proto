using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum PlayerStatus {
	Null,
	A, // first reader or first challenger
	B, // second reader or second challenger
	C // third reader
}

[System.Serializable]
public class Player {
	public string name;
	public PlayerStatus status;
	public int points;
}

public class PlayerManager : MonoBehaviour {

	[HideInInspector] public static PlayerManager instance;

	[Header("Start Game Player Configuration")]
	public int startPoints = 10;
	public int turnsPerPlayer = 2;

	[Header("Start Game UI")]
	public Text playerNumberText;
	public Text[] playersText = new Text[6];

	[Header("Player Data")]
	public int playerCount;
	public Player[] players;

	void Start () {
		// singleton instance
		if (instance == null)
			instance = this;
		else if (instance != this)
			Destroy(gameObject);

		DontDestroyOnLoad(gameObject);
	}

	public void startPlayers(){
		playerCount = 0;

		for (int i = 0; i < 6; i++)
			if (playersText[i].text != "") playerCount++;

		if (playerCount < 3){
			playerNumberText.color = Color.red;
			playerNumberText.fontStyle = FontStyle.Bold;
			return;
		}

		players = new Player[playerCount];
		int j = 0;
		for (int i = 0; i < 6; i++){
			if (playersText[i].text != ""){
				players[j] = new Player();
				players[j].name = playersText[i].text;
				players[j].status = PlayerStatus.Null;
				players[j].points = startPoints;
				j++;
			}
		}

		// start deck manager
		DeckManager.instance.loadDeckFile();
	}

	public void changePoints(int player, int offset){
		players[player].points += offset;
	}
}
