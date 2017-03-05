using UnityEngine;
using System.Collections;

public class drawcircle : MonoBehaviour {

    public float radius = 1.0f;
	public float MinRadius = 0.4f;

    public int pointCount = 20;
    private float angle;
    private LineRenderer render;

	// Use this for initialization
	void Start () {
        render = GetComponent<LineRenderer>();
        angle = 360.0f / pointCount;
		draw (1);
	}
	
	// Update is called once per frame
	void Update () {
	    
	}

    public void draw(int jumpCount)
    {
		float t = radius - jumpCount*0.2f;
		float CurRadius = Mathf.Clamp (t, MinRadius, radius);
        float curAngel = 0;
        render.SetVertexCount(pointCount + 1);
        for (int i = 0; i <= pointCount; i++)
        {
			float x = CurRadius * Mathf.Sin(curAngel / 180 * Mathf.PI);
			float z = CurRadius * Mathf.Cos(curAngel / 180 * Mathf.PI);
            render.SetPosition(i, new Vector3(x,0.2f,z));
            curAngel += angle;
        }
    }
}
