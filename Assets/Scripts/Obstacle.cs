using UnityEngine;
using System.Collections;

public class Obstacle : MonoBehaviour 
{

	public float maxSpeed = 10f;
	public float gap = 1f;
	
	private float headTheta = 0f;
	private float radius = 1f;
	private float speed = 0f;
	
	void Start () 
	{
		speed = Random.Range(maxSpeed/2f, maxSpeed);
		speed *= Mathf.Sign(Random.value - 0.5f);
		radius = Random.Range(Player.minRadius + 1, Player.maxRadius + 1);
	}
	
	
	void FixedUpdate () 
	{		
		headTheta -= speed * Time.deltaTime;
		
		for(int i = 1; i <= transform.childCount; i++)
		{
			float theta = headTheta - (gap * i)/radius;
			float x = radius * Mathf.Cos(theta);
			float y = radius * Mathf.Sin(theta);
			Vector3 pos = new Vector3(x, y);
			Transform part = transform.GetChild(i-1);
			part.rotation = Quaternion.Euler(new Vector3(0f, 0f, Mathf.Rad2Deg * theta));
			part.position = pos;
		}
	}
}
