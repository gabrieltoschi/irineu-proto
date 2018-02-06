using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Ranking {
	public Ranking(int Player, int Points){
		this.player = Player;
		this.points = Points;
	}

	public override string ToString(){
		return "Player: " + player + " (" + points + ")";
	}

	public int player;
	public int points;
}

public class EndGame : MonoBehaviour {

	public Color winColor;
	public string menuScene;

	void Start () {
		DeckManager.instance.saveUsedCardsData();
		updateAllPlayers();

		List<Ranking> ranking = new List<Ranking>();
		for (int i = 0; i < PlayerManager.instance.playerCount; i++)
			ranking.Add(new Ranking(i, PlayerManager.instance.players[i].points));

		ranking.Sort(delegate (Ranking a, Ranking b) {
			return (a.points.CompareTo(b.points));
		});

		Ranking[] sortedRanking = ranking.ToArray();
		int maxPoints = sortedRanking[PlayerManager.instance.playerCount - 1].points;
		Debug.Log(maxPoints);
		for (int i = PlayerManager.instance.playerCount - 1; i >= 0; i--){
			if (sortedRanking[i].points == maxPoints){
				transform.Find("Player " + sortedRanking[i].player).Find("Point Space").GetComponent<Image>().color = winColor;
			} else {
				transform.Find("Player " + sortedRanking[i].player).gameObject.GetComponent<Button>().interactable = false;
			}
		}
	}

	void updateAllPlayers(){
		for (int i = 0; i < 6; i++){
			GameObject playerObj = transform.Find("Player " + i).gameObject;
			if (i >= PlayerManager.instance.playerCount){
				playerObj.SetActive(false);
				continue;
			}

			playerObj.transform.Find("Name").GetComponent<Text>().text = PlayerManager.instance.players[i].name;
			playerObj.transform.Find("Points").GetComponent<Text>().text = PlayerManager.instance.players[i].points.ToString();
		}
	}

	public void restartGame(){
		Destroy(DeckManager.instance);
		Destroy(PlayerManager.instance);
		SceneManager.LoadScene(menuScene);
	}
}
