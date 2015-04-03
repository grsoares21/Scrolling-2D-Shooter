using UnityEngine;
using UnityEngine.UI;
using System.Collections;

/*
 * This class handles the metadate and inputs from the player
 * so everything related to bullets, life, inputs are treated here
 * it constantly interates with the player controller class
 * with responsible for the physical control of the player
 */

[RequireComponent(typeof(Animator))]
public class PlayerManager : PlaneManager {

	public Animator animator;

	public GameObject HUD;
	
	public float normalBulletCooldown;
	public float specialBulletCooldown;
	public float specialBulletRechargeCooldown;

	private bool recharging;

	private bool ableToFireNormalBullet = true;
	private bool ableToFireSpecialBullet = true;

	private GameObject GameManager;

	// Use this for initialization
	void Start () {
		currentHealth = health;
		 
		if (animator == null)
			animator = GetComponent<Animator> ();
		if (controller == null)
			controller = GetComponent<PlaneController>();

		GameManager = GameObject.Find("GameManager");
	}
	
	// Update is called once per frame
	void Update () 
	{
		animator.SetInteger("NumberOfSpecialBullets", nbSpecialBullets);

		Slider healthBar = HUD.transform.FindChild("HealthBar").GetComponent<Slider>();
		healthBar.value = currentHealth;
	}

	void FixedUpdate () 
	{
		GameFlowController flowController = GameManager.GetComponent<GameFlowController>();
		//Gets fire inputs
		if (Input.GetButton("Fire1") && isAbleToFireNormalBullet() && flowController.hasGameStarted()) 
		{
			controller.FireNormalBullet();
			ableToFireNormalBullet = false;
			StartCoroutine(enableNormalFiring());
		}
		if(Input.GetButton("Fire2") && isAbleToFireSpecialBullet() && isAbleToFireNormalBullet() && flowController.hasGameStarted())
		{
			GameObject targetedObject = null;

			Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
			Collider2D clickedCollider = Physics2D.Raycast(mousePosition, Vector2.zero).collider;

			if(clickedCollider != null) {
				targetedObject = clickedCollider.gameObject;

				if(targetedObject.tag == "Enemy") {
					targetedObject.GetComponent<EnemyManager>().GotTargeted();
				}
			}	 


			controller.FireSpecialBullet(targetedObject);
			nbSpecialBullets--;

			if(!recharging)
				StartCoroutine(incrementSpecialBullets());

			ableToFireSpecialBullet = false;
			StartCoroutine(enableSpecialFiring());
		}

		//Gets movement Inputs
		float horizontalAxis = Input.GetAxis ("Horizontal");
		float verticalAxis = Input.GetAxis ("Vertical");

		if(!flowController.hasGameStarted())
			horizontalAxis = verticalAxis = 0f;

		controller.Move(horizontalAxis, verticalAxis);
	}

	bool isAbleToFireSpecialBullet() 
	{
		return (nbSpecialBullets > 0) && ableToFireSpecialBullet;
	}

	bool isAbleToFireNormalBullet() 
	{
		return ableToFireNormalBullet;
	}

	IEnumerator enableNormalFiring()
	{
		yield return new WaitForSeconds(normalBulletCooldown);
		ableToFireNormalBullet = true;
	}

	IEnumerator enableSpecialFiring()
	{
		yield return new WaitForSeconds(specialBulletCooldown);
		ableToFireSpecialBullet = true;
	}

	IEnumerator incrementSpecialBullets()
	{
		recharging = true;

		yield return StartCoroutine(fillMissileHUD(0f));
		//the actual timing for incrementing the number of bullets is controlled by the HUD changing coroutine, so that
		//the HUD has a good precision according to the actual player data

		nbSpecialBullets++;
		if(nbSpecialBullets < 3) 
			StartCoroutine(incrementSpecialBullets());
		else
			recharging = false;
	}

	IEnumerator fillMissileHUD(float fill)
	{
		GameObject missileSprite = HUD.transform.FindChild("Missile" + (nbSpecialBullets + 1)).gameObject;
		//always fills the right missile on the HUD according to how many missiles the player has
		int i = nbSpecialBullets + 2;
		while(i <= 3)
		{
			HUD.transform.FindChild("Missile" + i).gameObject.GetComponent<Image>().fillAmount = 0f;
			i++;
		}
			

		float newFill = fill + (1f/8f);
		missileSprite.GetComponent<Image>().fillAmount = fill;

		//fills the HUD missile in eight steps

		if(fill >= 1f)
			yield break;

		yield return new WaitForSeconds(specialBulletRechargeCooldown / 8);

		yield return StartCoroutine(fillMissileHUD(newFill));


	}

	public override void gotHit(int hitStrength) 
	{
		currentHealth -= hitStrength;

		if(currentHealth <= 0) {
			currentHealth = 0;
			Explode();
		}

	}
	

	void Die() 
	{
		GameObject.Find("GameManager").GetComponent<GameFlowController>().endGame();
		Destroy(this.gameObject);
	}

}
