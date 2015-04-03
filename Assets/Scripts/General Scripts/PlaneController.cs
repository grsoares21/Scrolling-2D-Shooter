using UnityEngine;
using System.Collections;

/*
 * This class is used as a controller to the physical part of the plane
 * so everything related to the transform, rigidbody, collisions, etc are treated here
 * it constantly interates with the plane manager class which is responsible for the
 * plane's metadata and AI in case of enemies
 */

[RequireComponent(typeof(PlaneManager), typeof(Rigidbody2D))]
public class PlaneController : MonoBehaviour 
{
	public PlaneManager manager;

	public Camera mainCamera;
	public float planeSpeed; //base speed of the plane

	private float maxHorizontalDislocation; //movement restriction values
	private float maxVerticalDislocation;

	public bool restrictXToScreen; //if it's necessary to restrict the plane's to the screen
	public bool restrictYToScreen;

	public GameObject normalBullet; //game objects for the plane's bullets
	public GameObject specialBullet;

	// Use this for initialization
	void Start () 
	{
		if (mainCamera == null) 
			mainCamera = Camera.main;

		if (manager == null) 
			manager = GetComponent<PlaneManager> ();

		Vector3 upperCorner = new Vector3 (Screen.width, Screen.height, 0.0f);
		Vector3 maxDislocation = mainCamera.ScreenToWorldPoint(upperCorner);
		//Takes screen edges

		float planeHorizontalBounds = GetComponent<SpriteRenderer>().bounds.extents.x;
		float planeVerticalBounds = GetComponent<SpriteRenderer>().bounds.extents.y;
		//Takes in consideration the plane's sprite size

		maxHorizontalDislocation = maxDislocation.x - planeHorizontalBounds;
		maxVerticalDislocation = maxDislocation.y - planeVerticalBounds;
		//Establishes the maximum dislocation area;
	}
	
	// Update is called once per frame
	void Update () 
	{
	
	}

	// Update is called once per fixed framerate frame;
	void FixedUpdate() 
	{


	}

	void OnCollisionEnter2D(Collision2D collision)
	{
		if(collision.gameObject.tag == "Player")
		{ //I'm an enemy
			Collider2D[] colliders = GetComponents<Collider2D>();
			foreach(Collider2D currentCollider in colliders) 
			{
				currentCollider.enabled = false;
			}
			collision.gameObject.GetComponent<PlaneManager>().gotHit(manager.getCurrentHealth() / 10);			
			manager.gotHit(manager.getCurrentHealth()); //Dies on collision
		}			
		
	}



	//Moves the plane according to axis values;
	public void Move(float horizontalAxis, float verticalAxis) 
	{
		float newY = rigidbody2D.position.y + (verticalAxis * planeSpeed);
		float newX = rigidbody2D.position.x + (horizontalAxis * planeSpeed);
		//Calculates the new position according to plane's speed and axis

		if(restrictYToScreen)
			newY = Mathf.Clamp (newY, -maxVerticalDislocation, maxVerticalDislocation);
		if(restrictXToScreen)
			newX = Mathf.Clamp (newX, -maxHorizontalDislocation, maxHorizontalDislocation);
		//Clamps the new position inside the dislocation area

		Vector2 newPosition = new Vector2 (newX, newY);

		rigidbody2D.MovePosition(newPosition);
		//Moves the plane
	}

	public void FireNormalBullet()
	{
		GameObject.Instantiate (normalBullet, transform.position, transform.rotation);
	}

	public void FireSpecialBullet(GameObject targetedObject)
	{
		float planeHorizontalBounds = GetComponent<SpriteRenderer>().bounds.extents.x;
		//Gets plane's half size

		float distanceOfMissiles = (0.3515625f * planeHorizontalBounds) + (0.09375f * planeHorizontalBounds * (manager.nbSpecialBullets - 1));
		//In the sprite the first missile is at 35.15625% away between the center and the border
		//And each missile is 9.375% further than the previous one

		
		GameObject leftMissile = GameObject.Instantiate(specialBullet, new Vector3(transform.position.x - distanceOfMissiles, 
		                                                  transform.position.y, transform.position.z), transform.rotation) as GameObject;
		GameObject rightMissile = GameObject.Instantiate(specialBullet, new Vector3(transform.position.x + distanceOfMissiles, 
		                                                  transform.position.y, transform.position.z), transform.rotation) as GameObject;

		leftMissile.GetComponent<SpecialBulletController>().setTargetedObject(targetedObject);
		rightMissile.GetComponent<SpecialBulletController>().setTargetedObject(targetedObject);

	}



	public bool isOnScreen() 
	{
		float planeHorizontalBounds = GetComponent<SpriteRenderer>().bounds.extents.x;
		float planeVerticalBounds = GetComponent<SpriteRenderer>().bounds.extents.y;

		bool isVerticallyOnScreen = (transform.position.y < maxVerticalDislocation + planeHorizontalBounds) && 
									(transform.position.y > -maxVerticalDislocation - planeHorizontalBounds);
		bool isHorizontallyOnScreen = (transform.position.x < maxHorizontalDislocation + planeVerticalBounds) && 
									  (transform.position.x > -maxHorizontalDislocation - planeVerticalBounds);

		return isVerticallyOnScreen && isHorizontallyOnScreen;
	}

}
