using UnityEngine;
using System.Collections;

public class TileGenerator : MonoBehaviour {

	public GameObject proceduralTileSet;

	private GameObject lastTileSetCreated;

	// Use this for initialization
	void Start () {
		Vector3 centerOfScreen = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width / 2, 0f, 0f));
		InstantiateTileSet(centerOfScreen, proceduralTileSet);
	}

	
	// Update is called once per frame
	void Update () {
	
	}

	public void InstantiateTileSet(Vector3 slotPosition, GameObject tileSet) 
	{
		lastTileSetCreated = GameObject.Instantiate(tileSet, slotPosition, Quaternion.identity) as GameObject;
		lastTileSetCreated.GetComponent<ProceduralTileGenerator>().gameManager = this.gameObject;
	}

	public void InstantiateNextTileSet() 
	{
		Vector3 nextTileSetPos = lastTileSetCreated.GetComponent<ProceduralTileGenerator>().getNextTileSetPos();
		InstantiateTileSet(nextTileSetPos, proceduralTileSet);
	}
}
