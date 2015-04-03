using UnityEngine;
using System.Collections;

[RequireComponent(typeof(PlaneController), typeof(Animator))]
public abstract class PlaneManager : MonoBehaviour {

	public PlaneController controller;

	public int nbSpecialBullets;
	public int health;	
	public int currentHealth;

	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	protected void Explode () 
	{
		GetComponent<Animator>().SetTrigger("Explode");
	}

	public int getCurrentHealth()
	{
		return currentHealth;
	}

	public abstract void gotHit(int hitStrength);

}
