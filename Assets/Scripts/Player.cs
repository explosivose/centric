using UnityEngine;
using System.Collections;

public class Player : MonoBehaviour
{
	public float speed = 10f;
	public Transform token;
	public AudioClip move;
	public AudioClip pickup;
	public AudioClip death;
	public AudioClip scorePoint;
	
	
	public static int maxRadius = 5;
	public static int minRadius = 2;
	public static int score = 0;
	
	private float theta = 0f;
	private float radius = 1f;
	private int circle = 2;
	private bool changingLane = false;
	private bool hasToken = false;
	private bool scoring = false;
	
	
	void Start () 
	{
		radius = minRadius;
		circle = minRadius;
		token = Instantiate(token) as Transform;
		PlaceToken();
		gameObject.AddComponent<AudioSource>().clip = move;
		audio.loop = true;
		audio.Play();
	}

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
		float yIn = Input.GetAxisRaw("Vertical");
		if (! changingLane)
		{
			if (yIn > 0f && circle < maxRadius) StartCoroutine( ChangeRadius( (float)(circle + 1) ) );
			if (yIn < 0f && circle > minRadius) StartCoroutine( ChangeRadius( (float)(circle - 1) ) );
		}

		
		float xIn = Input.GetAxis("Horizontal");
		theta -= xIn * speed * Time.deltaTime;
		
		float x = radius * Mathf.Cos(theta);
		float y = radius * Mathf.Sin(theta);
		Vector3 pos = new Vector3(x, y);
		transform.position = pos;
		
		if (hasToken)
		{
			token.position = pos;
			if (radius == minRadius)
				StartCoroutine (Score ());
		}
		
		audio.pitch = radius / maxRadius;
		audio.volume = Mathf.Abs(xIn) + 0.2f;
	}
	
	IEnumerator ChangeRadius(float newR)
	{
		changingLane = true;
		
		float oldR = radius;
		float s = Time.time;
		while (!Mathf.Approximately(radius, newR))
		{
			radius = Mathf.Lerp (oldR, newR, (Time.time - s)*speed);
			yield return new WaitForFixedUpdate();
		}
		radius = newR;
		circle = (int) newR;
		changingLane = false;
	}
	
	IEnumerator Score()
	{
		scoring = true;
		hasToken = false;
		Vector3 oldPos = token.position;
		float s = Time.time;
		while(token.position.magnitude > 0.1f)
		{
			token.position = Vector3.Lerp(oldPos, Vector3.zero, (Time.time - s));
			yield return new WaitForFixedUpdate();
		}
		score++;
		AudioSource.PlayClipAtPoint(scorePoint, transform.position);
		
		yield return new WaitForSeconds(0.5f);
		PlaceToken();
		scoring = false;
	}
	
	
	void OnTriggerEnter2D(Collider2D col)
	{
		if ( col.tag == "Token")
		{
			if (!scoring && !hasToken)
			{
				hasToken = true;
				AudioSource.PlayClipAtPoint(pickup, transform.position);
			} 
		}
		else
		{
			Debug.DrawLine(transform.position, col.transform.position, Color.red, Mathf.Infinity);
			speed = 0f;
			AudioSource.PlayClipAtPoint(death, transform.position);
			audio.Stop();
		}
	}
	
	void OnGUI()
	{
		GUI.Label(new Rect(Screen.width/2f, 10f, 100f, 30f), Player.score.ToString());
		if (Input.GetKey(KeyCode.Escape)) Restart();
	}
	
	void Restart()
	{
		Application.LoadLevel(0);
		score = 0;
	}
	
}
