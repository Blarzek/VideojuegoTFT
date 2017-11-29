using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class SingleScore : MonoBehaviour {

    public int rank;
    public string playerName;
    public int score;

	// Use this for initialization
	void Start () {
        UpdateText();
    }

	// Update is called once per frame
	void Update () {
		
	}

    public void UpdateText() {
        transform.GetChild(0).GetComponent<Text>().text = rank.ToString();
        transform.GetChild(1).GetComponent<Text>().text = playerName;
        transform.GetChild(2).GetComponent<Text>().text = score.ToString();
    }

    public SingleScore(int rank, string playerName, int score) {
        this.rank = rank;
        this.playerName = playerName;
        this.score = score;
    }

    public int GetRank() {
        return rank;
    }

    public string GetName() {
        return playerName;
    }

    public int GetScore() {
        return score;
    }
}
