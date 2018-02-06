[System.Serializable]
public class IrineuCard {
	public int id;
	public string type;
	public string theme;
	public string question;
	public string[] answers;
	public int correct;
	public string fact;
}

[System.Serializable]
public class IrineuDeck {
	public string name;
	public int cardTotal;
	public IrineuCard[] cards;
}
