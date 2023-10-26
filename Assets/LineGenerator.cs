using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LineGenerator : MonoBehaviour
{
    public Material material;
    public Vector3 cubeCenter;
    public Vector3 cubeRotation;
    public float cubeSideLength;
    public float focalLength;

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


        var squareVectors = GetFrontSquare();


        var frontScale = focalLength / ((cubeCenter.z - cubeSideLength * .5f) + focalLength);

        for (int i = 0; i < squareVectors.Length; i++ )
        {
            
            var deductedVector = cubeCenter - squareVectors[i];
            var rotatedVectors =  RotateBy(cubeRotation.z, deductedVector.x, deductedVector.y);
            squareVectors[i] = rotatedVectors;
        }
        for (int i = 0; i < squareVectors.Length; i++)
        {

            GL.Color(material.color);
            var point1 = squareVectors[i] * frontScale;
            GL.Vertex3(point1.x, point1.y, 0);
            var point2 = squareVectors[(i + 1) % squareVectors.Length] * frontScale;
            GL.Vertex3(point2.x, point2.y, 0);

        }

        GL.PopMatrix();
        GL.End();

    }

    public Vector2 RotateBy(float angle, float axis1, float axis2)
    {
        var firstAxis = axis1 * Mathf.Cos(angle) - axis2 * Mathf.Sin(angle);
        var secondAxis = axis2 * Mathf.Cos(angle) + axis1 * Mathf.Sin(angle);
        return new Vector2(firstAxis, secondAxis);
    }

    public Vector3[] GetFrontSquare()
    {
        var halfLength = cubeSideLength * .5f;

        return new[] {
            new Vector3(cubeCenter.x + halfLength, cubeCenter.y + halfLength, -halfLength),
            new Vector3(cubeCenter.x - halfLength, cubeCenter.y + halfLength, -halfLength),
            new Vector3(cubeCenter.x - halfLength, cubeCenter.y - halfLength, -halfLength),
            new Vector3(cubeCenter.x + halfLength, cubeCenter.y - halfLength, -halfLength),
        };
    }
}
