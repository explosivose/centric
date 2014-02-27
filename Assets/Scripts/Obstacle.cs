using UnityEngine;
using System.Collections;

/*  Obstacle MonoBehaviour
 *  A snake that runs around a particular ring
 *  These snakes are made of several child gameObjects (body parts)
 */


public class Obstacle : MonoBehaviour 
{
	// Public Variables
	/*******************/
	
	// Note that speed is affecting rate of change of angle (not rate of change of distance)
	// May want to change the maths in future to calculate angular velocity or something
	// Because this way, outer snakes can move faster than inner snakes
	// (at time or writing player behaves the same way, moves faster on outer rings)
	
	public float maxSpeed = 10f;		// maximum speed for snake obstacle
	public float gap = 1f;				// the gap between snake body parts
	
	
	// Private variables
	/*******************/
	
	private float headTheta = 0f;		// the angle for the first body part (RADIANS)
	public float radius = 1f;			// the radius of the circle to run around on
	private float speed = 0f;			// the actual speed used (bound by the maxSpeed)
	
	void Start () 
	{
		// randomise the speed
		speed = Random.Range(maxSpeed/2f, maxSpeed);
		// random direction (random sign (+/-) for speed)
		speed *= Mathf.Sign(Random.value - 0.5f);
		// random circle to run around on
		radius = Random.Range(Player.minRadius + 1, Player.maxRadius + 1);
	}
	
	
	void FixedUpdate () 
	{
		// constantly modify angle of first body part (keep running around)
		// decrement angle for clockwise movement 
		// (doesn't actually matter here because speed sign (+/-) is randomised)
		headTheta -= speed * Time.deltaTime;
		
		// Move the body parts
		for(int i = 1; i <= transform.childCount; i++)
		{
			// position is based on angle and radius
			// angle for body part depends on gap between body parts
			float theta = headTheta - (gap * i)/radius;
			float x = radius * Mathf.Cos(theta);
			float y = radius * Mathf.Sin(theta);
			Vector3 pos = new Vector3(x, y);
			
			Transform part = transform.GetChild(i-1);
			part.position = pos;
			
			// Also modify rotation (comment out to see why we need to do this)
			// Note conversion from radians to degrees
			part.rotation = Quaternion.Euler(new Vector3(0f, 0f, Mathf.Rad2Deg * theta));
		}
	}
}
