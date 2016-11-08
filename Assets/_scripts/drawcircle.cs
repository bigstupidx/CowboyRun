using UnityEngine;
using System.Collections;

public class drawcircle : MonoBehaviour {

    public float radius = 1.0f;

    public int pointCount = 20;
    private float angle;
    private LineRenderer render;

	// Use this for initialization
	void Start () {
        render = GetComponent<LineRenderer>();
        angle = 360.0f / pointCount;
        draw();
	}
	
	// Update is called once per frame
	void Update () {
	    
	}

    private void draw()
    {
        float curAngel = 0;
        render.SetVertexCount(pointCount+1);
        for (int i = 0; i <= pointCount+1; i++)
        {
            float x = radius * Mathf.Sin(curAngel / 180 * Mathf.PI);
            float z = radius * Mathf.Cos(curAngel / 180 * Mathf.PI);
            render.SetPosition(i, new Vector3(x,0.2f,z));
            curAngel += angle;
        }
    }
}
