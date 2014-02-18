using UnityEngine;
using System.Collections;

public class Player : MonoBehaviour
{
	public float orbitSpeed = 10f;
	public int maxRadius = 5;
	public int minRadius = 2;
	
	private float theta = 0f;
	private float radius;
	private int circle;
	private enum LaneChange {up, down};
	private bool changingLane = false;
	// Use this for initialization
	void Start () 
	{
		radius = minRadius;
		circle = minRadius;
	}
	float yIn;
	// Update is called once per frame
	void Update () 
	{
		
		 yIn = Input.GetAxisRaw("Vertical");
		if (! changingLane)
		{
			if (yIn > 0f && circle < maxRadius) StartCoroutine( ChangeRadius( (float)(circle + 1) ) );
			if (yIn < 0f && circle > minRadius) StartCoroutine( ChangeRadius( (float)(circle - 1) ) );
		}

		float xIn = Input.GetAxis("Horizontal");
		theta -= xIn * orbitSpeed * Time.deltaTime;
		

		float x = radius * Mathf.Cos(theta);
		float y = radius * Mathf.Sin(theta);
		Vector3 pos = new Vector3(x, y);
		transform.position = pos;
	}
	
	IEnumerator ChangeRadius(float newR)
	{
		
		changingLane = true;
		
		float oldR = radius;
		float s = Time.time;
		while (!Mathf.Approximately(radius, newR))
		{
			radius = Mathf.Lerp (oldR, newR, (Time.time - s)*orbitSpeed);
			yield return new WaitForFixedUpdate();
		}
		radius = newR;
		circle = (int) newR;
		changingLane = false;
	}
	
}
