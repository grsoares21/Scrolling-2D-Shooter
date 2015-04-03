using UnityEngine;
using System.Collections;

public class ScaleInterpolator : MonoBehaviour {

	public Vector2 scaleFrom;
	public Vector2 scaleTo;

	public float interpolationSpeed;

	// Use this for initialization
	void Start () {
		transform.localScale = scaleFrom;
	}
	
	// Update is called once per frame
	void Update () {
		transform.localScale = Vector2.Lerp(transform.localScale, scaleTo, Time.deltaTime * interpolationSpeed);
	}
}
