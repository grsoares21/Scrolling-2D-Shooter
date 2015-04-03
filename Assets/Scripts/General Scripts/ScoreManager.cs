using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ScoreManager : MonoBehaviour {

	public Text scoreText;
	private int score = 0;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		scoreText.text = "SCORE: " + score;
	}

	public void incrementScore(int points) 
	{
		score += points;
	}

	public void startTimeScore()
	{
		StartCoroutine(timeScore());
	}

	IEnumerator timeScore()
	{
		if(GetComponent<GameFlowController>().hasGameEnded())
			yield break;

		yield return new WaitForSeconds(0.5f);
		score++;
		StartCoroutine(timeScore());
	}
}
