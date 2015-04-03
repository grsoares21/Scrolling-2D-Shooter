using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class GameFlowController : MonoBehaviour {

	public GameObject initialMenu;
	public GameObject finalScreen;
	public GameObject HUD;
	public GameObject creditsScreen;

	private bool gameEnded = false;
	private bool gameStarted = false;
	// Use this for initialization
	void Start () {	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public bool hasGameStarted() 
	{
		return gameStarted;
	}

	public bool hasGameEnded()
	{
		return gameEnded;
	}


	public void startGame()
	{
		initialMenu.SetActive(false);
		HUD.SetActive(true);
		gameStarted = true;
		GetComponent<ScoreManager>().startTimeScore();

	}

	public void endGame()
	{
		gameEnded = true;
		finalScreen.SetActive(true);
	}

	public void exitGame()
	{
		Application.Quit();
	}

	public void activateCreditsScreen()
	{
		creditsScreen.SetActive(true);
	}

	public void deactivateCreditsScreen()
	{
		creditsScreen.SetActive(false);
	}

}
