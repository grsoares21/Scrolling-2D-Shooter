using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody2D), typeof(Animator))]
public class BulletController : MonoBehaviour {

	public float bulletSpeed;
	public int bulletStrength;
	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {
		rigidbody2D.velocity = transform.up * bulletSpeed;
	}

	void OnCollisionEnter2D(Collision2D collision)
	{
		if(collision.gameObject.tag == "Enemy" || collision.gameObject.tag == "Player") {
			collision.gameObject.GetComponent<PlaneManager>().gotHit(bulletStrength);

			SpecialBulletController specialController = GetComponent<SpecialBulletController>();
			if(specialController != null)
				specialController.targetHit();
		} //Tells the plane manager it has been hit.

		foreach(Collider2D col in GetComponents<Collider2D>())
			col.enabled = false;

		rigidbody2D.isKinematic = true;
		rigidbody2D.velocity = Vector2.zero; 
		//Stops the bullet from moving


		Explode();

	}

	void Explode() 
	{
		Animator animator = GetComponent<Animator>();	
		animator.SetTrigger("ExplosionTrigger"); //Starts explosion animation
	}

	void OnBecameInvisible() {
		Die();
	}

	private void Die() {
		Destroy(this.gameObject);
	}
}
