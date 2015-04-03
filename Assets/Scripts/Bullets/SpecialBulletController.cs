using UnityEngine;
using System.Collections;

[RequireComponent(typeof(BulletController))]
public class SpecialBulletController : MonoBehaviour {

	public enum BehaviourChoices{followPerfect, followImperfect, Direction};
	
	public BehaviourChoices followingBehaviour;

	public float finalSpeed;
	public float acceleration;
	public float initialTurningSpeed;
	public float finalTurningSpeed;

	/*
	 * Since the bullet controller moves the bullet always to the direction it's pointed
	 * the aiming is done just by changing the angle of the bullets
	 */
	protected GameObject targetedObject = null; 
	// Use this for initialization
	void Start () {
		if(followingBehaviour == BehaviourChoices.Direction && targetedObject != null) //Initially adjusts the angle to the target
		{
			Vector2 targetPosition = targetedObject.transform.position;
			
			float newAngle = Mathf.Rad2Deg * Mathf.Atan2(transform.position.x - targetPosition.x,targetPosition.y - transform.position.y);
			
			rigidbody2D.MoveRotation(newAngle);
		}
	}

	// Update is called once per frame
	void Update() {

	}

	void FixedUpdate () {
		if(targetedObject == null)
			return;


		if(followingBehaviour == BehaviourChoices.followPerfect)  //Adjusts the angle to be always directioned to the targeted
		{
			Vector2 targetPosition = targetedObject.transform.position;
			
			float newAngle = Mathf.Rad2Deg * Mathf.Atan2(transform.position.x - targetPosition.x,targetPosition.y - transform.position.y);
			
			rigidbody2D.MoveRotation(newAngle);
		} else if(followingBehaviour == BehaviourChoices.followImperfect) //Adjusts the angle to be directioned to the targeted according to an increasing turning speed
		{
			BulletController controller = GetComponent<BulletController>();
			controller.bulletSpeed = Mathf.Lerp(controller.bulletSpeed, finalSpeed, Time.fixedDeltaTime);
			
			if(targetedObject == null)
				return;
			
			Vector2 targetPosition = targetedObject.transform.position;
			
			float targetedAngle = Mathf.Rad2Deg * Mathf.Atan2(transform.position.x - targetPosition.x,targetPosition.y - transform.position.y);
			float newAngle = Mathf.LerpAngle(rigidbody2D.rotation, targetedAngle, Time.fixedDeltaTime * initialTurningSpeed);
			
			initialTurningSpeed = Mathf.Lerp(initialTurningSpeed, finalTurningSpeed, Time.fixedDeltaTime / 2);
			
			rigidbody2D.MoveRotation(newAngle);
		}
	}

	public void setTargetedObject(GameObject tgtObject) 
	{
		targetedObject = tgtObject;
	}

	public void targetHit()
	{
		//TODO change the crosshair logic so that it's simplier to find and disable them
		if(targetedObject != null)
		{
			Transform targetCrosshair = targetedObject.transform.FindChild("TargetCross(Clone)");
			if(targetCrosshair != null)
				GameObject.Destroy(targetCrosshair.gameObject);
		}
			
	
		followingBehaviour = BehaviourChoices.Direction;
	}
}
