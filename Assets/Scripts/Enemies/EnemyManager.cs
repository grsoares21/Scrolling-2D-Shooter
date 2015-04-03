using UnityEngine;
using System.Collections;

public class EnemyManager : PlaneManager {
	[Range(0f,100f)]
	public float chanceOfMoving = 100f; //from 0% to 100%;
	public float coolDownForMovingAgain = 1f; 
	public float intensityOfMovement = 1f;
	//Variables that will determine the randomness of the enemies' movement

	public int scorePoints;

	public bool firesSpecialBullet = false;
	public float specialBulletCooldown = float.PositiveInfinity;

	private float verticalMovementAxis = -3f;
	private float horizontalMovementAxis = 0;

	private bool appearedOnScreen = false;
	private bool isFiring = false;

	public GameObject targetCrosshair;

	// Use this for initialization
	void Start ()
	{
		currentHealth = health;
		if (controller == null)
			controller = GetComponent<PlaneController>();

		StartCoroutine(MovementDice());

	}
	
	// Update is called once per frame
	void Update () 
	{
		if(controller.isOnScreen() && !appearedOnScreen) //If just appeared on screen
		{
			appearedOnScreen = true;
		}

		if(!controller.isOnScreen() && appearedOnScreen) //If just disappeared from screen
		{
			Die();
		}

	}

	void FixedUpdate() 
	{
		controller.Move(horizontalMovementAxis, verticalMovementAxis);

		if(controller.isOnScreen()) {
			if(firesSpecialBullet && !isFiring)
				StartCoroutine(fireSpecialBullet());

			verticalMovementAxis = Mathf.Lerp(verticalMovementAxis, -1f, Time.fixedDeltaTime * 2.5f);
			horizontalMovementAxis = Mathf.Lerp(horizontalMovementAxis, 0f, Time.fixedDeltaTime * 2f); 
			//enemy tends to stop moving horizontally everytime it starts a horizontal movement, to see: MovementDice()
		}


	}

	//Method for when the enemy got hit by a bullet
	public override void gotHit (int hitStrenght) 
	{ 
		if(currentHealth <= 0) 
			return; //Prevents the enemy from "dying again"

		currentHealth -= hitStrenght;

		if(currentHealth <= 0){
			Explode();
			GameObject gameManager = GameObject.Find("GameManager");

			if(gameManager != null)
				gameManager.SendMessage("incrementScore", scorePoints);
		} 

	}

	//Method for when the enemy got targeted by a special bullet
	public void GotTargeted() 
	{
		GameObject crosshair = GameObject.Instantiate(targetCrosshair) as GameObject;
		crosshair.transform.position = this.transform.position;
		crosshair.transform.parent = this.transform;
	}

	private void Die() 
	{
		Destroy(this.gameObject);
	}



	IEnumerator MovementDice()
	{
		yield return new WaitForSeconds(coolDownForMovingAgain);
		if(controller.isOnScreen()) 
		{
			float willMove = Random.value;
			if(willMove <= chanceOfMoving/100f) 
			{
				horizontalMovementAxis = ((Random.value - 0.5f)*2) * intensityOfMovement; 
				//random value between -1 and 1 times the intensity of movement
			}
		}
		StartCoroutine(MovementDice());
	}

	IEnumerator fireSpecialBullet() 
	{
		isFiring = true;

		controller.FireSpecialBullet(GameObject.FindWithTag("Player"));

		yield return new WaitForSeconds(specialBulletCooldown);

		StartCoroutine(fireSpecialBullet());

	}


}
