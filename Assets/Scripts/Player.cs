using UnityEngine;
using System.Collections;

/*  Player MonoBehaviour
 *  Controls the player movement via Input
 *  Handles player collisions and scoring
 *  
 */

public class Player : MonoBehaviour
{
	// Public Variables
	/*******************/
	
	public float speed = 10f;			// player movement speed
	public Transform token;				// the object that spawns on the outer ring
	
	// Player sounds
	public AudioClip move;				// played on a loop, pitch/volume depends on Input
	public AudioClip pickup;			// played when player picks up token
	public AudioClip death;				// played when player collides with obstacle
	public AudioClip scorePoint;		// played when player scores a point
	
	// Static variables (not accessible via Unity Inspector)
	public static int maxRadius = 5;	// furthest distance from center (0,0)
	public static int minRadius = 2;	// shortest distance from center (0,0)
	public static int score = 0;		// player score
	
	// Private variables
	/*******************/
	
	// Player position is calculated using an angle and a radius
	private float theta = 0f;			// angle 
	private float radius = 1f;
	private int circle = 2;				// integer value of radius
	
	// Internal states
	private bool changingLane = false;	// true when radius is not an integer (between circles)
	private bool hasToken = false;		// true when player has picked up token
	private bool scoring = false;		// true when player is scoring (prevents player picking up token again)
	
	
	void Start () 
	{
		// Start on the inner circle
		radius = minRadius;
		circle = minRadius;
		
		// Spawn the token into the scene and place it on the outer circle
		token = Instantiate(token) as Transform;
		PlaceToken();
		
		// move sound is not a one-time audio clip
		// easier to control via MonoBehaviour.audio component interface
		gameObject.AddComponent<AudioSource>().clip = move;
		audio.loop = true;
		audio.Play();
	}

	// Place token on a random point on the circumference of the outer circle
	void PlaceToken()
	{
		Transform token = GameObject.FindGameObjectWithTag("Token").transform;
		float theta = Random.Range(0f, 2f * Mathf.PI);
		float x = maxRadius * Mathf.Cos(theta);
		float y = maxRadius * Mathf.Sin(theta);
		token.position = new Vector3(x, y);
	}
	
	
	void FixedUpdate () 
	{
		// Get up/down Input
		float yIn = Input.GetAxisRaw("Vertical");
		
		// Don't try to change lane whilst we're in the middle of changing lane
		if (! changingLane)
		{
			// Don't change lane more than radius bounds (maxRadius/minRadius)
			// Changing lane is controlled in ChangeRadius coroutine
			if (yIn > 0f && circle < maxRadius) StartCoroutine( ChangeRadius( (float)(circle + 1) ) );
			if (yIn < 0f && circle > minRadius) StartCoroutine( ChangeRadius( (float)(circle - 1) ) );
		}

		// Get left/right Input
		float xIn = Input.GetAxis("Horizontal");
		
		// Modify angle based on left/right input
		// Note that, because position is based on angle and radius
		// ... player will move faster on outer circles
		theta -= xIn * speed * Time.deltaTime;
		
		// Find position on circle using radius and angle
		// ... this is a parametric form of a circle on a cartesian plane (X/Y)
		// ... x = a + r cos(t), y = b + r sin(t)
		float x = radius * Mathf.Cos(theta);
		float y = radius * Mathf.Sin(theta);
		Vector3 pos = new Vector3(x, y);
		transform.position = pos;
		
		
		if (hasToken)
		{
			// If player has picked up the token, move the token with the player
			token.position = pos;
			
			// If player reaches the middle with the token, score!
			if (radius == minRadius)
				StartCoroutine (Score ());
		}
		
		// Modify "move" sound
		// pitch depends on radius (distance from center)
		audio.pitch = radius / maxRadius;
		// volume depends on input (sounds louder when player moves)
		audio.volume = Mathf.Abs(xIn) + 0.2f;
		
		// camera
		Transform cam = Camera.main.transform;
		Vector3 camPos = transform.position / 2f;
		camPos.z = cam.position.z;
		cam.position = camPos;
	}
	
	// Coroutine for modifying radius over time
	IEnumerator ChangeRadius(float newR)
	{
		// set lane changing flag
		changingLane = true;
		
		// store start radius and start time for Lerp function
		float oldR = radius;
		float s = Time.time;
		
		// Lerp until we're close to the end point
		while (!Mathf.Approximately(radius, newR))
		{
			radius = Mathf.Lerp (oldR, newR, (Time.time - s)*speed);
			
			// This statement means "go away and come back
			// ... when the next FixedUpdate arrives"
			yield return new WaitForFixedUpdate();
		}
		
		// Clamp radius to newR, prevent wandering due to floating point errors
		radius = newR;
		
		// circle is the integer value of radius, used to prevent wandering, too
		circle = (int) newR;
		
		// unset flag
		changingLane = false;
	}
	
	// Coroutine for scoring a point
	// This has a very similar structure to ChangeRadius() coroutine
	// Some comments are therefore omitted
	IEnumerator Score()
	{
		// scoring flag required to prevent player continually picking up token
		scoring = true;
		hasToken = false;
		
		// Lerp token to the center
		Vector3 oldPos = token.position;
		float s = Time.time;
		while(token.position.magnitude > 0.1f)
		{
			token.position = Vector3.Lerp(oldPos, Vector3.zero, (Time.time - s));
			yield return new WaitForFixedUpdate();
		}
		
		// Once token has reached the center, increment score and play a sound
		score++;
		AudioSource.PlayClipAtPoint(scorePoint, transform.position);
		
		// Spawn a new token after a short time
		yield return new WaitForSeconds(0.5f);
		PlaceToken();
		
		scoring = false;
	}
	
	// Player collisions
	void OnTriggerEnter2D(Collider2D col)
	{
		// If player touched the token
		if ( col.tag == "Token")
		{
			// ... and the player isn't scoring and doesn't already have the token
			if (!scoring && !hasToken)
			{
				// Pick up the token
				hasToken = true;
				AudioSource.PlayClipAtPoint(pickup, transform.position);
			} 
		}
		// Otherwise assume the player touched something lethal
		else
		{
			Debug.DrawLine(transform.position, col.transform.position, Color.red, Mathf.Infinity);
			speed = 0f;
			AudioSource.PlayClipAtPoint(death, transform.position);
			audio.Stop();
		}
	}
	
	// Cheap and dirty GUI / restart level control
	void OnGUI()
	{
		GUI.Label(new Rect(Screen.width/2f, 10f, 100f, 30f), Player.score.ToString());
		if (Input.GetKey(KeyCode.Escape)) Restart();
	}
	
	// Restart the game
	void Restart()
	{
		Application.LoadLevel(0);
		score = 0;
	}
	
}
