using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LineGenerator : MonoBehaviour
{
    public Vector2 point1;
    public Vector2 point2;
    public Material material;

    private void OnPostRender()
    {
        DrawLine();
    }

    public void OnDrawGizmos()
    {
        DrawLine();
    }

    public void DrawLine()
    {

        if (material == null)
        {
            Debug.LogError("You need to add a material");
            return; 
        }
        GL.PushMatrix();

        GL.Begin(GL.LINES);
        material.SetPass(0);
        GL.Color(material.color);
        GL.Vertex3(point1.x, point1.y, 0);
        GL.Color(Color.blue);
        GL.Vertex3(point2.x, point2.y, 0);



        GL.PopMatrix();
        GL.End();

    }
}
