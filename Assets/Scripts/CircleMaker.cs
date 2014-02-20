using UnityEngine;
using System.Collections;

/*  CircleMaker MonoBehaviour
 *  Creates a line renderer in a circle
 *  
 */

public class CircleMaker : MonoBehaviour 
{

	public float radius = 1f;			
	public int vertexCount = 16;
	public float lineStartWidth = 0.1f;
	public float lineEndWidth = 0.5f;
	public Material material;
	
	private LineRenderer line;
	
	// Line is created in Start and never modified afterwards
	// In future a we could have functions that modify the circle points etc.
	void Start () 
	{
		// Initialize LineRenderer
		line = gameObject.AddComponent<LineRenderer>();
		line.material = material;
		line.SetWidth(lineStartWidth, lineEndWidth);
		line.SetVertexCount(vertexCount+1);
		
		// Define LineRenderer points
		// Here, a circle is defined on cartesian plane X/Y 
		// ... using the parametric form "x = a + r cos(t), y = a + r sin(t)"
		for (int i = 0; i < vertexCount + 1; i++)
		{
			float theta = (i * Mathf.PI * 2f) / vertexCount;
			float x = radius * Mathf.Cos(theta);
			float y = radius * Mathf.Sin(theta);
			Vector3 pos = new Vector3(x, y);
			line.SetPosition(i,pos);
		}
	}
	

}
