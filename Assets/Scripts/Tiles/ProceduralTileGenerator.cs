using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody2D))]
public class ProceduralTileGenerator : MonoBehaviour {

	public GameObject[] waterTileArray;
	//The border tiles should be ordered to match with the bitmap strategie
	//Top tile = 2^0 Right tile = 2^1 Bottom tile = 2^2 Left tile = 2^3
	//Considering that the coeficients are 0 for earth tiles and 1 for water tiles
	//The index of the corresponding tile should be calculated by multiplying the coeficients by its corresnponding value and summing it all
	//This can be interpreted as the binary number formed by the four adjacent tile in the order left, bottom, right, top and considering
	//the bit as a 1 if it's a water tile and the bit 0 if it's an earth tile 

	public GameObject earthTile;
	public GameObject gameManager;


	public float tileSpeed;

	private int[,] matrix;

	private GameObject lastCreatedTile;

	private Vector3 worldTopRightCorner;
	private Vector3 worldBottomLeftCorner;

	private float earthHeight;

	private Vector3 nextTileSetPos;

	private bool hasCreatedSucessor = false;
	// Use this for initialization
	void Start () {;

		SpriteRenderer earthRenderer = earthTile.GetComponent<SpriteRenderer>();
		float earthWidth = earthRenderer.bounds.size.x;
		earthHeight = earthRenderer.bounds.size.y;

		worldTopRightCorner = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, 0f));
		worldBottomLeftCorner = Camera.main.ScreenToWorldPoint(Vector3.zero);

		float screenWidthInUnits = worldTopRightCorner.x - worldBottomLeftCorner.x;
		float screenHeightInUnits = worldTopRightCorner.y - worldBottomLeftCorner.y;

		int tileColumns = Mathf.RoundToInt(screenWidthInUnits / earthWidth) + 2; 
		int tileRows = (Mathf.RoundToInt(screenHeightInUnits / earthHeight) + 2);
		//+2 to be sure that it will cover the whole screen even with the rounding
		//* 2 to create new instances without gaps

		matrix = new int[tileColumns, tileRows];
		for(int i = 0; i < tileColumns; i++) {
			for(int j = 0; j < tileRows; j++) {
				matrix[i, j] = Mathf.RoundToInt(Random.value + 0.04f); //Small tendency to water tiles (value: 1)
			}
		}

		int[] born678 = {0, 0, 0, 0, 0, 0, 1, 1, 1}, 
			  born5678 = {0, 0, 0, 0, 0, 1, 1, 1, 1},
			  survive345678 = {0, 0, 0, 1, 1, 1, 1, 1, 1};
		int[] survive5678 = born5678;


		matrix = ApplyCellularAutomaton(matrix, born5678, survive5678); //Applies smoothing automata	
		for(int i = 0; i < 20; i++) {
				matrix = ApplyCellularAutomaton(matrix, born678, survive345678); //Applies reduction automata
		}
		matrix = ApplyCellularAutomaton(matrix, born5678, survive5678); //Applies smoothing automata
		for(int i = 0; i < 20; i++) {
			matrix = ApplyCellularAutomaton(matrix, born678, survive345678); //Applies reduction automata
		}

		//This combination was chosen using an interactive cellular automata visualizer, referred on the credits

		for(int i = 0; i < tileColumns; i++) 
		{
			for(int j = 0; j < tileRows; j++)
			{
				Vector2 newTilePosition = new Vector2((earthWidth * i) + worldBottomLeftCorner.x, earthHeight * (j - 1) + transform.position.y);

				if(matrix[i, j] == 0) 
				{
					lastCreatedTile = GameObject.Instantiate(earthTile, newTilePosition, Quaternion.identity) as GameObject;
					lastCreatedTile.transform.parent = this.transform;	
				} else
				{
					//Bitmap method for tile choice
					int topValue, rightValue, bottomValue, leftValue;

					if(j == tileRows - 1)
						topValue = 1;
					else
						topValue = matrix[i, j+1];
					if(i == tileColumns - 1)
						rightValue = 1;
					else
						rightValue = matrix[i+1, j];
					if(j == 0)
						bottomValue = 1;
					else
						bottomValue = matrix[i, j-1];
					if(i == 0)
						leftValue = 1;
					else
						leftValue = matrix[i-1, j];
					//Consider the inexistent borders as if they were water

					int index = topValue + rightValue * 2 + bottomValue * 4 + leftValue * 8;
					int randomIndex = index + (Mathf.RoundToInt(Random.value) * 16); //There are 2 different tiles to each combination

					lastCreatedTile = GameObject.Instantiate(waterTileArray[randomIndex], newTilePosition, Quaternion.identity) as GameObject;
					lastCreatedTile.transform.parent = this.transform;
				}
			}
		}

		rigidbody2D.isKinematic = true;
		rigidbody2D.velocity = -Vector2.up * tileSpeed;


	}
	
	// Update is called once per frame
	void Update ()
	{
		float distBetweenTileGenerators = lastCreatedTile.transform.localPosition.y + 2*earthHeight;
		nextTileSetPos = new Vector3(transform.position.x, transform.position.y + distBetweenTileGenerators);

		if(transform.position.y < worldTopRightCorner.y && !hasCreatedSucessor) 
		{
			gameManager.GetComponent<TileGenerator>().InstantiateNextTileSet();
			hasCreatedSucessor = true;

			/*
			 * This was implemented this way so that the creation of the tile sets and the creation of the tiles could be done separately
			 * There are 2 major reasons for this choice
			 * 1st: This way we can generate tile sets with different rules of procedural generation as the game advances
			 * 2nd: In unity, in prefabs that reference themselves the reference is interpreted as a reference to the current game object and not to 
			 * the actual prefab
			 */

		}
		if((lastCreatedTile.transform.position.y + lastCreatedTile.GetComponent<SpriteRenderer>().bounds.extents.y) < worldBottomLeftCorner.y)
		{
			Die();
		}


	}

	void Die()
	{
		Destroy(this.gameObject);
	}

	public Vector3 getNextTileSetPos() {
		return nextTileSetPos;
		//It's a good practice in unity to leave only variables used by the editor as public
	}

	/*
	 *Implements a Life-like Cellular automata...
	 *Since the used matrixes are integers it's necessary to be careful to avoid illegal memory access
	 *It is recommended to use only 1's and 0's in the matrixes and int vectors of size 9 for the definition of the automata
	 *Where 1 means alive and 0 means dead.
	 *Integers are used to help tile choice algorithm 
	 */
	int[,] ApplyCellularAutomaton(int[,] givenMatrix, int[] born, int[] survive) 
	{
		int columns = givenMatrix.GetLength(0), rows = givenMatrix.GetLength(1);
		int[,] newMatrix = new int[givenMatrix.GetLength(0), givenMatrix.GetLength(1)];

		for(int i = 0; i < columns; i++) 
		{
			for(int j = 0; j < rows; j++) 
			{
				if(i == 0 || i == columns - 1 || j == 0 || j == rows - 1) {
					newMatrix[i, j] = 1; //The edges are alive
					continue;
				}

				int nextState = givenMatrix[i-1, j-1] + givenMatrix[i, j-1] + givenMatrix[i+1, j-1] + 
								givenMatrix[i-1, j]   + 		 0   		+ givenMatrix[i+1, j]   + 
								givenMatrix[i-1, j+1] + givenMatrix[i, j+1] + givenMatrix[i+1, j+1];

				if(givenMatrix[i, j] == 1) //If it was already alive
					newMatrix[i, j] = survive[nextState];
				else
					newMatrix[i, j] = born[nextState];
					
			}
		}

		return newMatrix;
	} 
}
