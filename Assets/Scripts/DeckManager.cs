using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.SceneManagement;

public class DeckManager : MonoBehaviour {

	[HideInInspector] public static DeckManager instance;
	public string deckFilename;
	public string loadingScene;
	public char[] usedCards;
	public IrineuCard[] cards;

	private bool isReady = false;
	private char usedCard = 't';
	private char notUsedCard = 'f'; 

	void Start () {
		// singleton instance
		if (instance == null)
			instance = this;
		else if (instance != this)
			Destroy(gameObject);

		DontDestroyOnLoad(gameObject);
	}

	private void shuffleCards(){
		for (int t = 0; t < cards.Length; t++){
			IrineuCard tmp = cards[t];
			int r = Random.Range(t, cards.Length);
			cards[t] = cards[r];
			cards[r] = tmp;
		}
	}

	private void createUsedCardsChecker(int total){
		usedCards = new char[total];
		for (int i = 0; i < total; i++) usedCards[i] = notUsedCard;
	}

	private string charArrayToString(char[] arr){
		string str = "";
		for (int i = 0; i < arr.Length; i++)
			str += arr[i];
		return str;
	}

	public void loadDeckFile(){
		SceneManager.LoadScene(loadingScene);

		TextAsset txtAsset = Resources.Load(deckFilename) as TextAsset;
		Debug.Log(txtAsset);
		string dataAsJson = txtAsset.text;

		IrineuDeck deck = JsonUtility.FromJson<IrineuDeck>(dataAsJson);
		cards = deck.cards;
		shuffleCards();

		Debug.Log("Deck '" + deck.name + "' opened.");

		// checking used cards string for this deck
		if (PlayerPrefs.HasKey(deckFilename)){
			Debug.Log("used cards exist in playerprefs");
			usedCards = PlayerPrefs.GetString(deckFilename).ToCharArray();
			Debug.Log(PlayerPrefs.GetString(deckFilename));

			if (usedCards.Length != deck.cardTotal){
				Debug.Log("update used cards by new deck");
				createUsedCardsChecker(deck.cardTotal);
			} else {
				int counter = 0;
				for (int i = 0; i < deck.cardTotal; i++)
					if (usedCards[i] == notUsedCard) counter++;

				if (counter < PlayerManager.instance.turnsPerPlayer * PlayerManager.instance.playerCount){
					Debug.Log("forget that, start a new used cards checker");
					createUsedCardsChecker(deck.cardTotal);
				}
			}
		} else {
			Debug.Log("used cards DON'T exist in playerprefs");
			createUsedCardsChecker(deck.cardTotal);
		}

		Debug.Log(charArrayToString(usedCards));
		isReady = true;
	}

	public bool isCardUsed(int cardId){
		return usedCards[cardId] == usedCard ? true : false;
	}

	public void useCard(int cardId){
		usedCards[cardId] = usedCard;
	}

	public void saveUsedCardsData(){
		PlayerPrefs.SetString(deckFilename, charArrayToString(usedCards));
		PlayerPrefs.Save();
	}

	public bool deckIsReady(){
		return isReady;
	}

	void Update () {
		
	}
}
