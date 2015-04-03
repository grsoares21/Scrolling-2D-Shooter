using UnityEngine;
using System.Collections;

[RequireComponent(typeof(GameFlowController))]
public class EnemyGenerator : MonoBehaviour {

	private Vector3 screenUpperCorner;
	private Vector3 worldUpperCorner;

	public float initialEnemyCooldown;
	public float finalEnemyCooldown;

	public float currentEnemyCooldown;

	public float cooldownDecreaseSpeed;

	public GameObject[] ListOfEnemies;
	// Use this for initialization
	void Start () {
		currentEnemyCooldown = initialEnemyCooldown;

		screenUpperCorner = new Vector3 (Screen.width, Screen.height, 0.0f);
		worldUpperCorner = Camera.main.ScreenToWorldPoint(screenUpperCorner);

		StartCoroutine(generationDice());
	}

	void Update () {
		if(!GetComponent<GameFlowController>().hasGameStarted())
			return;

		currentEnemyCooldown = Mathf.Lerp(currentEnemyCooldown, finalEnemyCooldown, Time.deltaTime * cooldownDecreaseSpeed);
	}

	void generateEnemy(GameObject enemy, Vector2 position) {
		GameObject.Instantiate(enemy, position, Quaternion.identity);
	}

	IEnumerator generationDice() 
	{
		if(GetComponent<GameFlowController>().hasGameStarted()) 
		{
			int randomEnemyIndex = Mathf.RoundToInt(Random.value * (ListOfEnemies.Length - 1));
			GameObject randomEnemy = ListOfEnemies[randomEnemyIndex];
			
			float enemyHorizontalBounds = randomEnemy.GetComponent<SpriteRenderer>().bounds.extents.x;
			float enemyVerticalBounds = randomEnemy.GetComponent<SpriteRenderer>().bounds.extents.y;
			
			float randomX = ((Random.value - 0.5f) * 2) * (worldUpperCorner.x - enemyHorizontalBounds); 
			//Generates a random x value that is between the left and right borders of the screen guaranteeing that the enemy will by onscreen 
			
			Vector2 generationPosition = new Vector2(randomX, worldUpperCorner.y + (enemyVerticalBounds * 4f));
			//Generates a position with our previous random x value and a y value that is a little above screen
			
			generateEnemy(randomEnemy, generationPosition);
		}

		yield return new WaitForSeconds(currentEnemyCooldown);

		StartCoroutine(generationDice());
	}

}
