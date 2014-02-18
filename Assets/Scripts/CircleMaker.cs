using UnityEngine;
using System.Collections;

public class CircleMaker : MonoBehaviour 
{

	public float radius = 1f;
	public int vertexCount = 16;
	public float lineStartWidth = 0.1f;
	public float lineEndWidth = 0.5f;
	public Material material;
	
	public int v;
	
	private LineRenderer line;
	
	// Use this for initialization
	void Start () 
	{
		line = gameObject.AddComponent<LineRenderer>();
		line.material = material;
		line.SetWidth(lineStartWidth, lineEndWidth);
		
		line.SetVertexCount(vertexCount+1);
		
		for (int i = 0; i < vertexCount + 1; i++)
		{
			float theta = (i * Mathf.PI * 2f) / vertexCount;
			float x = radius * Mathf.Cos(theta);
			float y = radius * Mathf.Sin(theta);
			Vector3 pos = new Vector3(x, y);
			line.SetPosition(i,pos);
		}
	}
	
	// Update is called once per frame
	void Update () {

	}
}
